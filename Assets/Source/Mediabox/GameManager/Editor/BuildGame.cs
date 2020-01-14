using System.IO;
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

            if (!File.Exists("AssetBundles/AssetBundles.manifest") && !EditorUtility.DisplayDialog("Warning", "It is recommended to build Asset Bundles first, using the GameDefinitionManager-Window. Continuing without may ", "OK", "Cancel"))
                return;
            buildPlayerOptions.assetBundleManifestPath = "AssetBundles/AssetBundles.manifest";
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}