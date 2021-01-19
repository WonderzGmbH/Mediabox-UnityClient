using System.Threading.Tasks;

namespace Mediabox.GameKit.Game {
	public interface IGame<TGameDefinition> {
		/// <summary>
		/// This method is called when the game is supposed to start and it sends all necessary information.
		/// </summary>
		/// <param name="contentBundleFolderPath">The path where all downloaded files will be stored.</param>
		/// <param name="gameDefinition">The game definition file. It may be null, if configured so in GameDefinitionSettings.</param>
		Task StartGame(string contentBundleFolderPath, TGameDefinition gameDefinition);
		/// <summary>
		/// This method will be called on GameStart and whenever the NativeAPI sends an event for language change.
		/// </summary>
		/// <param name="language">The identifier for the new language.</param>
		Task SetLanguage(string language);
		/// <summary>
		/// This method will be called to notify you to save the game progress before quitting the game.
		/// </summary>
		/// <param name="path">The directory in which to store any relevant savegame data.</param>
		Task Save(string path);
		/// <summary>
		/// This method will be called to notify you to save the game progress before quitting the game.
		/// </summary>
		/// <param name="path">The directory in which to store any relevant savegame data.</param>
		Task Load(string path);
	}
}