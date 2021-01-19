namespace Mediabox.GameKit.GameDefinition {
	public interface IGameBundleSceneDefinition : IGameBundleDefinition, IGameSceneDefinition {
		string[] AdditionalBundles { get; }
	}
}