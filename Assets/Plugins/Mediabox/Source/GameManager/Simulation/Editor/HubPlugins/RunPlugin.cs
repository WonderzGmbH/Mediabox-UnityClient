using System;
using System.IO;
using System.IO.Compression;
using Mediabox.GameManager.Editor;
using Mediabox.GameManager.Editor.Build;
using Mediabox.GameManager.Editor.HubPlugins;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Simulation.Editor.HubPlugins {
	public class RunPlugin : IHubPlugin {
		readonly SettingsPlugin settingsPlugin;
		readonly ManagementPlugin managementPlugin;
		readonly BuildPlugin build;
		readonly GameDefinitionHub manager;

		public RunPlugin(SettingsPlugin settingsPlugin, ManagementPlugin managementPlugin, BuildPlugin build, GameDefinitionHub manager) {
			this.settingsPlugin = settingsPlugin;
			this.managementPlugin = managementPlugin;
			this.build = build;
			this.manager = manager;
		}

		public string Title => "Run";
		public bool ToggleableWithTitleLabel => true;

		public void Update() { }

		public bool Render() {
			var directories = this.managementPlugin.AllDirectories;
			DrawRunPlatformsArea();
			GUILayout.Label($"Running: {string.Join(", ", GetSelectedBuildTargets())}");

			if (GUILayout.Button("Build and Run All")) {
				this.build.BuildGameDefinitions(directories, true, new []{GetSelectedBuildTargets()});
				RunGameDefinitions(directories, true, GetSelectedBuildTargets());
				return false;
			}

			if (GUILayout.Button($"Build and Run {Path.GetFileName(this.managementPlugin.SelectedDirectory)}")) {
				this.build.BuildGameDefinitions(new[] {this.managementPlugin.SelectedDirectory}, false, new []{GetSelectedBuildTargets()});
				RunGameDefinitions(new[] {this.managementPlugin.SelectedDirectory}, false, GetSelectedBuildTargets());
			}
			
			if (GUILayout.Button($"Run All")) {
				RunGameDefinitions(directories, true, GetSelectedBuildTargets());
				return false;
			}

			if (this.manager != null & GUILayout.Button($"Run {Path.GetFileName(this.managementPlugin.SelectedDirectory)}")) {
				RunGameDefinitions(new[] {this.managementPlugin.SelectedDirectory}, false, GetSelectedBuildTargets());
				return false;
			}

			return true;
		}


		enum RunPlatformOption {
			CurrentPlatform,
			ManualPlatforms
		}

		RunPlatformOption runPlatformOption;
		BuildTarget manualPlatform;

		BuildTarget GetSelectedBuildTargets() {
			switch (this.runPlatformOption) {
				case RunPlatformOption.CurrentPlatform:
					return EditorUserBuildSettings.activeBuildTarget;
				case RunPlatformOption.ManualPlatforms:
					return this.manualPlatform;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		void RunGameDefinitions(string[] directories, bool clearDirectory, BuildTarget buildTarget) {
			var targetPath = Path.Combine(Application.streamingAssetsPath, "GameDefinitions");
			PathUtility.EnsureEmptyDirectory(targetPath);
			foreach (var directory in directories) {
				var localDir = PathUtility.GetRelativePath(directory, this.settingsPlugin.settings.gameDefinitionDirectoryPath);
				var zipPath = Path.ChangeExtension(Path.Combine(Path.Combine(this.settingsPlugin.buildSettings.gameDefinitionBuildPath, buildTarget.ToString()), localDir), "zip");
				if (!File.Exists(zipPath)) {
					Debug.LogWarning($"Skipping {zipPath}, as it has not been built. Make sure to rebuild the Game Definitions.");
					continue;
				}

				var zipTargetPath = Path.Combine(targetPath, localDir);
				ZipFile.ExtractToDirectory(zipPath, zipTargetPath);
			}
			BuildGame.Build(buildTarget, true, true);
			PathUtility.DeleteDirectoryIfExists(targetPath);
			PathUtility.DeleteFileIfExists($"{targetPath}.meta");
		}

		void DrawRunPlatformsArea() {
			this.runPlatformOption = (RunPlatformOption) EditorGUILayout.EnumPopup(this.runPlatformOption);
			if (this.runPlatformOption == RunPlatformOption.ManualPlatforms) {
				this.manualPlatform = (BuildTarget) EditorGUILayout.EnumPopup(this.manualPlatform);
			} else if (this.runPlatformOption == RunPlatformOption.CurrentPlatform) {
				GUILayout.Label("Current Platform: " + EditorUserBuildSettings.activeBuildTarget);
			}
		}
	}
}