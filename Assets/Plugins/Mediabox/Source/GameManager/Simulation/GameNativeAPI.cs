using System;
using System.IO;
using System.Linq;
using Mediabox.API;
using UnityEngine;

namespace Mediabox.GameManager.Simulation {
	public class GameNativeAPI : ISimulationNativeAPI {
		protected string BundleName { get; private set; }
		readonly IDialog dialog;
		string apiGameObjectName;
		string locale = "DE";
		string saveDataFolder = "./save";
		bool waitingForLoadingCallback;
		bool waitingForSaveDataCallback;
		bool waitingForUnloadCallback;

		public string[] AvailableBundles {
			get;
			private set;
		}

		protected virtual string GameDefinitionDirectoryPath => Path.Combine(Application.streamingAssetsPath, "GameDefinitions");
		protected virtual string SaveDataDirectoryPath => Path.Combine(Application.persistentDataPath, this.saveDataFolder);
		
		enum State {
			WaitingForInitialize,
			Startable,
			Starting,
			Stoppable,
			Stopping,
			Stopped
		}

		State state;
		readonly GUIDialog guiDialog;

		string[] _cachedAllAvailableGameDefinitions;

		public string[] AllAvailableGameDefinitions {
			get { return this._cachedAllAvailableGameDefinitions ?? (this._cachedAllAvailableGameDefinitions = GetAllAvailableGameDefinitions()); }
		}
		
		protected virtual string[] GetAllAvailableGameDefinitions() {
			return new DirectoryInfo(GameDefinitionDirectoryPath).GetDirectories().Select(dir => dir.Name).ToArray();
		}
		
		public GameNativeAPI(string bundleName, IDialog dialog) {
			this.BundleName = bundleName;
			this.dialog = dialog;
			this.guiDialog = new GUIDialog();
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
			SendEvent(nameof(IMediaboxCallbacks.SetSaveDataFolder), this.SaveDataDirectoryPath);
			SendEvent(nameof(IMediaboxCallbacks.SetContentBundleFolder), this.ContentBundleFolder);
		}

		protected virtual string ContentBundleFolder => Path.Combine(this.GameDefinitionDirectoryPath, this.BundleName);

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

		protected virtual void StopApplication() {
			Application.Quit();
		}

		protected virtual void HandleLoadingFailed() {
			// no action required
		}

		public void OnLoadingFailed() {
			this.state = State.Stopped;
			this.waitingForLoadingCallback = false;
		}
	
		public void OnUnloadingSucceeded() {
			this.waitingForUnloadCallback = false;
			SwitchStateIfDone();
		}

		public void OnSaveDataWritten() {
			this.waitingForSaveDataCallback = false;
			SwitchStateIfDone();
		}

		protected enum ScreenshotUserChoice {
			Open,
			Delete,
			Continue
		}

		protected virtual void HandleScreenshotUserChoice(string path, ScreenshotUserChoice choice) {
			switch (choice) {
				case ScreenshotUserChoice.Open:
					Application.OpenURL($"file://{Path.GetFullPath(path)}");
					break;
				case ScreenshotUserChoice.Delete:
					File.Delete(path);
					break;
				case ScreenshotUserChoice.Continue:
					break;
				default:
					this.dialog.Show("Not Supported.", $"Option {choice} is not supported at this time.");
					break;
			}
		}

		public void OnCreateScreenshotSucceeded(string path) {
			this.dialog.Show("Screenshot creation succeeded", $"Screenshot can be found at '{path}'.", HandleUserChoice, Enum.GetNames(typeof(ScreenshotUserChoice)));

			void HandleUserChoice(string choice) {
				HandleScreenshotUserChoice(path, (ScreenshotUserChoice) Enum.Parse(typeof(ScreenshotUserChoice), choice));
			}
		}

		public void OnCreateScreenshotFailed() {
			this.dialog.Show("Screenshot creation failed", "An unknown error occured", null, "OK");
		}

		public void OnGameExitRequested() {
			StopApplication();
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

		protected virtual void ValueTextField(string label, ref string value) {
			GUILayout.BeginHorizontal();
			GUILayout.Label(label);
			value = GUILayout.TextField(value);
			GUILayout.EndHorizontal();
		}
		
		public void OnGUI(string bundleName) {
			this.dialog.Update();
			GUILayout.Label("----------");
			GUILayout.Label("Simulating...");
			ValueTextField("Locale", ref this.locale);
			ValueTextField("SaveDataFolder", ref this.saveDataFolder);
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

