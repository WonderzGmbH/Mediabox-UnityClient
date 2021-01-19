using System.Threading.Tasks;

namespace Mediabox.GameKit.Game {
	/// <summary>
	/// Inherit from this class, if you want to define a game without a savegame. Place it in any relevant scene / prefab that is loaded from GameManager.
	/// </summary>
	/// <typeparam name="TGameDefinition">The type of GameDefinition needs to match the one used in your GameManager.</typeparam>
	public abstract class GameWithoutSaveGame<TGameDefinition> : GameBase<TGameDefinition> {
		public override Task Save(string path) => Task.CompletedTask;
		public override Task Load(string path) => Task.CompletedTask;
	}
}