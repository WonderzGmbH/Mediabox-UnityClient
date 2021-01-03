using System;
using System.IO;
using Mediabox.GameManager.Editor.Build;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.HubPlugins {
	public class BuildPlugin : IGameDefinitionManagerPlugin {
		readonly SettingsPlugin settingsPlugin;
		readonly GameDefinitionManagementPlugin managementPlugin;
		readonly GameDefinitionHub manager;

		public BuildPlugin(SettingsPlugin settingsPlugin, GameDefinitionManagementPlugin managementPlugin, GameDefinitionHub manager) {
			this.settingsPlugin = settingsPlugin;
			this.managementPlugin = managementPlugin;
			this.manager = manager;
		}

		public string Title => "Build";
		public bool ToggleableWithTitleLabel => true;

		public void Update() { }

		public bool Render() {
			var directories = this.managementPlugin.AllDirectories;
			DrawBuildPlatformsArea();
			GUILayout.Label($"Building: {string.Join(", ", GetSelectedBuildTargets())}");
			EditorGUI.EndDisabledGroup();
			if (GUILayout.Button($"Build All")) {
				BuildGameDefinitions(directories, true, GetSelectedBuildTargets());
			}

			if (this.manager != null & GUILayout.Button($"Build {Path.GetFileName(this.managementPlugin.SelectedDirectory)}")) {
				BuildGameDefinitions(new[] {this.managementPlugin.SelectedDirectory}, false, GetSelectedBuildTargets());
			}

			return true;
		}


		enum BuildPlatformOption {
			AllSupportedPlatforms,
			CurrentPlatform,
			ManualPlatforms
		}

		BuildPlatformOption buildPlatformOption;
		BuildTarget manualPlatform;

		BuildTarget[] GetSelectedBuildTargets() {
			switch (this.buildPlatformOption) {
				case BuildPlatformOption.AllSupportedPlatforms:
					return this.settingsPlugin.buildSettings.supportedBuildTargets;
				case BuildPlatformOption.CurrentPlatform:
					return new[] {EditorUserBuildSettings.activeBuildTarget};
				case BuildPlatformOption.ManualPlatforms:
					return new[] {this.manualPlatform};
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		void BuildGameDefinitions(string[] directories, bool clearDirectory, BuildTarget[] buildTargets) {
			var build = CreateGameDefinitionBuild(directories, clearDirectory, buildTargets);
			build.Execute();
		}

		void DrawBuildPlatformsArea() {
			this.buildPlatformOption = (BuildPlatformOption) EditorGUILayout.EnumPopup(this.buildPlatformOption);
			if (this.buildPlatformOption == BuildPlatformOption.ManualPlatforms) {
				this.manualPlatform = (BuildTarget) EditorGUILayout.EnumPopup(this.manualPlatform);
			} else if (this.buildPlatformOption == BuildPlatformOption.CurrentPlatform) {
				GUILayout.Label("Current Platform: " + EditorUserBuildSettings.activeBuildTarget);
			}
		}

		/// <summary>
		/// Overload this method in order to inject your own Build Script.
		/// </summary>
		protected virtual GameDefinitionBuild CreateGameDefinitionBuild(string[] directories, bool clearDirectory, BuildTarget[] buildTargets) {
			return new GameDefinitionBuild(directories, clearDirectory, buildTargets, this.manager.GetGameDefinitionType(), this.settingsPlugin.settings, this.settingsPlugin.buildSettings);
		}
	}
}