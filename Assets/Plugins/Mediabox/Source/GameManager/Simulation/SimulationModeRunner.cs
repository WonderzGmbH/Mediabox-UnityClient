using System;
using Mediabox.GameKit.GameManager;
using UnityEditor;

namespace Mediabox.GameManager.Simulation {
	public class SimulationModeRunner {
		readonly IPrefs prefs;
		readonly Func<ISimulationNativeAPI> createEditorNativeAPI;

		const string autoSimulatePrefKey = "Mediabox.GameManager.Editor.AutoSimulate";
		const string contentBundleFolderPrefKey = "Mediabox.GameManager.Editor.ContentBundleFolder";
		public ISimulationNativeAPI SimulationModeNativeApi { get; private set; }
		string bundleName;
		public string BundleName { 
			get => this.bundleName;
			set {
				if (value == this.bundleName) 
					return;
				this.bundleName = value;
				EditorPrefs.SetString(contentBundleFolderPrefKey, value);
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
				EditorPrefs.SetBool(autoSimulatePrefKey, value);
			}
		}

		public bool IsInSimulationMode => this.SimulationModeNativeApi != null;
		public SimulationModeRunner(IPrefs prefs, Func<ISimulationNativeAPI> createEditorNativeAPI) {
			this.prefs = prefs;
			this.createEditorNativeAPI = createEditorNativeAPI;
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
			if (!EditorApplication.isPlaying || this.isQuitting)
				return;
			if (!this.IsInSimulationMode) {
				StartSimulationMode();
			} else {
				this.SimulationModeNativeApi.AutoSimulate(this.BundleName);
			}
		}
		
		public void StartSimulationMode() {
			this.SimulationModeNativeApi = this.createEditorNativeAPI();
			UnityEngine.Object.FindObjectOfType<GameManagerBase>().SetNativeApi(this.SimulationModeNativeApi);
		}

		

		public void StopSimulationMode() {
			if (this.SimulationModeNativeApi == null)
				return;
			this.SimulationModeNativeApi.StopSimulation();
			if(this.SimulationModeNativeApi is IDisposable disposable)
				disposable.Dispose();
			this.SimulationModeNativeApi = null;
		}
	}
}