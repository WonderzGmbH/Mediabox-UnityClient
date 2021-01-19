
using Mediabox.GameManager.Editor.Build.Validator;

namespace Mediabox.GameManager.Editor.Build.Provider {
	public interface IGameDefinitionBuildInfoProvider {
		GameDefinitionBuildInfoResult Provide(string[] directories, string gameDefinitionFileName, System.Type gameDefinitionType, IGameDefinitionBuildValidator[] validators);
	}
}