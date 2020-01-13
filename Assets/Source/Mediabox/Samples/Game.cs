using System.Threading.Tasks;
using Mediabox.GameKit.Game;
using UnityEngine;

namespace Mediabox.Samples {
	/// <summary>
	/// A sample implementation of the game class. This behaviour should be placed in every game scene that's loaded and marks a full game.
	/// This sample shows how to load and save save games.
	/// Details to this class can be found in the base class.
	/// </summary>
	public class Game : GameWithSaveGame<GameDefinition, SaveGame> {
		int level;
		public override async Task StartGame(string contentBundleFolderPath, GameDefinition gameDefinition) {
			Debug.Log("[Game] Started.");
		}

		public override async Task SetLanguage(string language) {
			Debug.Log($"[Game] Set language to {language}.");
		}

		protected override string SaveGameName => "save.json";
		protected override SaveGame CreateSaveGame() {
			return new SaveGame {
				level = ++this.level
			};
		}

		protected override async Task LoadSaveGame(SaveGame saveGame) {
			this.level = saveGame.level;
		}
	}
}