using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Mediabox.GameKit.GameManager;
using Mediabox.GameKit.Pause;
using Mediabox.GameKit.Pause.Actions;
using UnityEngine;

namespace Mediabox.GameKit.Game {
	public abstract class GameBase<TGameDefinition> : MonoBehaviour, IGame<TGameDefinition> {
		string _savePath;
		private float _score;
		public IGameAPI API { get; private set; }
		protected string GetSavePath(string path) => Path.Combine(this._savePath, path);
		public IPauseSynchronizationService pauseSynchronizationService;
		private readonly HashSet<IPauseAction> _pauseActions = new HashSet<IPauseAction>();

		protected virtual IEnumerable<IPauseAction> CreatePauseActions()
		{
			yield return new TimePauseActionWithState();
			yield return new VolumePauseActionWithState();
		}

		public void Initialize(IGameAPI gameAPI, IPauseSynchronizationService pauseSynchronizationService)
		{
			this.API = gameAPI;
			this.pauseSynchronizationService = pauseSynchronizationService;
			var pauseActions = CreatePauseActions();
			foreach (var pauseAction in pauseActions)
			{
				pauseSynchronizationService.AddPauseAction(pauseAction);
				this._pauseActions.Add(pauseAction);
			}
		}

		protected virtual void OnDestroy()
		{
			foreach (var pauseAction in this._pauseActions)
			{
				pauseSynchronizationService.RemovePauseAction(pauseAction);
			}

			this._pauseActions.Clear();
		}
		
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
		/// This will change the score of the game.
		/// Note: If you want the score to persist and to be reported to Mediabox as well, use <see cref="API"/>'s member <see cref="IGameAPI.ReportNewUserScore"/> instead.
		/// </summary>
		/// <param name="score">The new score</param>
		/// <returns>A Task that can be awaited for.</returns>
		public virtual Task SetScore(float score)
		{
			this._score = score;
			return Task.CompletedTask;
		}

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

		public float Score => this._score;

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