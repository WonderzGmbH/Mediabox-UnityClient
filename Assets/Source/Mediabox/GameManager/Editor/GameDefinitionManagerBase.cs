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

		const string autoSimulateEditorPrefKey = "Mediabox.GameManager.Editor.AutoSimulate";
		int selectedIndex;
		string newDefinitionName;

		public TGameDefinition gameDefinition;
		static TWindow window;
		EditorNativeAPI simulationModeNativeApi;
		bool IsInSimulationMode => this.simulationModeNativeApi != null;
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
				DrawSimulationArea(directories);
			}
			DrawBuildArea();
		}

		static void DrawBuildArea() {
			if (GUILayout.Button("Build GameDefinitions")) {
				EditorUtility.DisplayDialog("Not implemented", "This feature has not been implemented so far.", "OK");
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

		void DrawSimulationArea(string[] directories) {
			if (directories.Length == 0)
				return;
			EditorGUI.BeginChangeCheck();
			var autoSimulate = GUILayout.Toggle(EditorPrefs.GetBool(autoSimulateEditorPrefKey, true), "Auto-Simulate");
			LoadDefaultSceneOnPlayMode.enabled = autoSimulate;
			if(EditorGUI.EndChangeCheck())
				EditorPrefs.SetBool(autoSimulateEditorPrefKey, autoSimulate);
			if (!Application.isPlaying) {
				this.simulationModeNativeApi = null;
				DrawStartPlayMode( directories[this.selectedIndex]);
				DrawDummyStartSimulation();
			} else {
				DrawStopPlayMode();
				if (!this.IsInSimulationMode) {
					DrawStartSimulationMode(autoSimulate, directories[this.selectedIndex]);
				} else {
					DrawSimulationMode(autoSimulate, directories[this.selectedIndex]);
				}
			}
		}

		void DrawSimulationMode(bool autoSimulate, string contentBundleFolder) {
			this.simulationModeNativeApi.OnGUI(autoSimulate, contentBundleFolder);
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

		void DrawStartSimulationMode(bool autoSimulate, string contentBundleFolder) {
			if ((autoSimulate && Event.current.type == EventType.Repaint) || GUILayout.Button("Start Simulation")) {
				StartSimulationMode(contentBundleFolder);
			}
		}

		void StartSimulationMode(string contentBundleFolder) {
			this.simulationModeNativeApi = new EditorNativeAPI(contentBundleFolder);
			FindObjectOfType<GameManagerBase<TGameDefinition>>().SetNativeApi(this.simulationModeNativeApi);
		}

		void DrawStartPlayMode(string contentBundleFolder) {
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
				this.selectedIndex = Array.IndexOf(directories, this.newDefinitionName);
				this.gameDefinition = new TGameDefinition();
				File.WriteAllText(Path.Combine(newDirectory, this.settings.gameDefinitionFileName), JsonUtility.ToJson(this.gameDefinition));
			}

			GUILayout.EndHorizontal();
			return directories;
		}
	}
}