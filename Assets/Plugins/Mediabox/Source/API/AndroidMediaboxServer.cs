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
		
		void IMediaboxServer.InitializeApi(string apiGameObjectName) {
			this.androidNativeServer.CallStatic(nameof(IMediaboxServer.InitializeApi).LowerCaseFirst(), apiGameObjectName);
		}

		void IMediaboxServer.OnLoadingSucceeded() {
			this.androidNativeServer.CallStatic(nameof(IMediaboxServer.OnLoadingSucceeded).LowerCaseFirst());
		}

		void IMediaboxServer.OnLoadingFailed() {
			this.androidNativeServer.CallStatic(nameof(IMediaboxServer.OnLoadingFailed).LowerCaseFirst());
		}

		void IMediaboxServer.OnUnloadingSucceeded() {
			this.androidNativeServer.CallStatic(nameof(IMediaboxServer.OnUnloadingSucceeded).LowerCaseFirst());
		}

		void IMediaboxServer.OnSaveDataWritten() {
			this.androidNativeServer.CallStatic(nameof(IMediaboxServer.OnSaveDataWritten).LowerCaseFirst());
		}

		void IMediaboxServer.OnCreateScreenshotSucceeded(string path) {
			this.androidNativeServer.CallStatic(nameof(IMediaboxServer.OnCreateScreenshotSucceeded).LowerCaseFirst(), path);
		}

		void IMediaboxServer.OnUserScoreChanged(float newValue)
		{
			this.androidNativeServer.CallStatic(nameof(IMediaboxServer.OnUserScoreChanged).LowerCaseFirst(), newValue);
		}

		void IMediaboxServer.OnCreateScreenshotFailed() {
			this.androidNativeServer.CallStatic(nameof(IMediaboxServer.OnCreateScreenshotFailed).LowerCaseFirst());
		}

		void IMediaboxServer.OnGameExitRequested() {
			this.androidNativeServer.Call(nameof(IMediaboxServer.OnGameExitRequested).LowerCaseFirst());
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