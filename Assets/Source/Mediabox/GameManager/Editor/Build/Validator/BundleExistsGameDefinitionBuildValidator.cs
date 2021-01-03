using System.Linq;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Build.Provider;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build.Validator {
	public class BundleExistsGameDefinitionBuildValidator : IGameDefinitionBuildValidator {
		public bool Validate(GameDefinitionBuildInfo gameDefinitionBuildInfo) {
			var gameDefinition = gameDefinitionBuildInfo.gameDefinition;
			if (gameDefinition is IGameBundleDefinition gameBundleDefinition) {
				if (!AssetDatabase.GetAllAssetBundleNames().Contains(gameBundleDefinition.BundleName)) {
					Debug.LogError($"Invalid GameDefinition at path '{gameDefinitionBuildInfo.directory}', the bundle named {gameBundleDefinition.BundleName} does not exist.");
					return false;
				}
			}

			return true;
		}
	}
}