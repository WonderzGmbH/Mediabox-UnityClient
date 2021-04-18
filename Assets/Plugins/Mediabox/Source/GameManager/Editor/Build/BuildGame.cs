using System.IO;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.HubPlugins;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build {
	public static class BuildGame {
		[MenuItem("MediaBox/Build Game")]
		public static void Build() {
			Build(EditorUserBuildSettings.activeBuildTarget);
		}

		public static void Build(BuildTarget buildTarget, bool streamingAssetsGameDefinitionMode = false, bool autoRun = true) {
			BuildPlayerOptions buildPlayerOptions;
			try {
				buildPlayerOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(new BuildPlayerOptions(){target = buildTarget});
			} catch (BuildPlayerWindow.BuildMethodException e) {
				Debug.Log("BuildMethodException caught: " + e);
				return;
			}

			buildPlayerOptions.target = buildTarget;
			var settingsPlugin = new SettingsPlugin();
			settingsPlugin.Update();
			var manifestPath = settingsPlugin.buildSettings.GetAssetBundleManifestPath(buildTarget);
			if (!ValidateAssetBundleManifestFileExists(manifestPath))
				return;
			buildPlayerOptions.assetBundleManifestPath = manifestPath;
			if (autoRun)
				buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;
			var oldIntegrationMode = settingsPlugin.settings.ServerMode;
			if(streamingAssetsGameDefinitionMode) {
				Debug.Log($"Configuring {nameof(ServerMode)}.{nameof(ServerMode.Simulation)}");
				settingsPlugin.settings.ServerMode = ServerMode.Simulation;
				EditorUtility.SetDirty(settingsPlugin.settings);
			}
			BuildPipeline.BuildPlayer(buildPlayerOptions);
			settingsPlugin.settings.ServerMode = oldIntegrationMode;
		}
		
		static bool ValidateAssetBundleManifestFileExists(string manifestPath) {
			return File.Exists(manifestPath) || EditorUtility.DisplayDialog("Warning", "It is recommended to build Asset Bundles first, using the GameDefinitionManager-Window. Continuing without may result in required code being stripped from the engine.", "OK", "Cancel");
		}
	}
}