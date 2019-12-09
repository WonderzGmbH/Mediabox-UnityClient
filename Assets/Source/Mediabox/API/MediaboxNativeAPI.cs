namespace Mediabox.API {
    public class MediaboxNativeAPI : INativeAPI {
        public void InitializeApi(string apiGameObjectName) {
            NativeAPI.InitializeApi(apiGameObjectName);
        }

        public void OnLoadingSucceeded() {
            NativeAPI.OnLoadingSucceeded();
        }

        public void OnLoadingFailed() {
            NativeAPI.OnLoadingFailed();
        }

        public void OnUnloadingSucceeded() {
            NativeAPI.OnUnloadingSucceeded();
        }

        public void OnSaveDataWritten() {
            NativeAPI.OnSaveDataWritten();
        }
    }
    
}