using System.IO;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.HubPlugins {
	public class DirectoryPlugin : IGameDefinitionManagerPlugin {
		readonly SettingsPlugin settingsPlugin;

		public DirectoryPlugin(SettingsPlugin settingsPlugin) {
			this.settingsPlugin = settingsPlugin;
		}
		public string Title => $"Directory {this.settingsPlugin.settings.gameDefinitionDirectoryPath}";
		public bool ToggleableWithTitleLabel => true;

		public void Update() {
			EnsureDirectory();
		}

		public bool Render() {
			GUILayout.Label($"Path: {this.settingsPlugin.settings.gameDefinitionDirectoryPath}");
			DrawDirectoryArea();
			EditorGUILayout.HelpBox("You can change the path in your Settings.", MessageType.Info);
			return true;
		}

		void DrawDirectoryArea() {
			if (GUILayout.Button("Open Directory")) {
				OpenInFileBrowser.Open(this.settingsPlugin.settings.gameDefinitionDirectoryPath);
			}
		}

		void EnsureDirectory() {
			if (!Directory.Exists(this.settingsPlugin.settings.gameDefinitionDirectoryPath)) {
				Directory.CreateDirectory(this.settingsPlugin.settings.gameDefinitionDirectoryPath);
			}
		}
	}
}