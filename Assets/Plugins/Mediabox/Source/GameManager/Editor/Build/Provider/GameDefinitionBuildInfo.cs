using Mediabox.GameKit.GameDefinition;

namespace Mediabox.GameManager.Editor.Build.Provider {
	public class GameDefinitionBuildInfo {
		public readonly IGameDefinition gameDefinition;
		public readonly string directory;

		public GameDefinitionBuildInfo(IGameDefinition gameDefinition, string directory) {
			this.gameDefinition = gameDefinition;
			this.directory = directory;
		}
	}
}