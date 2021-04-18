namespace Mediabox.API {
    // Outgoing API methods for communication with Mediabox 
    public interface IMediaboxServer {
        /// <summary>
        /// Initialize communication with Mediabox API.
        /// The parameter is the name of the Game Object receiving incoming calls from Mediabox.
        /// Call this method only once.
        /// </summary>
        /// <param name="apiGameObjectName"></param>
        void InitializeApi(string apiGameObjectName);
        /// <summary>
        /// Report to Mediabox that assets and save data were loaded successfully and the control should be handed over to Unity.
        /// This method should be called as a reply to the methods SetContentBundleFolder() and SetSaveDataFolder()
        /// </summary>
        void OnLoadingSucceeded();
        /// <summary>
        /// Report to Mediabox that loading failed and an error message should be displayed.
        /// This method should be called as a reply to the methods SetContentBundleFolder() and SetSaveDataFolder().
        /// </summary>
        void OnLoadingFailed();
        /// <summary>
        /// Report to Mediabox that unloading succeeded and Unity can be send to standby.
        /// This method should be called as a reply to UnloadGameContent().
        /// </summary>
        void OnUnloadingSucceeded();
        /// <summary>
        /// Report to Mediabox that save data writing has finished.
        /// This method should be called as a reply to WriteSaveData().
        /// </summary>
        void OnSaveDataWritten();
        /// <summary>
        /// Report to Mediabox that creating a screenshot has finished.
        /// This method should be called as a reply to CreateScreenshot().
        /// The path to the screenshot should be passed as a parameter.
        /// </summary>
        void OnCreateScreenshotSucceeded(string path);
        /// <summary>
        /// Report to Mediabox that creating a screenshot has finished.
        /// </summary>
        void OnCreateScreenshotFailed();
        /// <summary>
        /// Report to Mediabox that game exit has been requested.
        /// </summary>
        void OnGameExitRequested();
    }
}