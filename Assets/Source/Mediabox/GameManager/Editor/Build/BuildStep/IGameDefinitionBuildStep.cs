using UnityEditor;

namespace Mediabox.GameManager.Editor.Build.Validator {
	public interface IGameDefinitionBuildStep {
		void PreProcess();
		void Execute(BuildTarget buildTarget, GameDefinitionBuildInfo[] gameDefinitions);
		void PostProcess();
	}
}