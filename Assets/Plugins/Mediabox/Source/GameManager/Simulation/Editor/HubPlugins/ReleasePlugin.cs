using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor;
using Mediabox.GameManager.Editor.Build;
using Mediabox.GameManager.Editor.HubPlugins;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Simulation.Editor.HubPlugins {
	public class ReleasePlugin : IHubPlugin {
		readonly SettingsPlugin settingsPlugin;
		readonly ManagementPlugin managementPlugin;
		readonly BuildPlugin build;
		readonly GameDefinitionHub manager;

		public ReleasePlugin(SettingsPlugin settingsPlugin, ManagementPlugin managementPlugin, BuildPlugin build, GameDefinitionHub manager) {
			this.settingsPlugin = settingsPlugin;
			this.managementPlugin = managementPlugin;
			this.build = build;
			this.manager = manager;
		}

		public string Title => "Release";
		public bool ToggleableWithTitleLabel => true;

		bool RebuildGameDefinitions {
			get => EditorPrefs.GetBool($"{typeof(DebugRunPlugin).FullName}.{nameof(RebuildGameDefinitions)}", true);
			set => EditorPrefs.SetBool($"{typeof(DebugRunPlugin).FullName}.{nameof(RebuildGameDefinitions)}", value);
		}
		bool AutoRunPlayer {
			get => EditorPrefs.GetBool($"{typeof(DebugRunPlugin).FullName}.{nameof(AutoRunPlayer)}", true);
			set => EditorPrefs.SetBool($"{typeof(DebugRunPlugin).FullName}.{nameof(AutoRunPlayer)}", value);
		}

		public void Update() { }

		public bool Render() {
			EditorGUILayout.HelpBox("This creates a Release Build.", MessageType.Info);
			var directories = this.managementPlugin.AllDirectories;
			DrawRunPlatformsArea();
			if (GUILayout.Button($"Build Release Client ({GetSelectedBuildTarget()})")) {
				if(this.RebuildGameDefinitions)
					this.build.BuildGameDefinitions(directories, true, new []{GetSelectedBuildTarget()});
				if(ValidateBuildConditions(this.settingsPlugin.buildSettings.GetAssetBundleManifestPath(GetSelectedBuildTarget())))
					BuildGame.Build(GetSelectedBuildTarget(), false, this.AutoRunPlayer);
				return false;
			}

			this.AutoRunPlayer = EditorGUILayout.Toggle("Auto Run Player", this.AutoRunPlayer);
			this.RebuildGameDefinitions = EditorGUILayout.Toggle("Rebuild Game Definitions", this.RebuildGameDefinitions);

			return true;
		}


		enum RunPlatformOption {
			CurrentPlatform,
			ManualPlatforms
		}

		RunPlatformOption runPlatformOption;
		BuildTarget manualPlatform;

		BuildTarget GetSelectedBuildTarget() {
			switch (this.runPlatformOption) {
				case RunPlatformOption.CurrentPlatform:
					return EditorUserBuildSettings.activeBuildTarget;
				case RunPlatformOption.ManualPlatforms:
					return this.manualPlatform;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		void DrawRunPlatformsArea() {
			this.runPlatformOption = (RunPlatformOption) EditorGUILayout.EnumPopup(this.runPlatformOption);
			if (this.runPlatformOption == RunPlatformOption.ManualPlatforms) {
				this.manualPlatform = (BuildTarget) EditorGUILayout.EnumPopup(this.manualPlatform);
			} else if (this.runPlatformOption == RunPlatformOption.CurrentPlatform) {
				GUILayout.Label("Current Platform: " + EditorUserBuildSettings.activeBuildTarget);
			}
		}
		
		bool ValidateBuildConditions(string manifestPath) {
			
			if (!typeof(IGameBundleDefinition).IsAssignableFrom(this.manager.GetGameDefinitionType()))
				return true;

			if (!this.build.GetGameDefinitions(this.managementPlugin.AllDirectories, true, new[] { GetSelectedBuildTarget() }, out var gameDefinitionsPerPlatform) &&
			    !EditorUtility.DisplayDialog("There have been errors", "Some GameDefinitions had errors. Do you still want to continue?", "OK", "Cancel")) {
				return false;
			}

			var gameDefinitions = gameDefinitionsPerPlatform.FirstOrDefault(grouping => grouping.Key == GetSelectedBuildTarget());
			if (gameDefinitions == null || !gameDefinitions.Any()) {
				Debug.LogWarning("It appears, that no Game Definitions have been configured to be built for this platform. You can ignore this warning, if this is intended.");
				return true;
			}

			var manifestBundlePath = Path.ChangeExtension(manifestPath, null);
			if (!File.Exists(manifestPath) || !File.Exists(manifestBundlePath)) {
				if (EditorUtility.DisplayDialog("Warning", "It appears that Game Definition Bundles have not been built, yet. It is recommended to do this step first, to avoid problems with Code Stripping.", "OK", "Ignore")) {
					this.build.BuildGameDefinitions(this.managementPlugin.AllDirectories, true, new []{GetSelectedBuildTarget()}, true);
				}

				return true;
			}
			
			AssetBundle.UnloadAllAssetBundles(true);
			var manifestAssetBundle = AssetBundle.LoadFromFile(manifestBundlePath);
			var manifest = manifestAssetBundle.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));
			manifestAssetBundle.Unload(false);
			var assetBundles = new HashSet<string>(manifest.GetAllAssetBundles());
				
			HashSet<string> missingGameDefinitions = new HashSet<string>();
			foreach (var gameDefinition in gameDefinitions) {
				if (!assetBundles.Contains((gameDefinition.gameDefinition as IGameBundleDefinition).BundleName)) {
					missingGameDefinitions.Add(gameDefinition.directory);
				}
			}

			if (missingGameDefinitions.Count > 0 && EditorUtility.DisplayDialog("Warning", $"The following Game Definitions have not been built, yet. It is recommended to build them before building your Unity Client, to avoid problems with Engine Stripping. Build these Bundles now?: {string.Join(", ", missingGameDefinitions)}", "OK", "Ignore")) {
				this.build.BuildGameDefinitions(this.managementPlugin.AllDirectories, true, new []{GetSelectedBuildTarget()}, true);
			}

			return true;
		}
	}
}