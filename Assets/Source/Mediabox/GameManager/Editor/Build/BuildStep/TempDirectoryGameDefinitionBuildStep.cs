using Mediabox.GameManager.Editor.Build.Provider;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;

namespace Mediabox.GameManager.Editor.Build.BuildStep {
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