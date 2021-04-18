using System;
using Mediabox.GameKit.GameManager;

namespace Mediabox.GameManager.Simulation {
	public class SimulationModeRunner {
		readonly IPrefs prefs;
		readonly Func<bool> isActive;
		readonly Func<ISimulationMediaboxServer> createSimulationMediaboxServer;

		const string autoSimulatePrefKey = "Mediabox.GameManager.Editor.AutoSimulate";
		const string contentBundleFolderPrefKey = "Mediabox.GameManager.Editor.ContentBundleFolder";
		public ISimulationMediaboxServer SimulationModeMediaboxServer { get; private set; }
		string bundleName;
		public string BundleName { 
			get => this.bundleName;
			set {
				if (value == this.bundleName) 
					return;
				this.bundleName = value;
				this.prefs.SetString(contentBundleFolderPrefKey, value);
			} 
		}

		bool _autoSimulate;
		bool isQuitting;

		public bool AutoSimulate {
			get => this._autoSimulate;
			set {
				if (value == this._autoSimulate)
					return;
				this._autoSimulate = value;
				this.prefs.SetBool(autoSimulatePrefKey, value);
			}
		}

		public bool IsInSimulationMode => this.SimulationModeMediaboxServer != null;
		public SimulationModeRunner(IPrefs prefs, Func<bool> isActive, Func<ISimulationMediaboxServer> createSimulationMediaboxServer) {
			this.prefs = prefs;
			this.isActive = isActive;
			this.createSimulationMediaboxServer = createSimulationMediaboxServer;
			this._autoSimulate = prefs.GetBool(autoSimulatePrefKey, true);
			this.bundleName = prefs.GetString(contentBundleFolderPrefKey, null);
		}

		public void Start() {
			this.isQuitting = false;
		}

		public void OnDestroy() {
			StopSimulationMode();
			this.isQuitting = true;
		}

		public void Update() {
			if (!this.AutoSimulate)
				return;
			if (!this.isActive() || this.isQuitting)
				return;
			if (!this.IsInSimulationMode) {
				StartSimulationMode();
			} else {
				this.SimulationModeMediaboxServer.AutoSimulate(this.BundleName);
			}
		}
		
		public void StartSimulationMode() {
			this.SimulationModeMediaboxServer = this.createSimulationMediaboxServer();
			UnityEngine.Object.FindObjectOfType<GameManagerBase>().SetNativeApi(this.SimulationModeMediaboxServer);
		}

		public void StopSimulationMode() {
			if (this.SimulationModeMediaboxServer == null)
				return;
			this.SimulationModeMediaboxServer.StopSimulation();
			if(this.SimulationModeMediaboxServer is IDisposable disposable)
				disposable.Dispose();
			this.SimulationModeMediaboxServer = null;
		}
	}
}