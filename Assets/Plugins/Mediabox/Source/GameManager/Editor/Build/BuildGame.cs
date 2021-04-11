using System.IO;
using Mediabox.GameManager.Editor.HubPlugins;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build {
	public static class BuildGame {
		[MenuItem("MediaBox/Build Game")]
		public static void Build() {
			BuildPlayerOptions buildPlayerOptions;
			try {
				buildPlayerOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(new BuildPlayerOptions());
			} catch (BuildPlayerWindow.BuildMethodException e) {
				Debug.Log("BuildMethodException caught: " + e);
				return;
			}

			var buildTarget = buildPlayerOptions.target;
			var settingsPlugin = new SettingsPlugin();
			settingsPlugin.Update();
			var manifestPath = settingsPlugin.buildSettings.GetAssetBundleManifestPath(buildTarget);
			if (!ValidateAssetBundleManifestFileExists(manifestPath))
				return;
			buildPlayerOptions.assetBundleManifestPath = manifestPath;
			BuildPipeline.BuildPlayer(buildPlayerOptions);
		}

		static bool ValidateAssetBundleManifestFileExists(string manifestPath) {
			return File.Exists(manifestPath) || EditorUtility.DisplayDialog("Warning", "It is recommended to build Asset Bundles first, using the GameDefinitionManager-Window. Continuing without may result in required code being stripped from the engine.", "OK", "Cancel");
		}
	}
}