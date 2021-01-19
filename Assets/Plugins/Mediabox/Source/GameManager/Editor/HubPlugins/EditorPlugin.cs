using System;
using System.IO;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.HubPlugins {
	public class EditorPlugin<TGameDefinition> : IHubPlugin
		where TGameDefinition : class, IGameDefinition, new() {
		readonly SettingsPlugin settingsPlugin;
		readonly ManagementPlugin management;
		readonly GameDefinitionHub<TGameDefinition> manager;
		string declinedRepairPath;

		public EditorPlugin(SettingsPlugin settingsPlugin, ManagementPlugin management, GameDefinitionHub<TGameDefinition> manager) {
			this.settingsPlugin = settingsPlugin;
			this.management = management;
			this.manager = manager;
		}
		public string Title => $"Definition Editor ({this.management.SelectedDirectory})";
		public bool ToggleableWithTitleLabel => true;

		public void Update() {
			var gameDefinitionPath = Path.Combine(this.management.SelectedDirectory, this.settingsPlugin.settings.gameDefinitionFileName);
			LoadGameDefinition(gameDefinitionPath);
		}

		public bool Render() {
			var gameDefinitionPath = Path.Combine(this.management.SelectedDirectory, this.settingsPlugin.settings.gameDefinitionFileName);
			DrawGameDefinitionEditor(gameDefinitionPath);
			return true;
		}
		
		void LoadGameDefinition(string gameDefinitionPath) {
			if (!File.Exists(gameDefinitionPath)) {
				PathUtility.EnsureDirectory(Path.GetDirectoryName(gameDefinitionPath));
				if (this.declinedRepairPath != gameDefinitionPath && EditorUtility.DisplayDialog("GameDefinitionManager Error", $"Expected to find a file named '{this.settingsPlugin.settings.gameDefinitionFileName}' at path '{gameDefinitionPath}'. This can be repaired automatically, but you will have to setup the {this.settingsPlugin.settings.gameDefinitionFileName} manually.", "OK", "Cancel")) {
					this.manager.gameDefinition = new TGameDefinition();
					File.WriteAllText(gameDefinitionPath, JsonUtility.ToJson(this.manager.gameDefinition));
				} else {
					this.declinedRepairPath = gameDefinitionPath;
					throw new Exception($"No '{this.settingsPlugin.settings.gameDefinitionFileName}' found at path '{gameDefinitionPath}', please create the specified file manually");
				}
			} else {
				this.declinedRepairPath = null;
			}

			this.manager.gameDefinition = JsonUtility.FromJson<TGameDefinition>(File.ReadAllText(gameDefinitionPath));
		}

		void DrawGameDefinitionEditor(string gameDefinitionPath) {
			var so = new SerializedObject(this.manager);
			so.ApplyModifiedProperties();
			var property = so.FindProperty(nameof(this.manager.gameDefinition));
			EditorGUILayout.PropertyField(property, true);
			so.ApplyModifiedProperties();
			PathUtility.EnsureDirectory(Path.GetDirectoryName(gameDefinitionPath));
			File.WriteAllText(gameDefinitionPath, JsonUtility.ToJson(this.manager.gameDefinition));
		}
	}
}