using System.IO;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build {
	public class GameDefinitionBuildSettings : ScriptableObject {
		const string PathWithinResources = "GameDefinitionBuildSettings.asset";
		public const string SettingsPath = "Assets/Plugins/Mediabox/Editor/Resources/"+PathWithinResources;
		public const string customPlatformSettings = "customPlatforms.json";
		
		public BuildTarget[] supportedBuildTargets = {BuildTarget.Android, BuildTarget.iOS};
		public string gameDefinitionBuildPath = "GameDefinitionBuild";
		public string TempGameDefinitionBuildPath => Path.Combine(this.gameDefinitionBuildPath, "_TMP_");
		public string assetBundleBuildPath = "AssetBundles";
		public string tempSimulationBuildPath = "SimulationBuildCache";
		public BuildAssetBundleOptions buildAssetBundleOptions;

		public string GetAssetBundleBuildPath(BuildTarget buildTarget)
			=> Path.Combine(this.assetBundleBuildPath, buildTarget.ToString());

		public string GetAssetBundleManifestPath(BuildTarget buildTarget)
			=> Path.ChangeExtension(Path.Combine(GetAssetBundleBuildPath(buildTarget), buildTarget.ToString()), "manifest");
		
		static string SettingsResourcePath => Path.ChangeExtension(PathWithinResources, null);
	}
}