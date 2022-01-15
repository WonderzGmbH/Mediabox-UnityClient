using System;
using System.IO;
using Mediabox.API;
using UnityEngine;


namespace Mediabox.GameManager.Simulation {
	/// <summary>
	/// A base class for simulating Mediabox' Native API.
	/// This can be implemented for simulating WonderZ' Mediabox Framework.
	/// Handles all communication with the Unity SDK to ensure that your Games run,
	/// even without a fully integrated build.
	/// </summary>
	public abstract class SimulationMediaboxServer : ISimulationMediaboxServer {
		enum State {
			WaitingForInitialize,
			Startable,
			Starting,
			Stoppable,
			Stopping,
			Stopped
		}

		protected string BundleName { get; private set; }
		protected readonly IDialog dialog;
		protected string ContentBundleFolder => Path.Combine(this.GameDefinitionDirectoryPath, this.BundleName);
		protected abstract string GameDefinitionDirectoryPath { get; }
		protected abstract string[] GetAllAvailableGameDefinitions();

		protected virtual void PrepareContentBundle() {
		}

		protected abstract void StopApplication();
		protected abstract void HandleLoadingFailed();
		protected abstract void HandleScreenshotUserChoice(string path, ScreenshotUserChoice choice);
		protected abstract void ValueTextField(string label, ref string value);

		string apiGameObjectName;
		string locale = "DE";
		string saveDataFolder = "./save";
		bool waitingForLoadingCallback;
		bool waitingForSaveDataCallback;
		bool waitingForUnloadCallback;
		State state;
		string[] _cachedAllAvailableGameDefinitions;

		string SaveDataDirectoryPath => Path.Combine(Application.persistentDataPath, this.saveDataFolder);

		protected SimulationMediaboxServer(string bundleName, IDialog dialog) {
			this.BundleName = bundleName;
			this.dialog = dialog;
		}

		#region ISimulationMediaboxServer

		string[] ISimulationMediaboxServer.AllAvailableGameDefinitions => this._cachedAllAvailableGameDefinitions ?? (this._cachedAllAvailableGameDefinitions = GetAllAvailableGameDefinitions());

		void ISimulationMediaboxServer.StopSimulation() {
			if (this.state != State.Stoppable)
				return;
			Stop();
		}

		void ISimulationMediaboxServer.AutoSimulate(string bundleName) {
			if (this.state == State.Startable) {
				Start(bundleName);
			} else if (this.state == State.Stoppable && this.BundleName != bundleName) {
				Stop();
			}
		}

		void ISimulationMediaboxServer.OnGUI(string bundleName) {
			this.dialog.Update();
			GUILayout.Label("----------");
			GUILayout.Label("Simulating...");
			ValueTextField("Locale", ref this.locale);
			ValueTextField("SaveDataFolder", ref this.saveDataFolder);
			GUILayout.Label($"State: {this.state}");
			DrawContextButtons(bundleName);
		}

		#endregion ISimulationMediaboxServer

		#region INativeAPI

		void IMediaboxServer.InitializeApi(string apiGameObjectName) {
			this.apiGameObjectName = apiGameObjectName;
			this.state = State.Startable;
		}

		void IMediaboxServer.OnCreateScreenshotSucceeded(string path) {
			this.dialog.Show("Screenshot creation succeeded", $"Screenshot can be found at '{path}'.", HandleUserChoice, Enum.GetNames(typeof(ScreenshotUserChoice)));

			void HandleUserChoice(string choice) {
				HandleScreenshotUserChoice(path, (ScreenshotUserChoice) Enum.Parse(typeof(ScreenshotUserChoice), choice));
			}
		}

		void IMediaboxServer.OnLoadingSucceeded() {
			this.state = State.Stoppable;
			this.waitingForLoadingCallback = false;
		}

		void IMediaboxServer.OnCreateScreenshotFailed() {
			this.dialog.Show("Screenshot creation failed", "An unknown error occured", null, "OK");
		}

