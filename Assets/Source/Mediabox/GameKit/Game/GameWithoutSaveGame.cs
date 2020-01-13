using System.Threading.Tasks;

namespace Mediabox.GameKit.Game {
	/// <summary>
	/// Inherit from this class, if you want to define a game without a savegame. Place it in any relevant scene / prefab that is loaded from GameManager.
	/// </summary>
	/// <typeparam name="TGameDefinition">The type of GameDefinition needs to match the one used in your GameManager.</typeparam>
	public abstract class GameWithoutSaveGame<TGameDefinition> : GameBase<TGameDefinition> {
		public override async Task Save(string path) { }
		public override async Task Load(string path) { }
	}
}