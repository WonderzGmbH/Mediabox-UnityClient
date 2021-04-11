using System.IO;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Build.Provider;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;

namespace Mediabox.GameManager.Editor.Build.BuildStep {
	public class ArchiveGameDefinitionBuildStep : IGameDefinitionBuildStep {
		readonly GameDefinitionSettings settings;
		readonly GameDefinitionBuildSettings buildSettings;
		readonly bool clearDirectory;

		public ArchiveGameDefinitionBuildStep(GameDefinitionSettings settings, GameDefinitionBuildSettings buildSettings, bool clearDirectory) {
			this.settings = settings;
			this.buildSettings = buildSettings;
			this.clearDirectory = clearDirectory;
		}

		public void PreProcess() { }

		public void Execute(BuildTarget buildTarget, GameDefinitionBuildInfo[] gameDefinitions) {
			var buildDirectory = Path.Combine(this.buildSettings.gameDefinitionBuildPath, buildTarget.ToString());
			if (this.clearDirectory)
				PathUtility.DeleteDirectoryIfExists(buildDirectory);
			PathUtility.EnsureDirectory(buildDirectory);
			foreach (var gameDefinition in gameDefinitions) {
				ZipGameDefinition(gameDefinition, buildDirectory);
			}
		}

		public void PostProcess() {
		}

		void ZipGameDefinition(GameDefinitionBuildInfo gameDefinition, string buildPath) {
			var relativePath = PathUtility.GetRelativePath(gameDefinition.directory, this.settings.gameDefinitionDirectoryPath);
			var bundlePath = Path.ChangeExtension(Path.Combine(buildPath, relativePath), "zip");
			PathUtility.DeleteFileIfExists(bundlePath);
			var customPlatformSettingsPath = Path.Combine(gameDefinition.directory, GameDefinitionBuildSettings.customPlatformSettings);
			PathUtility.ZipDirectoryWithExcludeFile(gameDefinition.directory, bundlePath, customPlatformSettingsPath, this.buildSettings.TempGameDefinitionBuildPath);
		}
	}
}