		void IMediaboxServer.OnGameExitRequested() {
			StopApplication();
		}

		void IMediaboxServer.OnLoadingFailed() {
			this.state = State.Stopped;
			this.waitingForLoadingCallback = false;
			HandleLoadingFailed();
		}

		void IMediaboxServer.OnUnloadingSucceeded() {
			this.waitingForUnloadCallback = false;
			SwitchStateIfDone();
		}

		void IMediaboxServer.OnSaveDataWritten() {
			this.waitingForSaveDataCallback = false;
			SwitchStateIfDone();
		}

		#endregion INativeAPI

		void SendEvent(string name, string arg) {
			Debug.Log($"[EditorNativeAPI] Sending Event '{name}' with argument '{arg}'");
			var gameObject = GameObject.Find(this.apiGameObjectName);
			gameObject.SendMessage(name, arg);
		}

		void Start(string bundleName) {
			this.BundleName = bundleName;
			try {
				PrepareContentBundle();
			} catch (Exception e) {
				Debug.LogException(e);
				return;
			}

			this.state = State.Starting;
			this.waitingForLoadingCallback = true;
			EnsureSaveDataDirectoryExists();
			SendEvent(nameof(IMediaboxClient.SetContentLanguage), this.locale);
			SendEvent(nameof(IMediaboxClient.SetSaveDataFolder), this.SaveDataDirectoryPath);
			SendEvent(nameof(IMediaboxClient.SetContentBundleFolder), this.ContentBundleFolder);
		}

		void EnsureSaveDataDirectoryExists() {
			if (!Directory.Exists(this.SaveDataDirectoryPath)) {
				Directory.CreateDirectory(this.SaveDataDirectoryPath);
			}
		}


		void Stop() {
			this.state = State.Stopping;
			this.waitingForSaveDataCallback = true;
			this.waitingForUnloadCallback = true;
			SendEvent(nameof(IMediaboxClient.WriteSaveData), this.SaveDataDirectoryPath);
			SendEvent(nameof(IMediaboxClient.UnloadGameContent), null);
		}

		void CreateScreenshot() {
			SendEvent(nameof(IMediaboxClient.CreateScreenshot), null);
		}

		void PauseApplication() {
			SendEvent(nameof(IMediaboxClient.PauseApplication), null);
		}

		void UnpauseApplication() {
			SendEvent(nameof(IMediaboxClient.UnpauseApplication), null);
		}

		void SwitchStateIfDone() {
			if (this.waitingForUnloadCallback)
				return;
			if (this.waitingForSaveDataCallback)
				return;
			this.state = State.Startable;
		}

		void DrawContextButtons(string bundleName) {
			switch (this.state) {
				case State.Startable:
					if (GUILayout.Button("Start Game"))
						Start(bundleName);
					break;
				case State.Stoppable:
					DrawRunningGameContextButtons();
					break;
				case State.Starting:
					if (this.waitingForLoadingCallback)
						GUILayout.Label($"Waiting for {nameof(IMediaboxServer.OnLoadingSucceeded)}-Callback or {nameof(IMediaboxServer.OnLoadingFailed)}-Callback.");
					break;
				case State.Stopping:
					if (this.waitingForUnloadCallback)
						GUILayout.Label($"Waiting for {nameof(IMediaboxServer.OnUnloadingSucceeded)}-Callback.");
					if (this.waitingForSaveDataCallback)
						GUILayout.Label($"Waiting for {nameof(IMediaboxServer.OnSaveDataWritten)}-Callback.");
					break;
			}
		}

		void DrawRunningGameContextButtons() {
			if (GUILayout.Button("Stop Game"))
				Stop();
			if (GUILayout.Button("Create Screenshot"))
				CreateScreenshot();
			if (GUILayout.Button("Pause Game"))
				PauseApplication();
			if (GUILayout.Button("Unpause Game"))
				UnpauseApplication();
		}
	}
}