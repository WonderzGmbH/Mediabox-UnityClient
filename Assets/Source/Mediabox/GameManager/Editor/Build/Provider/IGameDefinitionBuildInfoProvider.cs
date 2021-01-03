
using Mediabox.GameManager.Editor.Build.Validator;

namespace Mediabox.GameManager.Editor.Build {
	public interface IGameDefinitionBuildInfoProvider {
		GameDefinitionBuildInfoResult Provide(string[] directories, string gameDefinitionFileName, IGameDefinitionBuildValidator[] validators);
	}
}