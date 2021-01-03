using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build.Plugins {
	public class BuildPlugin<TGameDefinition> : IGameDefinitionManagerPlugin
		where TGameDefinition : class, IGameDefinition, new() {
		readonly SettingsPlugin settingsPlugin;
		readonly GameDefinitionManagementPlugin managementPlugin;
		readonly GameDefinitionManagerBase manager;

		public BuildPlugin(SettingsPlugin settingsPlugin, GameDefinitionManagementPlugin managementPlugin, GameDefinitionManagerBase<TGameDefinition> manager) {
			this.settingsPlugin = settingsPlugin;
			this.managementPlugin = managementPlugin;
			this.manager = manager;
		}

		public string Title => "Build";

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
		protected virtual GameDefinitionBuild<TGameDefinition> CreateGameDefinitionBuild(string[] directories, bool clearDirectory, BuildTarget[] buildTargets) {
			return new GameDefinitionBuild<TGameDefinition>(directories, clearDirectory, buildTargets, this.settingsPlugin.settings, this.settingsPlugin.buildSettings);
		}
	}
}