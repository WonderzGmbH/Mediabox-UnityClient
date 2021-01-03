using Mediabox.GameKit.GameDefinition;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build.Validator {
	public class BundleIsSpecifiedGameDefinitionBuildValidator : IGameDefinitionBuildValidator {
		public bool Validate(GameDefinitionBuildInfo gameDefinitionBuildInfo) {
			var gameDefinition = gameDefinitionBuildInfo.gameDefinition;
			if (gameDefinition is IGameBundleDefinition gameBundleDefinition && string.IsNullOrEmpty(gameBundleDefinition.BundleName)) {
				Debug.LogError($"Invalid GameDefinition at path '{gameDefinitionBuildInfo.directory}', no bundle name specified.");
				return false;
			}

			return true;
		}
	}
}