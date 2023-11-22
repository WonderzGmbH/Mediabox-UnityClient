namespace Mediabox.API {
    /// <summary>
    /// Interface for incoming calls from Mediabox-Server.
    /// You can implement this interface yourself, or use one of our standard base classes.
    /// e.g. <see cref="Mediabox.GameKit.GameManager.GameManagerBase"/>
    /// </summary>
    public interface IMediaboxClient
    {
        /// <summary>
        /// This method is called by <see cref="IMediaboxServer"/> to set the language selected by the user for content.
        /// The method is called after initialization, before save data and content bundle folder are set.
        /// </summary>
        /// <param name="locale">An identifier for the locale of the game</param>
        void SetContentLanguage(string locale);

        /// <summary>
        /// This method is called by <see cref="IMediaboxServer"/>, when assets were downloaded & extracted to the local file system.
        /// You cannot store/modify files permanently here, the folder is located in the temporary folder.
        /// This method is always called after SetSaveDataFolder().
        /// When content loading AND save game loading is complete,
        /// either <see cref="IMediaboxServer.OnLoadingSucceeded()"/> or <see cref="IMediaboxServer.OnLoadingFailed()"/> must be called.
        /// </summary>
        /// <param name="path">The path to the content bundle folder which contains the requested Game Definition.</param>
        void SetContentBundleFolder(string path);

        /// <summary>
        /// This method is called by <see cref="IMediaboxServer"/>, when save data should be loaded.
        /// On first launch, this folder might be empty.
        /// This method is always called before SetContentBundleFolder().
        /// When content loading AND save game loading is complete,
        /// either <see cref="IMediaboxServer.OnLoadingSucceeded"/> or <see cref="IMediaboxServer.OnLoadingFailed"/> must be called.
        /// </summary>
        /// <param name="path">The path to the folder that's been provided for you to Read and Write Data to.</param>
        void SetSaveDataFolder(string path);
        
        /// <summary>
        /// This method is called by <see cref="IMediaboxServer"/>, when save data should be loaded.
        /// </summary>
        /// <param name="scoreJson">A string representing the user's score in JSON format. 
        /// The expected format is {"value":0.12345}.</param>
        /// <remarks>
        /// The score parameter should be a valid JSON string containing a numeric value.
        /// The value should be in the "value" property of the JSON object.
        /// Example: {"value": 0.12345}
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when the provided JSON format is invalid or missing the "value" property.</exception>
        void SetUserScore(string scoreJson);

        /// <summary>
        /// This method is called by <see cref="IMediaboxServer"/>, when content should be unloaded before Unity is put into standby.
        /// Revert the actions performed in <see cref="SetContentBundleFolder(string)"/> and <see cref="SetSaveDataFolder(string)"/>.
        /// When unloading is complete, <see cref="IMediaboxServer.OnUnloadingSucceeded()"/> must be called.
        /// The contents of the save data folder will be backed up for the next launch of the same game content.
        /// </summary>
        void UnloadGameContent();

        /// <summary>
        /// This method is called by <see cref="IMediaboxServer"/>, when save data should be written.
        /// This might either occur shortly before the game is unloaded, or when
        /// the app is sent to background and might be killed by the system.
        /// You can rely on the path being the same as passed by <see cref="IMediaboxClient.SetSaveDataFolder(string)"/>.
        /// When saving is done, <see cref="IMediaboxServer.OnSaveDataWritten()"/> must be called.
        /// </summary>
        /// <param name="path">The path to the folder that's been provided for you to Read and Write Data to.</param>
        void WriteSaveData(string path);

        void PauseApplication();

        void UnpauseApplication();

        void CreateScreenshot();
    }
}