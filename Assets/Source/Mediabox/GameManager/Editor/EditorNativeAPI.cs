using Mediabox.API;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor {
	public class EditorNativeAPI : INativeAPI {
		string contentBundleFolder;
		string apiGameObjectName;
		string locale = "DE";
		string saveDataFolder = "./save";
		bool waitingForLoadingCallback;
		bool waitingForSaveDataCallback;
		bool waitingForUnloadCallback;

		enum State {
			WaitingForInitialize,
			Startable,
			Starting,
			Stoppable,
			Stopping,
			Stopped
		}

		State state;

		public EditorNativeAPI(string contentBundleFolder) {
			this.contentBundleFolder = contentBundleFolder;
		}
	
		void SendEvent(string name, string arg) {
			Debug.Log($"[EditorNativeAPI] Sending Event '{name} with argument '{arg}'" );
			var gameObject = GameObject.Find(this.apiGameObjectName);
			gameObject.SendMessage(name, arg);
		}
	
		public void InitializeApi(string apiGameObjectName) {
			this.apiGameObjectName = apiGameObjectName;
			this.state = State.Startable;
		}

		void Start() {
			this.state = State.Starting;
			this.waitingForLoadingCallback = true;
			SendEvent(nameof(IMediaboxCallbacks.SetContentLanguage), this.locale);
			SendEvent(nameof(IMediaboxCallbacks.SetSaveDataFolder), this.saveDataFolder);
			SendEvent(nameof(IMediaboxCallbacks.SetContentBundleFolder), this.contentBundleFolder);
		}

		void Stop() {
			this.state = State.Stopping;
			this.waitingForSaveDataCallback = true;
			this.waitingForUnloadCallback = true;
			SendEvent(nameof(IMediaboxCallbacks.WriteSaveData), this.saveDataFolder);
			SendEvent(nameof(IMediaboxCallbacks.UnloadGameContent), null);
		}

		public void OnLoadingSucceeded() {
			this.state = State.Stoppable;
			this.waitingForLoadingCallback = false;
		}

		public void OnLoadingFailed() {
			this.state = State.Stopped;
			this.waitingForLoadingCallback = false;
			EditorApplication.isPlaying = false;
		}
	
		public void OnUnloadingSucceeded() {
			this.waitingForUnloadCallback = false;
			SwitchStateIfDone();
		}

		public void OnSaveDataWritten() {
			this.waitingForSaveDataCallback = false;
			SwitchStateIfDone();
		}

		void SwitchStateIfDone() {
			if (this.waitingForUnloadCallback)
				return;
			if (this.waitingForSaveDataCallback)
				return;
			this.state = State.Startable;
		}

		public void OnGUI(bool autoSimulate, string contentBundleFolder) {
			GUILayout.Label("Simulating...");
			this.locale = EditorGUILayout.TextField("Locale", this.locale);
			this.saveDataFolder = EditorGUILayout.TextField("SaveDataFolder", this.saveDataFolder);
			GUILayout.Label($"State: {this.state}");
			if (autoSimulate) {
				if (this.state == State.Startable) {
					this.contentBundleFolder = contentBundleFolder;
					Start();
				} else if (this.state == State.Stoppable && this.contentBundleFolder != contentBundleFolder) {
					Stop();
				}
			} else {
				DrawContextButtons();
			}
			
		}

		void DrawContextButtons() {
			switch (this.state) {
				case State.Startable:
					if (GUILayout.Button("Start Game"))
						Start();
					break;
				case State.Stoppable:
					if (GUILayout.Button("Stop Game"))
						Stop();
					break;
				case State.Starting:
					if (this.waitingForLoadingCallback)
						GUILayout.Label($"Waiting for {nameof(OnLoadingSucceeded)}-Callback or {nameof(OnLoadingFailed)}-Callback.");
					break;
				case State.Stopping:
					if (this.waitingForUnloadCallback)
						GUILayout.Label($"Waiting for {nameof(OnUnloadingSucceeded)}-Callback.");
					if (this.waitingForSaveDataCallback)
						GUILayout.Label($"Waiting for {nameof(OnSaveDataWritten)}-Callback.");
					break;
			}
		}
	}
}