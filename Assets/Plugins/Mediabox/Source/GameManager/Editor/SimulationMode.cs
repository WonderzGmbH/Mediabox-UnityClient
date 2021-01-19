using System;
using Mediabox.GameKit.Bundles;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameKit.GameManager;
using Mediabox.GameManager.Editor.Build;
using UnityEditor;

namespace Mediabox.GameManager.Editor {
	[InitializeOnLoad]
	public static class SimulationMode {

		const string autoSimulateEditorPrefKey = "Mediabox.GameManager.Editor.AutoSimulate";
		const string contentBundleFolderEditorPrefKey = "Mediabox.GameManager.Editor.ContentBundleFolder";
		public static EditorNativeAPI SimulationModeNativeApi { get; private set; }
		static string bundleName;
		public static string BundleName { 
			get => bundleName;
			set {
				if (value == bundleName) 
					return;
				bundleName = value;
				EditorPrefs.SetString(contentBundleFolderEditorPrefKey, value);
			} 
		}

		static bool _autoSimulate;
		static bool isQuitting;

		public static bool AutoSimulate {
			get => _autoSimulate;
			set {
				if (value == _autoSimulate)
					return;
				_autoSimulate = value;
				EditorPrefs.SetBool(autoSimulateEditorPrefKey, value);
			}
		}

		public static bool IsInSimulationMode => SimulationModeNativeApi != null;
		static SimulationMode() {
			_autoSimulate = EditorPrefs.GetBool(autoSimulateEditorPrefKey, true);
			bundleName = EditorPrefs.GetString(contentBundleFolderEditorPrefKey, null);
			EditorApplication.update += Update;
			EditorApplication.playModeStateChanged += OnplayModeStateChanged;
		}

		static void OnplayModeStateChanged(PlayModeStateChange change) {
			switch (change) {
				case PlayModeStateChange.ExitingPlayMode:
					StopSimulationMode();
					isQuitting = true;
					break;
				case PlayModeStateChange.EnteredEditMode:
					isQuitting = false;
					break;
			}
		}

		static void Update() {
			LoadDefaultSceneOnPlayMode.enabled = AutoSimulate;
			UpdateAutoSimulationMode();
		}

		static void UpdateAutoSimulationMode() {
			if (!AutoSimulate)
				return;
			if (!EditorApplication.isPlaying || isQuitting)
				return;
			if (!IsInSimulationMode) {
				StartSimulationMode();
			} else {
				SimulationModeNativeApi.AutoSimulate(BundleName);
			}
		}
		
		public static void StartSimulationMode() {
			SimulationModeNativeApi = CreateNativeAPI();
			UnityEngine.Object.FindObjectOfType<GameManagerBase>().SetNativeApi(SimulationModeNativeApi);
		}

		static EditorNativeAPI CreateNativeAPI() {
			var gameDefinitionSettings = AssetDatabase.LoadAssetAtPath<GameDefinitionSettings>(GameDefinitionSettings.SettingsPath);
			var gameDefinitionBuildSettings = AssetDatabase.LoadAssetAtPath<GameDefinitionBuildSettings>(GameDefinitionBuildSettings.SettingsPath);
			return BundleManager.UseEditorBundles ? new EditorNativeAPI(BundleName, gameDefinitionSettings) : new EditorBuildNativeAPI(BundleName, gameDefinitionSettings, gameDefinitionBuildSettings);
		}

		public static void StopSimulationMode() {
			if (SimulationModeNativeApi == null)
				return;
			SimulationModeNativeApi.StopSimulation();
			if(SimulationModeNativeApi is IDisposable disposable)
				disposable.Dispose();
			SimulationModeNativeApi = null;
		}
	}
}