namespace Mediabox.API {
    // Incoming API methods for communication with Mediabox
    public interface IMediaboxCallbacks
    {
        // This method is called by Mediabox to set the language selected by the user for content.
        // The method is called after Initialization, before save data and content bundle folder are set.
        void SetContentLanguage(string locale);

        // This method is called by Mediabox, when save data should be loaded.
        // On first launch, this folder might be empty.
        // This method is always called before SetContentBundleFolder().
        // When content loading AND save game loading is complete,
        // either NativeAPI.OnAssetLoadingSucceeded() or NativeAPI.OnAssetLoadingFailed() must be called.
        void SetSaveDataFolder(string path);

        // This method is called by Mediabox, when assets were downloaded & extracted to the local file system.
        // You cannot store/modify files permanently here, the folder is located in the temporary folder.
        // This method is always called after SetSaveDataFolder().
        // When content loading AND save game loading is complete,
        // either NativeAPI.OnAssetLoadingSucceeded() or NativeAPI.OnAssetLoadingFailed() must be called.
        void SetContentBundleFolder(string path);

        // This method is called by Mediabox, when content should be unloaded before Unity is put into standby.
        // Revert the actions performed in SetContentBundleFolder and SetSaveDataFolder.
        // When unloading is complete, NativeAPI.OnUnloadingSucceeded() must be called.
        // The contents of the save data folder will be backed up for the next launch of the same game content.
        void UnloadGameContent();

        // This method is called by Mediabox, when save data should be written to the folder previously
        // passed by SetSaveDataFolder(). This might either occur shortly before the game is unloaded, or when
        // the app is sent to background and might be killed by the system.
        // You can rely on path being the same as passed by SetSaveDataFolder.
        // When saving is done, NativeAPI.OnSaveDataWritten() must be called.
        void WriteSave(string path);
    }
}