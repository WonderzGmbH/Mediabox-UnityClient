using Mediabox.GameKit.Game;
using UnityEngine;

namespace Mediabox.Samples {
	/// <summary>
	/// A sample implementation of the game class. This behaviour should be placed in every game scene that's loaded and marks a full game.
	/// This sample shows how to load and save save games.
	/// Details to this class can be found in the base class.
	/// </summary>
	public class Game : GameWithSavegame<GameDefinition, Savegame> {
		int level;
		public override void StartGame(string contentBundleFolderPath, GameDefinition gameDefinition) {
			Debug.Log("[Game] Started.");
		}

		public override void SetLanguage(string language) {
			Debug.Log($"[Game] Set language to {language}.");
		}

		protected override string SaveGameName => "save.json";
		protected override Savegame CreateSaveGame() {
			return new Savegame {
				level = ++this.level
			};
		}

		protected override void LoadSaveGame(Savegame savegame) {
			this.level = savegame.level;
		}
	}
}