using System.IO;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;

namespace Mediabox.GameManager.Editor.Build.Validator {
	public class TempDirectoryGameDefinitionBuildStep : IGameDefinitionBuildStep {
		readonly GameDefinitionBuildSettings buildSettings;

		public TempDirectoryGameDefinitionBuildStep(GameDefinitionBuildSettings buildSettings) {
			this.buildSettings = buildSettings;
		}

		public void PreProcess() {
			PathHelper.EnsureEmptyDirectory(this.buildSettings.TempGameDefinitionBuildPath);
		}

		public void Execute(BuildTarget buildTarget, GameDefinitionBuildInfo[] gameDefinitions) { }

		public void PostProcess() {
			PathHelper.SafeDeleteDirectory(this.buildSettings.TempGameDefinitionBuildPath);
		}
	}
}