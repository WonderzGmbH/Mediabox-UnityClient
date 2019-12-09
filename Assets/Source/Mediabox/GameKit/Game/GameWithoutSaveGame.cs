namespace Mediabox.GameKit.Game {
	/// <summary>
	/// Inherit from this class, if you want to define a game without a savegame. Place it in any relevant scene / prefab that is loaded from GameManager.
	/// </summary>
	/// <typeparam name="TGameDefinition">The type of GameDefinition needs to match the one used in your GameManager.</typeparam>
	public abstract class GameWithoutSavegame<TGameDefinition> : GameBase<TGameDefinition> {
		public override void Save(string path) { }
		public override void Load(string path) { }
	}
}