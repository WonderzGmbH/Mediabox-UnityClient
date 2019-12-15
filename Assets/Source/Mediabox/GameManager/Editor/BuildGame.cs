using UnityEditor;

namespace Mediabox.GameManager.Editor {
	public static class BuildGame {
		[MenuItem("MediaBox/Build Game")]
		public static void Build() {
			var buildPlayerOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(new BuildPlayerOptions());
			buildPlayerOptions.assetBundleManifestPath = "AssetBundles/iOS/iOS.manifest";
			BuildPipeline.BuildPlayer(buildPlayerOptions);
		}
	}
}