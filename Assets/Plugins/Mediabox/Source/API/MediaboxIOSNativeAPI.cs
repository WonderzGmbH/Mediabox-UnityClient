using System.Runtime.InteropServices;

namespace Mediabox.API {
    public class MediaboxIOSNativeAPI : INativeAPI {
#if UNITY_IOS
        [DllImport("__Internal")] static extern void InitializeApi(string apiGameObjectName);
        [DllImport("__Internal")] static extern void OnLoadingSucceeded();
        [DllImport("__Internal")] static extern void OnLoadingFailed();
        [DllImport("__Internal")] static extern void OnUnloadingSucceeded();
        [DllImport("__Internal")] static extern void OnSaveDataWritten();
        [DllImport("__Internal")] static extern void OnCreateScreenshotSucceeded(string path);
        [DllImport("__Internal")] static extern void OnCreateScreenshotFailed();
        [DllImport("__Internal")] static extern void OnGameExitRequested();
#endif
        
        void INativeAPI.InitializeApi(string apiGameObjectName) {
#if UNITY_IOS
            InitializeApi(apiGameObjectName);
#endif
        }

        void INativeAPI.OnLoadingSucceeded() {
#if UNITY_IOS
            OnLoadingSucceeded();
#endif
        }

        void INativeAPI.OnLoadingFailed() {
#if UNITY_IOS
            OnLoadingFailed();
#endif
        }

        void INativeAPI.OnUnloadingSucceeded() {
#if UNITY_IOS
            OnUnloadingSucceeded();
#endif
        }

        void INativeAPI.OnSaveDataWritten() {
#if UNITY_IOS
            OnSaveDataWritten();
#endif
        }

        void INativeAPI.OnCreateScreenshotSucceeded(string path) {
#if UNITY_IOS
            OnCreateScreenshotSucceeded(path);
#endif
        }

        void INativeAPI.OnCreateScreenshotFailed() {
#if UNITY_IOS
            OnCreateScreenshotFailed();
#endif
        }
        
        void INativeAPI.OnGameExitRequested() {
#if UNITY_IOS
            OnGameExitRequested();
#endif
        }
    }
}