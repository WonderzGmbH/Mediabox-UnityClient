using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Mediabox.GameKit.Game {
	public abstract class GameBase<TGameDefinition> : MonoBehaviour, IGame<TGameDefinition> {
		string _savePath;
		protected string GetSavePath(string path) => Path.Combine(this._savePath, path);
		
		/// <summary>
		/// This method is called when the game is supposed to start and it sends all necessary information.
		/// </summary>
		/// <param name="contentBundleFolderPath">The path where all downloaded files will be stored.</param>
		/// <param name="gameDefinition">The game definition file. It may be null, if configured so in GameDefinitionSettings.</param>
		public abstract Task StartGame(string contentBundleFolderPath, TGameDefinition gameDefinition);
		
		/// <summary>
		/// This method will be called on GameStart and whenever the NativeAPI sends an event for language change.
		/// </summary>
		/// <param name="language">The identifier for the new language.</param>
		public abstract Task SetLanguage(string language);
		/// <summary>
		/// This method will be called to notify you to save the game progress before quitting the game.
		/// </summary>
		/// <param name="path">The directory in which to store any relevant savegame data.</param>
		public abstract Task Save(string path);

		/// <summary>
		/// This method will be called to notify you to load the game progress before starting the game.
		/// </summary>
		/// <param name="path">The directory from which to load any relevant savegame data.</param>
		public virtual Task Load(string path) {
			this._savePath = path;
			return Task.CompletedTask;
		}

		public void SaveData<T>(string path, T value) {
			var saveGamePath = GetSavePath(path);
			var directory = Path.GetDirectoryName(saveGamePath);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			File.WriteAllText(saveGamePath, JsonUtility.ToJson(value));
		}

		public bool DataExists(string path) {
			var saveGamePath = GetSavePath(path);
			return File.Exists(saveGamePath);
		}
		
		public T LoadData<T>(string path) {
			var saveGamePath = GetSavePath(path);
			return JsonUtility.FromJson<T>(File.ReadAllText(saveGamePath));
		}

		public void DeleteData(string path) {
			var saveGamePath = GetSavePath(path);
			File.Delete(saveGamePath);
		}
	}
}