using System.Runtime.InteropServices;

namespace Mediabox.API {
    public class IosMediaboxServer : IMediaboxServer {
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
        
        void IMediaboxServer.InitializeApi(string apiGameObjectName) {
#if UNITY_IOS
            InitializeApi(apiGameObjectName);
#endif
        }

        void IMediaboxServer.OnLoadingSucceeded() {
#if UNITY_IOS
            OnLoadingSucceeded();
#endif
        }

        void IMediaboxServer.OnLoadingFailed() {
#if UNITY_IOS
            OnLoadingFailed();
#endif
        }

        void IMediaboxServer.OnUnloadingSucceeded() {
#if UNITY_IOS
            OnUnloadingSucceeded();
#endif
        }

        void IMediaboxServer.OnSaveDataWritten() {
#if UNITY_IOS
            OnSaveDataWritten();
#endif
        }

        void IMediaboxServer.OnCreateScreenshotSucceeded(string path) {
#if UNITY_IOS
            OnCreateScreenshotSucceeded(path);
#endif
        }

        void IMediaboxServer.OnCreateScreenshotFailed() {
#if UNITY_IOS
            OnCreateScreenshotFailed();
#endif
        }
        
        void IMediaboxServer.OnGameExitRequested() {
#if UNITY_IOS
            OnGameExitRequested();
#endif
        }
    }
}