using Mediabox.GameManager.Editor.Build.Provider;
using UnityEditor;

namespace Mediabox.GameManager.Editor.Build.BuildStep {
	public interface IGameDefinitionBuildStep {
		void PreProcess();
		void Execute(BuildTarget buildTarget, GameDefinitionBuildInfo[] gameDefinitions);
		void PostProcess();
	}
}