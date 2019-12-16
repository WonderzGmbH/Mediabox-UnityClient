using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor
{
    public static class BuildGame
    {
        [MenuItem("MediaBox/Build Game")]
        public static void Build()
        {
            BuildPlayerOptions buildPlayerOptions;
            try
            {
                buildPlayerOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(new BuildPlayerOptions());
            }
            catch (UnityEditor.BuildPlayerWindow.BuildMethodException e)
            {
                Debug.Log("BuildMethodException caught: " + e);
                return;
            }
            buildPlayerOptions.assetBundleManifestPath = "AssetBundles/iOS/iOS.manifest";
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}