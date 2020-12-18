namespace Mediabox.API {
    // Outgoing API methods for communication with Mediabox 
    public interface INativeAPI {
        // Initialize communication with Mediabox API.
        // The parameter is the name of the Game Object receiving incoming calls from Mediabox.
        // Call this method only once.
        void InitializeApi(string apiGameObjectName);
        // Report to Mediabox that assets and save data were loaded successfully and the control should be handed over to Unity.
        // This method should be called as a reply to the methods SetContentBundleFolder() and SetSaveDataFolder()
        void OnLoadingSucceeded();
        // Report to Mediabox that loading failed and an error message should be displayed.
        // This method should be called as a reply to the methods SetContentBundleFolder() and SetSaveDataFolder().
        void OnLoadingFailed();
        // Report to Mediabox that unloading succeeded and Unity can be send to standby.
        // This method should be called as a reply to UnloadGameContent().
        void OnUnloadingSucceeded();
        // Report to Mediabox that save data writing has finished.
        // This method should be called as a reply to WriteSaveData().
        void OnSaveDataWritten();
        // Report to Mediabox that creating a screenshot has finished.
        // This method should be called as a reply to CreateScreenshot().
        // The path to the screenshot should be passed as a parameter.
        void OnCreateScreenshotSucceeded(string path);
        // Report to Mediabox that creating a screenshot has finished.
        void OnCreateScreenshotFailed();
        // Report to Mediabox that game exit has been requested.
        void OnGameExitRequested();
    }
}