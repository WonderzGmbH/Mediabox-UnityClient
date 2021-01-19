using System.IO;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Build.Provider;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build.Validator {
	public class SceneGameDefinitionBuildValidator : IGameDefinitionBuildValidator {
		public bool Validate(GameDefinitionBuildInfo gameDefinitionBuildInfo) {
			var gameDefinition = gameDefinitionBuildInfo.gameDefinition;
			if (gameDefinition is IGameSceneDefinition gameSceneDefinition) {
				var fullScenePath = Path.ChangeExtension(Path.Combine(gameSceneDefinition.SceneName), "unity");
				var scene = (SceneAsset) AssetDatabase.LoadAssetAtPath(fullScenePath, typeof(SceneAsset));
				if (scene == null) {
					Debug.LogError($"Invalid GameDefinition at path '{gameDefinitionBuildInfo.directory}', the scene at path {fullScenePath} does not exist.");
					return false;
				}

				if (gameDefinition is IGameBundleSceneDefinition gameBundleSceneDefinition) {
					var importAsset = AssetImporter.GetAtPath(fullScenePath);
					if (importAsset.assetBundleName != gameBundleSceneDefinition.BundleName) {
						importAsset.SetAssetBundleNameAndVariant(gameBundleSceneDefinition.BundleName, null);
					}
				}
			}

			return true;
		}
	}
}