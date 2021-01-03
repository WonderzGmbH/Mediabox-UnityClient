using Mediabox.GameKit.Bundles;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build.Plugins {
	public class SimulationPlugin : IGameDefinitionManagerPlugin {
		readonly GameDefinitionManagementPlugin managementPlugin;
		readonly SettingsPlugin settingsPlugin;

		public SimulationPlugin(GameDefinitionManagementPlugin managementPlugin, SettingsPlugin settingsPlugin) {
			this.managementPlugin = managementPlugin;
			this.settingsPlugin = settingsPlugin;
		}

		public void Update() {
			SimulationMode.BundleName = PathUtility.GetRelativePath(this.managementPlugin.SelectedDirectory, this.settingsPlugin.settings.gameDefinitionDirectoryPath) ;
			if (!Application.isPlaying) {
				SimulationMode.StopSimulationMode();
			}
		}

		public bool Render() {
			if (!SimulationMode.IsInSimulationMode) {
				BundleManager.UseEditorBundles = !EditorGUILayout.Toggle("Use Built Bundles", !BundleManager.UseEditorBundles);
				EditorGUILayout.HelpBox("Using Built Bundles allows you to test your built Bundles in Simulation Mode. This makes the simulation more device-like. Make sure to update your builds regularly, though!", MessageType.Info);
			}

			EditorGUI.BeginChangeCheck();
			SimulationMode.AutoSimulate = EditorGUILayout.Toggle("Auto-Simulate", SimulationMode.AutoSimulate);
			if (!Application.isPlaying) {
				DrawStartPlayMode();
				DrawDummyStartSimulation();
			} else {
				DrawStopPlayMode();
				if (!SimulationMode.IsInSimulationMode) {
					DrawStartSimulationMode();
				} else {
					DrawSimulationMode();
				}
			}
			return true;
		}
		
		public string Title => "Simulation";
		public bool ToggleableWithTitleLabel => true;

		void DrawSimulationMode() {
			SimulationMode.SimulationModeNativeApi.OnGUI(SimulationMode.BundleName);
		}

		void DrawStartSimulationMode() {
			if (GUILayout.Button("Start Simulation")) {
				SimulationMode.StartSimulationMode();
			}
		}

		void DrawStartPlayMode() {
			EditorGUILayout.HelpBox("Start play mode to enable Simulation Mode.", MessageType.Info);
			if (GUILayout.Button("Start Play Mode")) {
				EditorApplication.isPlaying = true;
			}
		}

		static void DrawDummyStartSimulation() {
			GUI.enabled = false;
			GUILayout.Button("Start Simulation");
			GUI.enabled = true;
		}

		static void DrawStopPlayMode() {
			if (GUILayout.Button("Stop Play Mode")) {
				EditorApplication.isPlaying = false;
			}
		}
	}
}