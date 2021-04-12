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
	public abstract class SimulationNativeAPI : ISimulationNativeAPI {
		
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
		protected virtual void PrepareContentBundle() { }
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

		protected SimulationNativeAPI(string bundleName, IDialog dialog) {
			this.BundleName = bundleName;
			this.dialog = dialog;
		}

#region ISimulationNativeAPI
		string[] ISimulationNativeAPI.AllAvailableGameDefinitions => this._cachedAllAvailableGameDefinitions ?? (this._cachedAllAvailableGameDefinitions = GetAllAvailableGameDefinitions());
		void ISimulationNativeAPI.StopSimulation() {
			if (this.state != State.Stoppable)
				return;
			Stop();
		}

		void ISimulationNativeAPI.AutoSimulate(string bundleName) {
			if (this.state == State.Startable) {
				Start(bundleName);
			} else if (this.state == State.Stoppable && this.BundleName != bundleName) {
				Stop();
			}
		}

		void ISimulationNativeAPI.OnGUI(string bundleName) {
			this.dialog.Update();
			GUILayout.Label("----------");
			GUILayout.Label("Simulating...");
			ValueTextField("Locale", ref this.locale);
			ValueTextField("SaveDataFolder", ref this.saveDataFolder);
			GUILayout.Label($"State: {this.state}");
			DrawContextButtons(bundleName);
		}
#endregion ISimulationNativeAPI

#region INativeAPI

		void INativeAPI.InitializeApi(string apiGameObjectName) {
			this.apiGameObjectName = apiGameObjectName;
			this.state = State.Startable;
		}
		
		void INativeAPI.OnCreateScreenshotSucceeded(string path) {
			this.dialog.Show("Screenshot creation succeeded", $"Screenshot can be found at '{path}'.", HandleUserChoice, Enum.GetNames(typeof(ScreenshotUserChoice)));

			void HandleUserChoice(string choice) {
				HandleScreenshotUserChoice(path, (ScreenshotUserChoice) Enum.Parse(typeof(ScreenshotUserChoice), choice));
			}
		}

		void INativeAPI.OnLoadingSucceeded() {
			this.state = State.Stoppable;
			this.waitingForLoadingCallback = false;
		}

		void INativeAPI.OnCreateScreenshotFailed() {
			this.dialog.Show("Screenshot creation failed", "An unknown error occured", null, "OK");
		}

		void INativeAPI.OnGameExitRequested() {
			StopApplication();
		}
		
		void INativeAPI.OnLoadingFailed() {
			this.state = State.Stopped;
			this.waitingForLoadingCallback = false;
			HandleLoadingFailed();
		}

		void INativeAPI.OnUnloadingSucceeded() {
			this.waitingForUnloadCallback = false;
			SwitchStateIfDone();
		}

		void INativeAPI.OnSaveDataWritten() {
			this.waitingForSaveDataCallback = false;
			SwitchStateIfDone();
		}
#endregion INativeAPI

		void SendEvent(string name, string arg) {
			Debug.Log($"[EditorNativeAPI] Sending Event '{name} with argument '{arg}'");
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
			SendEvent(nameof(IMediaboxCallbacks.SetContentLanguage), this.locale);
			SendEvent(nameof(IMediaboxCallbacks.SetSaveDataFolder), this.SaveDataDirectoryPath);
			SendEvent(nameof(IMediaboxCallbacks.SetContentBundleFolder), this.ContentBundleFolder);
		}


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
						GUILayout.Label($"Waiting for {nameof(INativeAPI.OnLoadingSucceeded)}-Callback or {nameof(INativeAPI.OnLoadingFailed)}-Callback.");
					break;
				case State.Stopping:
					if (this.waitingForUnloadCallback)
						GUILayout.Label($"Waiting for {nameof(INativeAPI.OnUnloadingSucceeded)}-Callback.");
					if (this.waitingForSaveDataCallback)
						GUILayout.Label($"Waiting for {nameof(INativeAPI.OnSaveDataWritten)}-Callback.");
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