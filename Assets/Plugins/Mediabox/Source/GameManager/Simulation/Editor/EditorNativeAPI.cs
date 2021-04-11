using System;
using System.IO;
using Mediabox.API;
using Mediabox.GameKit.GameDefinition;
using UnityEngine;
using UnityEditor;

namespace Mediabox.GameManager.Simulation.Editor {
	public class EditorNativeAPI : ISimulationNativeAPI {
		protected string BundleName { get; private set; }
		protected readonly GameDefinitionSettings settings;
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

		public EditorNativeAPI(string bundleName, GameDefinitionSettings settings) {
			this.BundleName = bundleName;
			this.settings = settings;
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

		void Start(string bundleName) {
			this.BundleName = bundleName;
			try {
				PrepareContentBundle();
			} catch (Exception e) {
				Debug.LogException(e);
				OnLoadingFailed();
				return;
			}
			this.state = State.Starting;
			this.waitingForLoadingCallback = true;
			SendEvent(nameof(IMediaboxCallbacks.SetContentLanguage), this.locale);
			SendEvent(nameof(IMediaboxCallbacks.SetSaveDataFolder), this.saveDataFolder);
			SendEvent(nameof(IMediaboxCallbacks.SetContentBundleFolder), this.ContentBundleFolder);
		}

		protected virtual string ContentBundleFolder => Path.Combine(this.settings.gameDefinitionDirectoryPath, this.BundleName);

		protected virtual void PrepareContentBundle() { }

		void Stop() {
			this.state = State.Stopping;
			this.waitingForSaveDataCallback = true;
			this.waitingForUnloadCallback = true;
			SendEvent(nameof(IMediaboxCallbacks.WriteSaveData), this.saveDataFolder);
			SendEvent(nameof(IMediaboxCallbacks.UnloadGameContent), null);
		}

		void CreateScreenshot() {
			SendEvent(nameof(IMediaboxCallbacks.CreateScreenshot), null);
		}
		
		void PauseApplication() {
			SendEvent(nameof(IMediaboxCallbacks.PauseApplication), null);
		}
		
		void UnpauseApplication() {
			SendEvent(nameof(IMediaboxCallbacks.UnpauseApplication), null);
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

		public void OnCreateScreenshotSucceeded(string path) {
			var dialogResult = EditorUtility.DisplayDialogComplex("Screenshot creation succeeded", $"Screenshot can be found at '{path}'.", "Open", "Delete", "Continue");
			switch (dialogResult) {
				case 0:
					EditorUtility.RevealInFinder(path);
					break;
				case 1:
					File.Delete(path);
					break;
				case 2:
					break;
				default:
					throw new System.ArgumentOutOfRangeException(nameof(dialogResult), dialogResult, "Value not expected.");
			}
		}

		public void OnCreateScreenshotFailed() {
			EditorUtility.DisplayDialog("Screenshot creation failed", "An unknown error occured", "OK");
		}

		public void OnGameExitRequested() {
			EditorApplication.ExitPlaymode();
		}

		void SwitchStateIfDone() {
			if (this.waitingForUnloadCallback)
				return;
			if (this.waitingForSaveDataCallback)
				return;
			this.state = State.Startable;
		}

		public void StopSimulation() {
			if (this.state != State.Stoppable)
				return;
			Stop();
		}
		
		public void AutoSimulate(string bundleName) {
			if (this.state == State.Startable) {
				Start(bundleName);
			} else if (this.state == State.Stoppable && this.BundleName != bundleName) {
				Stop();
			}
		}
		
		public void OnGUI(string bundleName) {
			GUILayout.Label("Simulating...");
			this.locale = EditorGUILayout.TextField("Locale", this.locale);
			this.saveDataFolder = EditorGUILayout.TextField("SaveDataFolder", this.saveDataFolder);
			GUILayout.Label($"State: {this.state}");
			DrawContextButtons(bundleName);
		}

		void DrawContextButtons(string contentBundleFolder) {
			switch (this.state) {
				case State.Startable:
					if (GUILayout.Button("Start Game"))
						Start(contentBundleFolder);
					break;
				case State.Stoppable:
					if (GUILayout.Button("Stop Game"))
						Stop();
					if (GUILayout.Button("Create Screenshot"))
						CreateScreenshot();
					if (GUILayout.Button("Pause Game"))
						PauseApplication();
					if (GUILayout.Button("Unpause Game"))
						UnpauseApplication();
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

