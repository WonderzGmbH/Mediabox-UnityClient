using System;
using UnityEngine;

namespace Mediabox.API {
	public class MediaboxAndroidNativeAPI : INativeAPI, IDisposable {
		readonly AndroidJavaObject nativeApi;

		static AndroidJavaObject CreateBridgeObject() {
			return new AndroidJavaObject("eu.wonderz.unity.NativeHelper");
		}

		public MediaboxAndroidNativeAPI() {
			this.nativeApi = CreateBridgeObject();
		}
		
		public void InitializeApi(string apiGameObjectName) {
			this.nativeApi.CallStatic(nameof(InitializeApi), apiGameObjectName);
		}

		public void OnLoadingSucceeded() {
			this.nativeApi.CallStatic(nameof(OnLoadingSucceeded).LowerCaseFirst());
		}

		public void OnLoadingFailed() {
			this.nativeApi.CallStatic(nameof(OnLoadingFailed).LowerCaseFirst());
		}

		public void OnUnloadingSucceeded() {
			this.nativeApi.CallStatic(nameof(OnUnloadingSucceeded).LowerCaseFirst());
		}

		public void OnUnloadingFailed() {
			this.nativeApi.CallStatic(nameof(OnUnloadingFailed).LowerCaseFirst());
		}

		public void OnSaveDataWritten() {
			this.nativeApi.CallStatic(nameof(OnSaveDataWritten).LowerCaseFirst());
		}

		public void OnCreateScreenshotSucceeded(string path) {
			this.nativeApi.CallStatic(nameof(OnCreateScreenshotSucceeded).LowerCaseFirst());
		}

		public void OnCreateScreenshotFailed() {
			this.nativeApi.CallStatic(nameof(OnCreateScreenshotFailed).LowerCaseFirst());
		}

		public void OnGameExitRequested() {
			this.nativeApi.Call(nameof(OnGameExitRequested).LowerCaseFirst());
		}

		void ReleaseUnmanagedResources() {
			this.nativeApi.Dispose();
		}

		void Dispose(bool disposing) {
			ReleaseUnmanagedResources();
			if (disposing) {
				this.nativeApi?.Dispose();
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~MediaboxAndroidNativeAPI() {
			Dispose(false);
		}
	}
}