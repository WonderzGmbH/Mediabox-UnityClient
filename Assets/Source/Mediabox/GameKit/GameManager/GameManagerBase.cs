using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mediabox.API;
using Mediabox.GameKit.Bundles;
using Mediabox.GameKit.Game;
using Mediabox.GameKit.GameDefinition;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mediabox.GameKit.GameManager {
    /// <summary>
    /// This class handles all communication with the Mediabox-NativeAPI.
    /// You need to implement this class once in your project and place it in your starting scene.
    /// </summary>
    /// <typeparam name="TGameDefinition">The type of GameDefinition that you use in your project. GameDefinitions will be stored separately on the Mediabox-Backend and allow you to launch different types of game within one Unity Client. This feature can be configured and disabled using the "Game Definition Manager" folder under the "Mediabox"-Menu.</typeparam>
    public abstract class GameManagerBase<TGameDefinition> : GameManagerBase, IMediaboxCallbacks {

        INativeAPI nativeApi;
        string language;
        string saveGamePath;
        IBundle loadedBundle;
        static GameManagerBase<TGameDefinition> instance;

        /// <summary>
        /// This method needs to be called so the API can be initialized.
        /// </summary>
        /// <param name="nativeApi">An implementation of NativeAPI.</param>
        /// <exception cref="NativeApiAlreadySetupException">Thrown, if SetNativeApi has already been called.</exception>
        public override void SetNativeApi(INativeAPI nativeApi) {
            if(this.nativeApi != null)
                throw new NativeApiAlreadySetupException();
            this.nativeApi = nativeApi;
            this.nativeApi.InitializeApi(this.gameObject.name);
        }

        class NativeApiAlreadySetupException : Exception {
            public NativeApiAlreadySetupException() : base("Native API has been assigned already. Reassigning is not supported currently.") { }
        }

        // All methods in this region will be called by the NativeAPI. Refer to NativeAPI.cs for details.
        #region IMediaboxCallbacks
        public void SetContentLanguage(string locale) {
            this.language = locale;
            FindGame()?.SetLanguage(this.language);
        }

        public void SetSaveDataFolder(string path) {
            this.saveGamePath = path;
            FindGame()?.Load(path);
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
                                await this.loadedBundle.LoadScene(gameBundleSceneDefinition.SceneName);
                            else
                                await LoadGameDefinitionScene(gameSceneDefinition);
                        } else {
                            //await LoadAllScenesInBundle(this.loadedBundle);
                        }
                    }
                }
                await OnStartGame(path, definition, this.saveGamePath);
                await FindGame().Load(this.saveGamePath);
                await FindGame().SetLanguage(this.language);
                await FindGame().StartGame(path, definition);
                this.nativeApi.OnLoadingSucceeded();
            } catch (Exception e) {
                Debug.LogException(e);
                this.nativeApi.OnLoadingFailed();
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
            await FindGame()?.Save(path);
            this.nativeApi.OnSaveDataWritten();
        }

        float timeScaleBeforePause;
        float volumeBeforePause;
        
        public void PauseApplication() {
            this.timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
            this.volumeBeforePause = AudioListener.volume;
            AudioListener.volume = 0f;
        }

        public void UnpauseApplication() {
            Time.timeScale = this.timeScaleBeforePause;
            AudioListener.volume = this.volumeBeforePause;
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
            while (!File.Exists(fullFilePath)) {
                if (Time.realtimeSinceStartup > startTime + timeout) {
                    this.nativeApi.OnCreateScreenshotFailed();
                    return;
                }
                await Task.Delay(100);
            }
            this.nativeApi.OnCreateScreenshotSucceeded(fullFilePath);
        }

        public async void UnloadGameContent() {
            await ResetGame();
            await Resources.UnloadUnusedAssets();
            this.nativeApi.OnUnloadingSucceeded();
        }

        async Task ResetGame() {
            if (SceneManager.GetActiveScene().name != "StartScene")
                await SceneManager.LoadSceneAsync("StartScene");
            if (this.loadedBundle != null) {
                this.loadedBundle.Unload();
                this.loadedBundle = null;
            }
        }

        #endregion // IMediaboxCallbacks

        public bool HasContentBundle => this.loadedBundle != null;
        public async Task<T> LoadAssetFromContentBundle<T>(string assetPath) where T:UnityEngine.Object {
            return await this.loadedBundle.LoadAssetAsync<T>(assetPath);
        }
        
        /// <summary>
        /// Implement this method to add your custom StartGame-Logic.
        /// This won't be necessary in most cases, as implementing IGameSceneDefinition and IGameBundleDefinition in TGameDefinition will be enough.
        /// </summary>
        /// <param name="contentBundleFolderPath">The path where all downloaded files will be stored.</param>
        /// <param name="definition">The game definition file. It may be null, if configured so in GameDefinitionSettings.</param>
        /// <param name="saveGamePath">The path from which to load and in which to store the SaveGame-File.</param>
        protected abstract Task OnStartGame(string contentBundleFolderPath, [CanBeNull] TGameDefinition definition, string saveGamePath);

        static async Task LoadGameDefinitionScene(IGameSceneDefinition gameSceneDefinition) {
            await SceneManager.LoadSceneAsync(gameSceneDefinition.SceneName);
        }

        async Task LoadGameDefinitionBundle(string path, IGameBundleDefinition gameBundleDefinition) {
            var bundlePath = Path.Combine(path, gameBundleDefinition.BundleName);
            var bundle = BundleManager.Load(bundlePath);
            if (bundle == null) {
                this.nativeApi.OnLoadingFailed();
                throw new Exception($"No AssetBundle found at contentBundleFolderPath {bundlePath}. Either, the contentBundleFolderPath has not been set up correctly in the GameDefinition, or the bundle has not been placed in the correct place.");
            }

            await bundle;
            this.loadedBundle = bundle.Result;
        }

        static TGameDefinition LoadGameDefinition(string path, GameDefinitionSettings settings) {
            var jsonPath = Path.Combine(path, settings.gameDefinitionFileName);
            if (!File.Exists(jsonPath)) {
                throw new Exception($"No {settings.gameDefinitionFileName} found at contentBundleFolderPath {path}.");
            }

            var jsonString = File.ReadAllText(jsonPath);
            var definition = JsonUtility.FromJson<TGameDefinition>(jsonString);
            return definition;
        }

        // This method will check for content folders where the whole folder has been zipped instead of the folder contents only.
        // That means, that instead of the expected contentBundleFolderPath of e.g. GameA/index.json, files can be found at GameA/GameA/index.json
        static string FixWronglyZippedArchive(string path) {
            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists) {
                throw new Exception($"Directory {path}, which was provided by MediaboxAPI, does not exist.");
            }

            var directoryName = Path.GetFileName(path);
            if (directoryName == null) {
                throw new Exception($"Directory {path}, does not have a valid directory name.");
            }

            if (OnlyTheGivenDirectoryExistsInDirectory(directoryInfo, directoryName) && NoNonSystemFileExistsInDirectory(directoryInfo) ) {
                path = Path.Combine(path, directoryName);
            }

            return path;
        }

        static bool OnlyTheGivenDirectoryExistsInDirectory(DirectoryInfo directoryInfo, string directoryName) {
            var directories = directoryInfo.GetDirectories();
            return directories.Length == 1 && directories[0].Name == directoryName;
        }

        static bool NoNonSystemFileExistsInDirectory(DirectoryInfo directoryInfo) {
            return directoryInfo.EnumerateFiles().All(file => file.Name == ".DS_Store");
        }
    
        static IGame<TGameDefinition> FindGame() {
            return Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<IGame<TGameDefinition>>().FirstOrDefault();
        }

        /// The awake method makes sure that only one GameManager exists at all times, that it won't be destroyed on scene changes and that in Non-Editor-Environments the MediaboxNativeAPI is hooked onto this script.
        void Awake() {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);
            if(!Application.isEditor)
                SetNativeApi(new MediaboxNativeAPI());
            DontDestroyOnLoad(this);
        }

        /// Cleans up the singleton instance.
        void OnDestroy() {
            if (instance == this)
                instance = null;
        }
    }

    public abstract class GameManagerBase : MonoBehaviour {
        public abstract void SetNativeApi(INativeAPI nativeAPI);
    }
}