using System;
using UnityEngine;

namespace Mediabox.API {
	public class AndroidMediaboxServer : IMediaboxServer, IDisposable {
		readonly AndroidJavaObject androidNativeServer;

		static AndroidJavaObject CreateBridgeObject() {
			return new AndroidJavaObject("eu.wonderz.unity.NativeHelper");
		}

		public AndroidMediaboxServer() {
			this.androidNativeServer = CreateBridgeObject();
		}
		
		public void InitializeApi(string apiGameObjectName) {
			this.androidNativeServer.CallStatic(nameof(InitializeApi).LowerCaseFirst(), apiGameObjectName);
		}

		public void OnLoadingSucceeded() {
			this.androidNativeServer.CallStatic(nameof(OnLoadingSucceeded).LowerCaseFirst());
		}

		public void OnLoadingFailed() {
			this.androidNativeServer.CallStatic(nameof(OnLoadingFailed).LowerCaseFirst());
		}

		public void OnUnloadingSucceeded() {
			this.androidNativeServer.CallStatic(nameof(OnUnloadingSucceeded).LowerCaseFirst());
		}

		public void OnUnloadingFailed() {
			this.androidNativeServer.CallStatic(nameof(OnUnloadingFailed).LowerCaseFirst());
		}

		public void OnSaveDataWritten() {
			this.androidNativeServer.CallStatic(nameof(OnSaveDataWritten).LowerCaseFirst());
		}

		public void OnCreateScreenshotSucceeded(string path) {
			this.androidNativeServer.CallStatic(nameof(OnCreateScreenshotSucceeded).LowerCaseFirst(), path);
		}

		public void OnCreateScreenshotFailed() {
			this.androidNativeServer.CallStatic(nameof(OnCreateScreenshotFailed).LowerCaseFirst());
		}

		public void OnGameExitRequested() {
			this.androidNativeServer.Call(nameof(OnGameExitRequested).LowerCaseFirst());
		}

		void ReleaseUnmanagedResources() {
			this.androidNativeServer.Dispose();
		}

		void Dispose(bool disposing) {
			ReleaseUnmanagedResources();
			if (disposing) {
				this.androidNativeServer?.Dispose();
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~AndroidMediaboxServer() {
			Dispose(false);
		}
	}
}