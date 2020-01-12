using System;
using System.IO;
using System.Linq;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameKit.GameManager;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor {
	/// <summary>
	/// Implement this class to add a typed Game Definition Manager Editor Window. for your project.
	/// Implement it like this:
	/// ATTENTION! Place this script within an Editor-Folder in your Unity-Project. Your project won't build if you don't.
	/// <code>
	/// public class GameDefinitionManager : GameDefinitionManagerBase<GameDefinitionManager, GameDefinition> {
	/// 	[MenuItem("MediaBox/Game Definition Manager")]
	/// 	static GameDefinitionManager OpenWindow() {
	/// 		return ShowWindow();
	/// 	}
	/// }
	/// </code>
	/// </summary>
	/// <typeparam name="TWindow">Your inherited class type.</typeparam>
	/// <typeparam name="TGameDefinition">The type of GameDefinition that you're using.</typeparam>
	public class GameDefinitionManagerBase<TWindow, TGameDefinition> : EditorWindow where TWindow : GameDefinitionManagerBase<TWindow, TGameDefinition> where TGameDefinition : class, new() {

		int selectedIndex;
		string newDefinitionName;

		public TGameDefinition gameDefinition;
		static TWindow window;
		GameDefinitionSettings settings;
		protected static TWindow ShowWindow()
		{
			window = GetWindow<TWindow>();
			window.titleContent = new GUIContent("Game Definition Manager");
			window.Show();
			return window;
		}

		void LoadOrCreateSettings() {
			this.settings = AssetDatabase.LoadAssetAtPath<GameDefinitionSettings>(GameDefinitionSettings.SettingsPath) ?? TryCreateSettings();
		}

		static GameDefinitionSettings TryCreateSettings() {
			var settingsDirectory = Path.GetDirectoryName(GameDefinitionSettings.SettingsPath);
			if (settingsDirectory == null)
				return null;
			if (!Directory.Exists(settingsDirectory))
				Directory.CreateDirectory(settingsDirectory);
			AssetDatabase.CreateAsset(CreateInstance<GameDefinitionSettings>(), GameDefinitionSettings.SettingsPath);
			return AssetDatabase.LoadAssetAtPath<GameDefinitionSettings>(GameDefinitionSettings.SettingsPath);
		}

		void OnGUI() {
			LoadOrCreateSettings();
			if (this.settings == null) {
				EditorGUILayout.HelpBox("Could not find or create GameDefinitionSettings. This might be caused by invalid SettingsPath in GameDefinitionSettings.cs-File.", MessageType.Error);
				return;
			}
			DrawSettingsArea();
			
			EnsureDirectory();
			DrawDirectoryArea();
			var directories = LoadGameDefinitions();
			directories = DrawCreateNew(directories);
			if (directories.Length == 0) {
				EditorGUILayout.HelpBox("Create a new GameDefinition to begin work.", MessageType.Info);
			} else {
				DrawEditorArea(directories);
				SimulationMode.ContentBundleFolder = directories[this.selectedIndex];
				DrawSimulationArea();
			}
			DrawBuildArea();
		}

		static void DrawBuildArea() {
			if (GUILayout.Button("Build GameDefinitions")) {
				EditorUtility.DisplayDialog(
                    "Not implemented yet 🤗",
                    "Please build the bundles via\n" +
                    "Window > Asset Bundle Browser > Build > Target iOS\n" +
                    "and copy the generated files to your GameDefinitions folder.",
                    "👍");
			}
		}

		string[] LoadGameDefinitions() {
			var directories = Directory.GetDirectories(this.settings.gameDefinitionDirectoryPath);
			return directories;
		}

		void DrawEditorArea(string[] directories) {
			ValidateSelectedGameDefinition(directories);
			directories = DrawSelector(directories);
			if (directories.Length == 0)
				return;
			LoadGameDefinition(directories);
			DrawGameDefinitionEditor(directories);

		}

		void DrawSimulationArea() {
			EditorGUI.BeginChangeCheck();
			SimulationMode.AutoSimulate = GUILayout.Toggle(SimulationMode.AutoSimulate, "Auto-Simulate");
			if (!Application.isPlaying) {
				SimulationMode.StopSimulationMode();
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
		}

		void DrawSimulationMode() {
			SimulationMode.SimulationModeNativeApi.OnGUI(SimulationMode.ContentBundleFolder);
		}

		void ValidateSelectedGameDefinition(string[] directories) {
			this.selectedIndex = Mathf.Clamp(this.selectedIndex, 0, directories.Length);
		}

		void DrawDirectoryArea() {
			if (GUILayout.Button("Open Directory")) {
				OpenInFileBrowser.Open(this.settings.gameDefinitionDirectoryPath);
			}
		}

		void EnsureDirectory() {
			if (!Directory.Exists(this.settings.gameDefinitionDirectoryPath)) {
				Directory.CreateDirectory(this.settings.gameDefinitionDirectoryPath);
			}
		}

		void DrawSettingsArea() {
			if (GUILayout.Button("Edit Settings")) {
				Selection.activeObject = this.settings;
			}
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

		void DrawGameDefinitionEditor(string[] directories) {
			ScriptableObject target = this;
			var so = new SerializedObject(target);
			so.ApplyModifiedProperties();
			var property = so.FindProperty("gameDefinition");
			EditorGUILayout.PropertyField(property, true);
			so.ApplyModifiedProperties();
			File.WriteAllText(Path.Combine(directories[this.selectedIndex], this.settings.gameDefinitionFileName), JsonUtility.ToJson(this.gameDefinition));
		}

		string declinedRepairPath;
		void LoadGameDefinition(string[] directories) {
			var filePath = Path.Combine(directories[this.selectedIndex], this.settings.gameDefinitionFileName);
			if (!File.Exists(filePath)) {
				if (this.declinedRepairPath != filePath && EditorUtility.DisplayDialog("GameDefinitionManager Error", $"Expected to find a file named '{this.settings.gameDefinitionFileName}' at path '{filePath}'. This can be repaired automatically, but you will have to setup the {this.settings.gameDefinitionFileName} manually.", "OK", "Cancel")) {
					this.gameDefinition = new TGameDefinition();
					File.WriteAllText(filePath, JsonUtility.ToJson(this.gameDefinition));
				} else {
					this.declinedRepairPath = filePath;
					throw new Exception($"No '{this.settings.gameDefinitionFileName}' found at path '{filePath}', please create the specified file manually");
				}
			} else {
				this.declinedRepairPath = null;
			}
			this.gameDefinition = JsonUtility.FromJson<TGameDefinition>(File.ReadAllText(filePath));
		}

		string[] DrawSelector(string[] directories) {
			GUILayout.BeginHorizontal();
			this.selectedIndex = EditorGUILayout.Popup("GameDefinition", this.selectedIndex, directories.Select(Path.GetFileName).ToArray());
			if (GUILayout.Button("Delete")) {
				Directory.Delete(directories[this.selectedIndex], true);
				directories = directories.Where(dir => dir != directories[this.selectedIndex]).ToArray();
                if (directories.Length > 0 && this.selectedIndex >= directories.Length - 1) {
                    this.selectedIndex = directories.Length - 1;
                }
            }
			GUILayout.EndHorizontal();
			return directories;
		}

		string[] DrawCreateNew(string[] directories) {
			GUILayout.BeginHorizontal();
			GUILayout.Label("Create new: ");
			this.newDefinitionName = GUILayout.TextArea(this.newDefinitionName);
			if (GUILayout.Button("Create")) {
				var newDirectory = Path.Combine(this.settings.gameDefinitionDirectoryPath, this.newDefinitionName);
				Directory.CreateDirectory(newDirectory);
				directories = Directory.GetDirectories(this.settings.gameDefinitionDirectoryPath);
				this.selectedIndex = Array.IndexOf(directories, newDirectory);
                Debug.Log("selected index: " + this.selectedIndex);
				this.gameDefinition = new TGameDefinition();
				File.WriteAllText(Path.Combine(newDirectory, this.settings.gameDefinitionFileName), JsonUtility.ToJson(this.gameDefinition));
			}

			GUILayout.EndHorizontal();
			return directories;
		}
	}
}