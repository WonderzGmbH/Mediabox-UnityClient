using System.Collections.Generic;
using System.IO;
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
			var oldDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildPlayerOptions.targetGroup);
			var defines = new HashSet<string>(oldDefines.Split(','));
			if (streamingAssetsGameDefinitionMode)
				defines.Add("STREAMING_ASSETS_GAME_DEFINITIONS");
			else
				defines.Remove("STREAMING_ASSETS_GAME_DEFINITIONS");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(buildPlayerOptions.targetGroup, string.Join(", ", defines));
			BuildPipeline.BuildPlayer(buildPlayerOptions);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(buildPlayerOptions.targetGroup, oldDefines);
		}
		
		static bool ValidateAssetBundleManifestFileExists(string manifestPath) {
			return File.Exists(manifestPath) || EditorUtility.DisplayDialog("Warning", "It is recommended to build Asset Bundles first, using the GameDefinitionManager-Window. Continuing without may result in required code being stripped from the engine.", "OK", "Cancel");
		}
	}
}