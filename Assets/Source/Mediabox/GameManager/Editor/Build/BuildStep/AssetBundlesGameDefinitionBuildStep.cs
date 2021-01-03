using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Build.Provider;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;

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
			
			var bundleBuildDirectory = Path.Combine(this.buildSettings.assetBundleBuildPath, buildTarget.ToString());
			if (!PathHelper.SafeCreateDirectory(bundleBuildDirectory))
				RepairBundleConflicts(gameDefinitions, this.buildSettings.assetBundleBuildPath);
			BuildPipeline.BuildAssetBundles(bundleBuildDirectory, CreateAssetBundleBuilds(gameDefinitions), BuildAssetBundleOptions.None, buildTarget);
			foreach (var gameDefinition in gameDefinitions) {
				var bundleName = (gameDefinition.gameDefinition as IGameBundleDefinition).BundleName;
				PathHelper.SafeCopyFile(bundleName, bundleBuildDirectory, gameDefinition.directory);
				this.builtBundlePaths.Add(Path.Combine(gameDefinition.directory, bundleName));
			}
		}

		public void PostProcess() {
			foreach (var bundlePath in this.builtBundlePaths) {
				PathHelper.SafeDeleteFile(bundlePath);
			}
		}

		static GameDefinitionBuildInfo[] FilterSupportedGameDefinitions(GameDefinitionBuildInfo[] gameDefinitions) {
			return gameDefinitions.Where(definition => definition.gameDefinition is IGameBundleDefinition).ToArray();
		}

		static AssetBundleBuild[] CreateAssetBundleBuilds(GameDefinitionBuildInfo[] gameDefinitions) {
			return gameDefinitions.Select(definition => new AssetBundleBuild {
				assetBundleName = (definition.gameDefinition as IGameBundleDefinition).BundleName,
				assetNames = AssetDatabase.GetAssetPathsFromAssetBundle((definition.gameDefinition as IGameBundleDefinition).BundleName)
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
					PathHelper.SafeDeleteFile(assetBundleBuildPath);
					PathHelper.SafeDeleteFile(definitionPath);
				}
			}
		}
	}
}