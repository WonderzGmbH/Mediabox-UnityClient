using System.Runtime.InteropServices;

namespace Mediabox.API {
    // Outgoing API methods for communication with Mediabox 
    public static class NativeAPI
    {
        // Initialize communication with Mediabox API.
        // The parameter is the name of the Game Object receiving incoming calls from Mediabox.
        // Call this method only once.
        [DllImport("__Internal")] public static extern void InitializeApi(string apiGameObjectName);

        // Report to Mediabox that assets and save data were loaded successfully and the control should be handed over to Unity.
        // This method should be called as a reply to the methods SetContentBundleFolder() and SetSaveDataFolder()
        [DllImport("__Internal")] public static extern void OnLoadingSucceeded();

        // Report to Mediabox that loading failed and an error message should be displayed.
        // This method should be called as a reply to the methods SetContentBundleFolder() and SetSaveDataFolder().
        [DllImport("__Internal")] public static extern void OnLoadingFailed();

        // Report to Mediabox that unloading succeeded and Unity can be send to standby.
        // This method should be called as a reply to UnloadGameContent().
        [DllImport("__Internal")] public static extern void OnUnloadingSucceeded();

        // Report to Mediabox that save data writing has finished.
        // This method should be called as a reply to WriteSaveData().
        [DllImport("__Internal")] public static extern void OnSaveDataWritten();
    }
}