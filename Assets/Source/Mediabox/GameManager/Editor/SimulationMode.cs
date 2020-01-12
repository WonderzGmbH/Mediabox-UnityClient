using Mediabox.API;
using Mediabox.GameKit.GameManager;
using UnityEditor;

namespace Mediabox.GameManager.Editor {
	[InitializeOnLoad]
	public static class SimulationMode {

		const string autoSimulateEditorPrefKey = "Mediabox.GameManager.Editor.AutoSimulate";
		const string contentBundleFolderEditorPrefKey = "Mediabox.GameManager.Editor.ContentBundleFolder";
		public static EditorNativeAPI SimulationModeNativeApi { get; private set; }
		static string _contentBundleFolder;
		public static string ContentBundleFolder { 
			get => _contentBundleFolder;
			set {
				if (value == _contentBundleFolder) 
					return;
				_contentBundleFolder = value;
				EditorPrefs.SetString(contentBundleFolderEditorPrefKey, value);
			} 
		}

		static bool _autoSimulate;

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
			_contentBundleFolder = EditorPrefs.GetString(contentBundleFolderEditorPrefKey, null);
			EditorApplication.update += Update;
		}

		static void Update() {
			LoadDefaultSceneOnPlayMode.enabled = AutoSimulate;
			UpdateAutoSimulationMode();
		}

		static void UpdateAutoSimulationMode() {
			if (!AutoSimulate)
				return;
			if (!EditorApplication.isPlaying)
				return;
			if (!IsInSimulationMode) {
				StartSimulationMode();
			} else {
				SimulationModeNativeApi.AutoSimulate(ContentBundleFolder);
			}
		}
		
		public static void StartSimulationMode() {
			SimulationModeNativeApi = new EditorNativeAPI(ContentBundleFolder);
			UnityEngine.Object.FindObjectOfType<GameManagerBase>().SetNativeApi(SimulationModeNativeApi);
		}

		public static void StopSimulationMode() {
			SimulationModeNativeApi = null;
		}
	}
}