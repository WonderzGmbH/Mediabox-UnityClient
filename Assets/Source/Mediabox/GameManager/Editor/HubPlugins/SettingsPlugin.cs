using System.IO;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Build;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.HubPlugins {
	public class SettingsPlugin : IHubPlugin {
		public GameDefinitionSettings settings { get; private set; }
		public GameDefinitionBuildSettings buildSettings { get; private set; }

		public void Update() {
			LoadOrCreateSettings();
		}

		public bool Render() {
			if (this.settings == null) {
				EditorGUILayout.HelpBox("Could not find or create GameDefinitionSettings. This might be caused by invalid SettingsPath in GameDefinitionSettings.cs-File.", MessageType.Error);
				return false;
			}

			DrawSettingsArea();
			DrawBuildSettingsArea();
			EditorGUILayout.HelpBox("These are your project-wide settings and build changes. Changes to these should be committed and shared with your team. Make sure to have a backup of your project before making changes.", MessageType.Info);
			return true;
		}
		
		public string Title => "Settings";
		public bool ToggleableWithTitleLabel => true;

		void LoadOrCreateSettings() {
			this.settings = AssetDatabase.LoadAssetAtPath<GameDefinitionSettings>(GameDefinitionSettings.SettingsPath) ?? TryCreateSettings();
			this.buildSettings = AssetDatabase.LoadAssetAtPath<GameDefinitionBuildSettings>(GameDefinitionBuildSettings.SettingsPath) ?? TryCreateBuildSettings();
		}

		static GameDefinitionSettings TryCreateSettings() {
			var settingsDirectory = Path.GetDirectoryName(GameDefinitionSettings.SettingsPath);
			if (settingsDirectory == null)
				return null;
			if (!Directory.Exists(settingsDirectory))
				Directory.CreateDirectory(settingsDirectory);
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GameDefinitionSettings>(), GameDefinitionSettings.SettingsPath);
			return AssetDatabase.LoadAssetAtPath<GameDefinitionSettings>(GameDefinitionSettings.SettingsPath);
		}

		static GameDefinitionBuildSettings TryCreateBuildSettings() {
			var settingsDirectory = Path.GetDirectoryName(GameDefinitionBuildSettings.SettingsPath);
			if (settingsDirectory == null)
				return null;
			if (!Directory.Exists(settingsDirectory))
				Directory.CreateDirectory(settingsDirectory);
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GameDefinitionBuildSettings>(), GameDefinitionBuildSettings.SettingsPath);
			return AssetDatabase.LoadAssetAtPath<GameDefinitionBuildSettings>(GameDefinitionBuildSettings.SettingsPath);
		}
		
		void DrawSettingsArea() {
			if (GUILayout.Button("Edit Settings")) {
				Selection.activeObject = this.settings;
			}
		}

		void DrawBuildSettingsArea() {
			if (GUILayout.Button("Edit Build Settings")) {
				Selection.activeObject = this.buildSettings;
			}
		}
	}
}