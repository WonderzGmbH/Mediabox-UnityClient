using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mediabox.API;
using Mediabox.GameKit.Bundles;
using Mediabox.GameKit.Game;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameKit.Pause;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Mediabox.GameKit.GameManager {
    /// <summary>
    /// This class handles all communication with the <see cref="IMediaboxServer"/>
    /// You need to implement this class once in your project and place it in your starting scene.
    /// You can import the Package Samples with Unity's Package Manager, if you want to get started quickly.
    /// </summary>
    /// <typeparam name="TGameDefinition">The type of GameDefinition that you use in your project. GameDefinitions will be stored separately on the Mediabox-Backend and allow you to launch different types of game within one Unity Client. This feature can be configured and disabled using the "Game Definition Hub" that can be found in the "Mediabox"-Menu.</typeparam>
    public abstract class GameManagerBase<TGameDefinition> : GameManagerBase, IMediaboxClient, IGameAPI {

        IMediaboxServer _mediaboxServer;
        string _language;
        float _userScore;
        string _saveGamePath;
        IBundle _loadedBundle;
        static GameManagerBase<TGameDefinition> instance;
        [Obsolete("Use `GameBase.API instead.")]
        public static GameManagerBase<TGameDefinition> Instance => instance;

        protected virtual string DefaultSceneName => "StartScene";

        readonly IPauseSynchronizationService _pauseSynchronizationService = new PauseSynchronizationService();
        private PauseHandle pauseHandle;

        #region UnityEventFunctions
        void Awake() {
            EnsureSingletonInstance();
            if (!Application.isEditor) {
                SetNativeApi(CreateNativeAPI());
            }
        }

        void OnDestroy() {
            CleanUpSingletonInstance();
        }
        #endregion // UnityEventFunctions
        
        #region PublicAPI
        /// <summary>
        /// This method needs to be called so the API can be initialized.
        /// </summary>
        /// <param name="mediaboxServer">An implementation of NativeAPI.</param>
        /// <exception cref="NativeApiAlreadySetupException">Thrown, if SetNativeApi has already been called.</exception>
        public override void SetNativeApi(IMediaboxServer mediaboxServer) {
            if(this._mediaboxServer != null)
                throw new NativeApiAlreadySetupException();
            this._mediaboxServer = mediaboxServer;
            this._mediaboxServer.InitializeApi(this.gameObject.name);
        }

        [Obsolete("Invoke QuitGame instead.")]
        public void QuitApplication() => (this as IGameAPI).Quit();
        #endregion // PublicAPI

        #region IGameAPI
        
        public float UserScore => _userScore;

        void IGameAPI.Quit() {
            this._mediaboxServer.OnGameExitRequested();
        }

        bool HasContentBundle => this._loadedBundle != null;
        void IGameAPI.ReportNewUserScore(float newScore)
        {
            if (Math.Abs(this._userScore - newScore) < float.Epsilon)
                return;
            this._userScore = newScore;
            FindGame()?.SetScore(newScore);
            this._mediaboxServer.OnUserScoreChanged(newScore);
        }

        async Task<T> IGameAPI.LoadAssetFromContentBundle<T>(string assetPath) {
            return await this._loadedBundle.LoadAssetAsync<T>(assetPath);
        }

        async Task IGameAPI.LoadSceneFromContentBundle<T>(string assetPath, LoadSceneMode loadSceneMode) {
            await this._loadedBundle.LoadScene(assetPath, loadSceneMode);
        }

        #endregion // IGameAPI

        // All methods in this region will be called by the NativeAPI. Refer to NativeAPI.cs for details.
        #region IMediaboxCallbacks
        public void SetContentLanguage(string locale) {
            this._language = locale;
            FindGame()?.SetLanguage(this._language);
        }

        public void SetSaveDataFolder(string path) {
            this._saveGamePath = path;
            FindGame()?.Load(path);
        }

        public void SetUserScore(string scoreJson)
        {
            this._userScore = JsonUtility.FromJson<UserScoreData>(scoreJson).value;
            FindGame()?.SetScore(this._userScore);
        }

        public async void SetContentBundleFolder(string path){
            try {
                await ResetGame();
                var definition = default(TGameDefinition);
                var settings = GameDefinitionSettings.Load();
                path = FixWronglyZippedArchive(path);
                if (settings.useGameDefinitionJsonFile) {
                    definition = LoadGameDefinition(path, settings);
                    if (definition is IGameBundleDefinition gameBundleDefinition) {
                        await LoadGameDefinitionBundle(path, gameBundleDefinition);
                        if (definition is IGameSceneDefinition gameSceneDefinition) {
                            if (definition is IGameBundleSceneDefinition gameBundleSceneDefinition)
                                await this._loadedBundle.LoadScene(gameBundleSceneDefinition.SceneName);
                            else
                                await LoadGameDefinitionScene(gameSceneDefinition);
                        } else {
                            //await LoadAllScenesInBundle(this.loadedBundle);
                        }
                    }
                }
                await OnStartGame(path, definition, this._saveGamePath);
                FindGame().Initialize(this, this._pauseSynchronizationService);
                await FindGame().Load(this._saveGamePath);
                await FindGame().SetScore(this._userScore);
                await FindGame().SetLanguage(this._language);
                await FindGame().StartGame(path, definition);
                this._mediaboxServer.OnLoadingSucceeded();
            } catch (Exception e) {
                Debug.LogException(e);
                this._mediaboxServer.OnLoadingFailed();
            }
        }

        // static async Task LoadAllScenesInBundle(IBundle bundle) {
        //     var scenePaths = bundle.GetAllScenePaths();
        //     if (scenePaths.Length <= 0) 
        //         return;
        //     await SceneManager.LoadSceneAsync(scenePaths[0]);
        //     for (var i = 1; i < scenePaths.Length; i++) {
        //         await SceneManager.LoadSceneAsync(scenePaths[i], LoadSceneMode.Additive);
        //     }
        // }

        public async void WriteSaveData(string path) {
            try {
                var saveTask = FindGame()?.Save(path);
                if (saveTask != null)
                    await saveTask;
            } catch (Exception e) {
                Debug.LogException(e);
            }
            this._mediaboxServer.OnSaveDataWritten();
        }
        
        public void PauseApplication() {
            if (this.pauseHandle != null)
            {
                Debug.LogWarning($"Warning, the application was paused multiple times. If you intend to have multiple pause handles, please use {nameof(GameBase<object>)}.{nameof(GameBase<object>.pauseSynchronizationService)}");
                return;
            }
            this.pauseHandle = this._pauseSynchronizationService.Pause();
        }

        public void UnpauseApplication() {
            if (this.pauseHandle == null)
            {
                Debug.LogWarning($"Warning, the application was unpaused while not paused. If you intend to have multiple pause handles, please use {nameof(GameBase<object>)}.{nameof(GameBase<object>.pauseSynchronizationService)}");
                return;
            }
            this._pauseSynchronizationService.Unpause(this.pauseHandle);
            this.pauseHandle = null;
        }
        
        public void CreateScreenshot() {
            _ = CreateScreenshotAsync();
        }

        async Task CreateScreenshotAsync() {
            var fileName = $"{Application.productName}-{DateTime.Now.ToString(("yyyyMMddHHmmssfff"))}.png";
            var fullFilePath = Application.installMode == ApplicationInstallMode.Editor ? fileName : Path.Combine(Application.persistentDataPath, fileName);
            ScreenCapture.CaptureScreenshot(fileName);
            var startTime = Time.realtimeSinceStartup;
            const float timeout = 5f;
            while (!FileHelper.Exists(fullFilePath)) {
                if (Time.realtimeSinceStartup > startTime + timeout) {
                    this._mediaboxServer.OnCreateScreenshotFailed();
                    return;
                }
                await Task.Delay(100);
            }
            this._mediaboxServer.OnCreateScreenshotSucceeded(fullFilePath);
        }

        public async void UnloadGameContent() {
            await ResetGame();
            await Resources.UnloadUnusedAssets();
            this._mediaboxServer.OnUnloadingSucceeded();
        }

        async Task ResetGame() {
            this._pauseSynchronizationService.Reset();
            if (SceneManager.GetActiveScene().name != this.DefaultSceneName)
                await SceneManager.LoadSceneAsync(this.DefaultSceneName);
            UnloadBundle();
            this._userScore = default;
        }

        void UnloadBundle() {
            if (this.HasContentBundle) {
                this._loadedBundle.Unload();
                this._loadedBundle = null;
            }
        }

        #endregion // IMediaboxCallbacks

        /// <summary>
        /// Implement this method to add your custom StartGame-Logic.
        /// This won't be necessary in most cases, as implementing IGameSceneDefinition and IGameBundleDefinition in TGameDefinition will be enough.
        /// </summary>
        /// <param name="contentBundleFolderPath">The path where all downloaded files will be stored.</param>
        /// <param name="definition">The game definition file. It may be null, if configured so in GameDefinitionSettings.</param>
        /// <param name="saveGamePath">The path from which to load and in which to store the SaveGame-File.</param>
        protected abstract Task OnStartGame(string contentBundleFolderPath, [CanBeNull] TGameDefinition definition, string saveGamePath);

        /// <summary>
        /// Overload this method, if you want to implement additional Native APIs.
        /// </summary>
        /// <returns>INativeAPI instance to handle Mediabox communication.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual IMediaboxServer CreateNativeAPI() {
            var server = GetMediaboxServerFromFactory();
            if (server != null)
                return server;
            switch (Application.platform) {
                case RuntimePlatform.Android:
                    return new AndroidMediaboxServer();
                case RuntimePlatform.IPhonePlayer:
                    return new IosMediaboxServer();
                default:
                    throw new ArgumentOutOfRangeException(nameof(Application.platform), Application.platform, string.Empty);
            }
        }

        IMediaboxServer GetMediaboxServerFromFactory() {
            return GetComponents<IMediaboxServerFactory>()
                .OrderBy(factory => factory.Priority)
                .FirstOrDefault()?.Create();
        }
        
        void EnsureSingletonInstance() {
            DontDestroyOnLoad(this);
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);
        }
        
        void CleanUpSingletonInstance() {
            if (instance == this)
                instance = null;
        }

        async Task LoadGameDefinitionBundle(string path, IGameBundleDefinition gameBundleDefinition) {
            var bundlePath = Path.Combine(path, gameBundleDefinition.BundleName);
            var bundle = BundleManager.Load(bundlePath);
            if (bundle == null) {
                this._mediaboxServer.OnLoadingFailed();
                throw new Exception($"No AssetBundle found at contentBundleFolderPath {bundlePath}. Either, the contentBundleFolderPath has not been set up correctly in the GameDefinition, or the bundle has not been placed in the correct place.");
            }

            await bundle;
            this._loadedBundle = bundle.Result;
        }
        
        static async Task LoadGameDefinitionScene(IGameSceneDefinition gameSceneDefinition) {
            await SceneManager.LoadSceneAsync(gameSceneDefinition.SceneName);
        }

        static TGameDefinition LoadGameDefinition(string path, GameDefinitionSettings settings) {
            var jsonPath = Path.Combine(path, settings.gameDefinitionFileName);
            if (!FileHelper.Exists(jsonPath)) {
                throw new Exception($"No {settings.gameDefinitionFileName} found at contentBundleFolderPath {path}.");
            }

            var jsonString = FileHelper.ReadAllText(jsonPath);
            var definition = JsonUtility.FromJson<TGameDefinition>(jsonString);
            return definition;
        }

        // This method will check for content folders where the whole folder has been zipped instead of the folder contents only.
        // That means, that instead of the expected contentBundleFolderPath of e.g. GameA/index.json, files can be found at GameA/GameA/index.json
        static string FixWronglyZippedArchive(string path) {
            if (!DirectoryHelper.Exists(path)) {
                throw new Exception($"Directory {path}, which was provided by MediaboxAPI, does not exist.");
            }

            var directoryName = Path.GetFileName(path);
            if (directoryName == null) {
                throw new Exception($"Directory {path}, does not have a valid directory name.");
            }

            if (OnlyTheGivenDirectoryExistsInDirectory(path, directoryName) && NoNonSystemFileExistsInDirectory(path) ) {
                path = Path.Combine(path, directoryName);
            }

            return path;
        }

        static bool OnlyTheGivenDirectoryExistsInDirectory(string path, string directoryName) {
            var directories = DirectoryHelper.GetDirectories(path).ToArray();
            return directories.Length == 1 && directories[0] == directoryName;
        }

        static bool NoNonSystemFileExistsInDirectory(string path)
        {
            // TODO: FIX!!
            return true; // directoryInfo.EnumerateFiles().All(file => file.Name == ".DS_Store");
        }
    
        static IGame<TGameDefinition> FindGame() {
            return Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<IGame<TGameDefinition>>().FirstOrDefault();
        }

        class NativeApiAlreadySetupException : Exception {
            public NativeApiAlreadySetupException() : base("Native API has been assigned already. Reassigning is not supported currently.") { }
        }
    }

    public abstract class GameManagerBase : MonoBehaviour {
        public abstract void SetNativeApi(IMediaboxServer mediaboxServer);
    }
}