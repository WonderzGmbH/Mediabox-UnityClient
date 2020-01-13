using System.Threading.Tasks;
using Mediabox.GameKit.GameManager;
using UnityEngine;

namespace Mediabox.Samples {
	/// <summary>
	/// An implementation of a game manager. Since the base class already handles all necessary scene and bundle loading for us, we didn't have to add any custom logic here.
	/// This class has been added to the project's StartScene. To Ensure that it's loaded after game startup.
	/// Details to this class can be found in the base class.
	/// </summary>
	public class GameManager : GameManagerBase<GameDefinition> {
		protected override Task OnStartGame(string contentBundleFolderPath, GameDefinition definition, string saveGamePath) {
			Debug.Log($"[GameManager] Starting Game: {JsonUtility.ToJson(definition)} at contentBundleFolderPath {contentBundleFolderPath} with saveGamePath {saveGamePath}");
			return Task.CompletedTask;
		}
	}
}