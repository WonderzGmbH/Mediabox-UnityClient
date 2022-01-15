using System;
using System.IO;
using System.IO.Compression;
using Mediabox.GameKit.GameManager;
using Mediabox.GameManager.Editor;
using Mediabox.GameManager.Editor.Build;
using Mediabox.GameManager.Editor.HubPlugins;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Simulation.Editor.HubPlugins {
	public class DebugRunPlugin : IHubPlugin {
		readonly SettingsPlugin settingsPlugin;
		readonly ManagementPlugin managementPlugin;
		readonly BuildPlugin build;
		readonly GameDefinitionHub manager;

		public DebugRunPlugin(SettingsPlugin settingsPlugin, ManagementPlugin managementPlugin, BuildPlugin build, GameDefinitionHub manager) {
			this.settingsPlugin = settingsPlugin;
			this.managementPlugin = managementPlugin;
			this.build = build;
			this.manager = manager;
		}

		public string Title => "Debug Run";
		public bool ToggleableWithTitleLabel => true;

		bool AutoRunPlayer {
			get => EditorPrefs.GetBool($"{typeof(DebugRunPlugin).FullName}.{nameof(AutoRunPlayer)}", true);
			set => EditorPrefs.SetBool($"{typeof(DebugRunPlugin).FullName}.{nameof(AutoRunPlayer)}", value);
		}

		public void Update() { }

		public bool Render() {
			EditorGUILayout.HelpBox($"Make sure to have a {nameof(SimulationMediaboxServerBehaviour)}-Script attached to the GameObject that also has your {nameof(GameManagerBase)}.", MessageType.Info);
			var directories = this.managementPlugin.AllDirectories;
			DrawRunPlatformsArea();
			GUILayout.Label($"Running: {string.Join(", ", GetSelectedBuildTargets())}");

			if (GUILayout.Button("Build and Run All Games")) {
				this.build.BuildGameDefinitions(directories, true, new []{GetSelectedBuildTargets()});
				RunGameDefinitions(directories, true, GetSelectedBuildTargets(), this.AutoRunPlayer);
				return false;
			}

			if (GUILayout.Button($"Build and Run Game '{Path.GetFileName(this.managementPlugin.SelectedDirectory)}'")) {
				this.build.BuildGameDefinitions(new[] {this.managementPlugin.SelectedDirectory}, false, new []{GetSelectedBuildTargets()});
				RunGameDefinitions(new[] {this.managementPlugin.SelectedDirectory}, false, GetSelectedBuildTargets(), this.AutoRunPlayer);
			}
			
			if (GUILayout.Button($"Run All Games")) {
				RunGameDefinitions(directories, true, GetSelectedBuildTargets(), this.AutoRunPlayer);
				return false;
			}

			if (this.manager != null & GUILayout.Button($"Run Game '{Path.GetFileName(this.managementPlugin.SelectedDirectory)}'")) {
				RunGameDefinitions(new[] {this.managementPlugin.SelectedDirectory}, false, GetSelectedBuildTargets(), this.AutoRunPlayer);
				return false;
			}

			this.AutoRunPlayer = EditorGUILayout.Toggle("Auto Run Player", this.AutoRunPlayer);

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

		void RunGameDefinitions(string[] directories, bool clearDirectory, BuildTarget buildTarget, bool autoRun) {
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
			BuildGame.Build(buildTarget, true, autoRun);
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