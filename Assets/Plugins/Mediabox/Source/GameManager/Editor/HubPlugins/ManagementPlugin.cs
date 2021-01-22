using System;
using System.IO;
using System.Linq;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.HubPlugins {

	[Serializable]
	public class ManagementPlugin : IHubPlugin {
		string[] directories;
		int selectedIndex;

		public string SelectedDirectory {
			get {
				if (this.directories == null || this.directories.Length <= this.selectedIndex || this.selectedIndex < 0)
					return null;
				return this.directories[this.selectedIndex];
			}
		}
		public string[] AllDirectories => this.directories;
		readonly SettingsPlugin settingsPlugin;
		readonly GameDefinitionHub manager;
		string newDefinitionName;

		public ManagementPlugin(SettingsPlugin settingsPlugin, GameDefinitionHub manager) {
			this.settingsPlugin = settingsPlugin;
			this.manager = manager;
		}
		
		public string Title => $"Definition Management ({this.directories?.Length ?? 0} Games)";
		public bool ToggleableWithTitleLabel => true;

		public void Update() {
			this.directories = LoadGameDefinitions();
			LoadSelectedIndex();
		}

		public bool Render() {
			this.directories = DrawCreateNew();
			if (this.directories.Length == 0) {
				EditorGUILayout.HelpBox("Create a new GameDefinition to begin work.", MessageType.Info);
				return false;
			}
			ValidateSelectedGameDefinition(this.directories);
			this.directories = DrawSelector(this.directories);
			if (this.directories.Length == 0)
				return false;
			return true;
		}

		string[] LoadGameDefinitions() {
			var directories = Directory.GetDirectories(this.settingsPlugin.settings.gameDefinitionDirectoryPath);
			return directories;
		}

		void LoadSelectedIndex() {
			var index = Array.IndexOf(this.directories, Path.Combine(this.settingsPlugin.settings.gameDefinitionDirectoryPath, SimulationMode.BundleName));
			this.selectedIndex = index > 0 ? index : 0;
		}
		
		string[] DrawCreateNew() {
			using (LayoutUtility.HorizontalStack()) {
				GUILayout.Label("Create new: ");
				this.newDefinitionName = GUILayout.TextArea(this.newDefinitionName);
				if (GUILayout.Button("Create")) {
					var newDirectory = Path.Combine(this.settingsPlugin.settings.gameDefinitionDirectoryPath, this.newDefinitionName);
					Directory.CreateDirectory(newDirectory);
					this.directories = Directory.GetDirectories(this.settingsPlugin.settings.gameDefinitionDirectoryPath);
					this.selectedIndex = Array.IndexOf(this.directories, newDirectory);
					Debug.Log("selected index: " + this.selectedIndex);
					File.WriteAllText(Path.Combine(newDirectory, this.settingsPlugin.settings.gameDefinitionFileName), JsonUtility.ToJson(this.manager.CreateGameDefinition()));
				}
			}
			return this.directories;
		}

		void ValidateSelectedGameDefinition(string[] directories) {
			this.selectedIndex = Mathf.Clamp(this.selectedIndex, 0, directories.Length);
		}
		
		string[] DrawSelector(string[] directories) {
			using (LayoutUtility.HorizontalStack()) {
				this.selectedIndex = EditorGUILayout.Popup("GameDefinition", this.selectedIndex, directories.Select(Path.GetFileName).ToArray());
				if (GUILayout.Button("Delete")) {
					Directory.Delete(directories[this.selectedIndex], true);
					directories = directories.Where(dir => dir != directories[this.selectedIndex]).ToArray();
					if (directories.Length > 0 && this.selectedIndex >= directories.Length - 1) {
						this.selectedIndex = directories.Length - 1;
					}
				}
			}
			return directories;
		}
	}
}