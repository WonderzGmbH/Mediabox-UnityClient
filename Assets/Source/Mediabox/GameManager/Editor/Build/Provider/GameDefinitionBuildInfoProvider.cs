using System.IO;
using System.Linq;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Build.Validator;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build.Provider {
	public class GameDefinitionBuildInfoProvider<TGameDefinition> : IGameDefinitionBuildInfoProvider
	where TGameDefinition : IGameDefinition {
		public GameDefinitionBuildInfoResult Provide(string[] directories, string gameDefinitionFileName, IGameDefinitionBuildValidator[] validators) {
			var buildInfos = directories.Select(directory => TryLoadGameDefinitionBuildInfo(directory, gameDefinitionFileName)).ToArray();
			var validBuildInfos = buildInfos.Where(buildInfo => ValidateGameDefinitionBuildInfo(buildInfo, validators)).ToArray();
			
			return new GameDefinitionBuildInfoResult {
				hadErrors = validBuildInfos.Length < buildInfos.Length,
				buildInfos = validBuildInfos
			};
		}
		
		/// <summary>
		/// Overload this method in order to inject your own Build Validators
		/// </summary>
		protected virtual IGameDefinitionBuildValidator[] CreateGameDefinitionBuildValidators() {
			return new IGameDefinitionBuildValidator[] {
				new SceneGameDefinitionBuildValidator(),
				new BundleExistsGameDefinitionBuildValidator(),
				new BundleIsSpecifiedGameDefinitionBuildValidator(),
			};
		}

		GameDefinitionBuildInfo TryLoadGameDefinitionBuildInfo(string path, string gameDefinitionFileName) {
			var filePath = Path.Combine(path, gameDefinitionFileName);
			if (!File.Exists(filePath)) {
				Debug.LogError($"Invalid GameDefinition at path '{path}', you can repair it using the Game Definition Manager.");
				return null;
			}

			return new GameDefinitionBuildInfo(JsonUtility.FromJson<TGameDefinition>(File.ReadAllText(filePath)), path);
		}

		static bool ValidateGameDefinitionBuildInfo(GameDefinitionBuildInfo gameDefinitionBuildInfo, IGameDefinitionBuildValidator[] validators) {
			return validators.All(validator => validator.Validate(gameDefinitionBuildInfo));
		}

		static bool ConfirmAllGameDefinitionsValid(GameDefinitionBuildInfo[] gameDefinitions, string[] directories) {
			return gameDefinitions.Length == directories.Length;
		}
	}
}