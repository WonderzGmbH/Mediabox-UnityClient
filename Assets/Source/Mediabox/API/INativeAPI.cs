namespace Mediabox.API {
    public interface INativeAPI {
        void InitializeApi(string apiGameObjectName);
        void OnLoadingSucceeded();
        void OnLoadingFailed();
        void OnUnloadingSucceeded();
        void OnSaveDataWritten();
        void OnCreateScreenshotSucceeded(string path);
        void OnCreateScreenshotFailed();
    }
}