using System.IO;
using Mediabox.GameManager.Editor.Build;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.HubPlugins {
	public class CustomPlatformSettingsPlugin : IGameDefinitionManagerPlugin {
		readonly GameDefinitionManagementPlugin management;
		readonly GameDefinitionHub manager;

		public CustomPlatformSettingsPlugin(GameDefinitionManagementPlugin management, GameDefinitionHub manager) {
			this.management = management;
			this.manager = manager;
		}
		public string Title => $"Custom Platform Settings ({this.OnOffString} for {this.management.SelectedDirectory})";
		public bool ToggleableWithTitleLabel => true;
		string OnOffString => (this.manager.customPlatformSettings != null ? "ON" : "OFF");

		public void Update() {
			var platformSettingsPath = Path.Combine(this.management.SelectedDirectory, GameDefinitionBuildSettings.customPlatformSettings);
			LoadCustomPlatformSettings(platformSettingsPath);
		}

		public bool Render() {
			var platformSettingsPath = Path.Combine(this.management.SelectedDirectory, GameDefinitionBuildSettings.customPlatformSettings);
			DrawCreateCustomPlatformSettings(platformSettingsPath);
			DrawCustomPlatformSettingsEditor(platformSettingsPath);
			EditorGUILayout.HelpBox("These overrides allow you to additionally whitelist and blacklist platforms for this one game definition. Change the BuildSettings, if you want to add or remove a platform for all games.", MessageType.Info);
			return true;
		}
		
		void LoadCustomPlatformSettings(string platformSettingsPath) {
			if (!File.Exists(platformSettingsPath)) {
				this.manager.customPlatformSettings = null;
				return;
			}

			this.manager.customPlatformSettings = JsonUtility.FromJson<CustomPlatformSettings>(File.ReadAllText(platformSettingsPath));
		}

		void DrawCreateCustomPlatformSettings(string platformSettingsPath) {
			var hasPlatformSettings = this.manager.customPlatformSettings != null;
			var createPlatformSettings = EditorGUILayout.Toggle("Override platforms", hasPlatformSettings);
			if (createPlatformSettings == hasPlatformSettings)
				return;
			if (createPlatformSettings) {
				this.manager.customPlatformSettings = new CustomPlatformSettings();
			} else {
				this.manager.customPlatformSettings = null;
				File.Delete(platformSettingsPath);
			}
		}
		
		void DrawCustomPlatformSettingsEditor(string platformSettingsPath) {
			if (this.manager.customPlatformSettings == null)
				return;
			ScriptableObject target = this.manager;
			var so = new SerializedObject(target);
			so.ApplyModifiedProperties();
			var property = so.FindProperty(nameof(this.manager.customPlatformSettings));
			EditorGUILayout.PropertyField(property, true);
			so.ApplyModifiedProperties();
			File.WriteAllText(platformSettingsPath, JsonUtility.ToJson(this.manager.customPlatformSettings));
		}
	}
}