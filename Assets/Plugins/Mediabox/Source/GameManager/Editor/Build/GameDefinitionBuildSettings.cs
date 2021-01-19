using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build {
	public class GameDefinitionBuildSettings : ScriptableObject {
		public const string SettingsPath = "Assets/Plugins/Mediabox/Editor/Resources/GameDefinitionSettings.asset";
		public const string customPlatformSettings = "customPlatforms.json";
		
		public BuildTarget[] supportedBuildTargets = {BuildTarget.Android, BuildTarget.iOS};
		public string gameDefinitionBuildPath = "GameDefinitionBuild";
		public string TempGameDefinitionBuildPath => Path.Combine(this.gameDefinitionBuildPath, "_TMP_");
		public string assetBundleBuildPath = "AssetBundles";
		public string tempSimulationBuildPath = "SimulationBuildCache";
		
		const string resourcePath = "/Resources/";
		static string SettingsResourcePath => Path.ChangeExtension(SettingsPath, null).Substring(SettingsPath.IndexOf(resourcePath, StringComparison.Ordinal) + resourcePath.Length);
	}
}