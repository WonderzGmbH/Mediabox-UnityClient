using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build.Plugins {
	public class SimulationPlugin : IGameDefinitionManagerPlugin {
		readonly GameDefinitionManagementPlugin managementPlugin;

		public SimulationPlugin(GameDefinitionManagementPlugin managementPlugin) {
			this.managementPlugin = managementPlugin;
		}

		public void Update() {
			SimulationMode.ContentBundleFolder = this.managementPlugin.SelectedDirectory;
			if (!Application.isPlaying) {
				SimulationMode.StopSimulationMode();
			}
		}

		public bool Render() {
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
			SimulationMode.SimulationModeNativeApi.OnGUI(SimulationMode.ContentBundleFolder);
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