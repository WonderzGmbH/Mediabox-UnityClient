using Mediabox.GameKit.GameDefinition;

namespace Mediabox.Samples {
	/// <summary>
	/// This is a sample GameDefinition for your Game.
	/// (optional) Implementing IGameBundleDefinition makes sure that GameManagerBase will automatically load the associated bundle for this game definition.
	/// (optional) Implementing IGameSceneDefinition makes sure that GameManagerBase will automatically load the associated scene for this game definition.
	/// This whole class will always be sent to your GameManager and Game classes when a Game is about to launch.
	/// You can add any kind of custom information here.
	/// You can configure, edit and add definitions in the GameDefinitionManager window.
	/// Details to this class can be found in the base class.
	/// </summary>
	[System.Serializable]
	public class GameDefinition : IGameBundleSceneDefinition, IGameDefinition {
		public string game;
		public string skin;
		public string bundlePath;
		public string scenePath;
		public string BundleName => this.bundlePath;
		public string SceneName => this.scenePath;
		public string[] AdditionalBundles => new string[0];
	}
}