using Mediabox.GameManager.Editor.Build.Provider;

namespace Mediabox.GameManager.Editor.Build.Validator {
	public interface IGameDefinitionBuildValidator {
		bool Validate(GameDefinitionBuildInfo gameDefinitionBuildInfo);
	}
}