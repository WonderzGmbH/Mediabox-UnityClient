using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Build.Provider;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build.BuildStep {
	public class AssetBundlesGameDefinitionBuildStep : IGameDefinitionBuildStep {
		readonly GameDefinitionBuildSettings buildSettings;
		HashSet<string> builtBundlePaths;

		public AssetBundlesGameDefinitionBuildStep(GameDefinitionBuildSettings buildSettings) {
			this.buildSettings = buildSettings;
		}

		public void PreProcess() {
			this.builtBundlePaths = new HashSet<string>();
		}

		public void Execute(BuildTarget buildTarget, GameDefinitionBuildInfo[] gameDefinitions) {
			gameDefinitions = FilterSupportedGameDefinitions(gameDefinitions);
			if (gameDefinitions.Length == 0)
				return;

			var bundleBuildDirectory = this.buildSettings.GetAssetBundleBuildPath(buildTarget);
			if (!PathUtility.EnsureDirectory(bundleBuildDirectory))
				RepairBundleConflicts(gameDefinitions, this.buildSettings.GetAssetBundleBuildPath(buildTarget));
			var assetBundleBuilds = CreateAssetBundleBuilds(gameDefinitions);
			foreach (var assetBundleBuild in assetBundleBuilds)
			{
				var invalidAssets = assetBundleBuild.assetNames.Where(assetName => Path.GetExtension(assetName) != ".unity");
				if (invalidAssets.Any())
				{
					Debug.LogError($"ERROR: Assets {string.Join(", ", invalidAssets)} are flagged to be part of Asset Bundle {assetBundleBuild.assetBundleName}. Please remove those. The build will fail in the next step.");
				}
			}
			BuildPipeline.BuildAssetBundles(bundleBuildDirectory, assetBundleBuilds, this.buildSettings.buildAssetBundleOptions, buildTarget);
			foreach (var gameDefinition in gameDefinitions) {
				var bundleName = (gameDefinition.gameDefinition as IGameBundleDefinition).BundleName;
				PathUtility.CopyFileToDirectory(bundleName, bundleBuildDirectory, gameDefinition.directory);
				this.builtBundlePaths.Add(Path.Combine(gameDefinition.directory, bundleName));
			}
		}

		public void PostProcess() {
			foreach (var bundlePath in this.builtBundlePaths) {
				PathUtility.DeleteFileIfExists(bundlePath);
			}
		}

		static GameDefinitionBuildInfo[] FilterSupportedGameDefinitions(GameDefinitionBuildInfo[] gameDefinitions) {
			return gameDefinitions.Where(definition => definition.gameDefinition is IGameBundleDefinition).ToArray();
		}

		static string[] GetAssetNamesForGameDefinition(GameDefinitionBuildInfo gameDefinitionBuildInfo) {
			var assetNames = AssetDatabase.GetAssetPathsFromAssetBundle((gameDefinitionBuildInfo.gameDefinition as IGameBundleDefinition).BundleName);
			if (gameDefinitionBuildInfo.gameDefinition is IGameSceneDefinition gameSceneDefinition) {
				if (!assetNames.Contains(gameSceneDefinition.SceneName)) {
					return assetNames.Concat(new[] { gameSceneDefinition.SceneName }).ToArray();
				}
			}

			return assetNames;
		}

		static AssetBundleBuild[] CreateAssetBundleBuilds(GameDefinitionBuildInfo[] gameDefinitions) {
			return gameDefinitions.Select(definition => new AssetBundleBuild {
				assetBundleName = (definition.gameDefinition as IGameBundleDefinition).BundleName,
				assetNames = GetAssetNamesForGameDefinition(definition)
			}).ToArray();
		}

		static void RepairBundleConflicts(IEnumerable<GameDefinitionBuildInfo> buildInfos, string assetBundleBuildPath) {
			foreach (var buildInfo in buildInfos) {
				var splitPath = (buildInfo.gameDefinition as IGameBundleDefinition).BundleName.Split('/');
				for (var i = 0; i < splitPath.Length - 1; i++) {
					var definitionPath = buildInfo.directory;
					for (var j = 0; j <= i; j++) {
						assetBundleBuildPath = Path.Combine(assetBundleBuildPath, splitPath[j]);
						definitionPath = Path.Combine(definitionPath, splitPath[j]);
					}
					PathUtility.DeleteFileIfExists(assetBundleBuildPath);
					PathUtility.DeleteFileIfExists(definitionPath);
				}
			}
		}
	}
}