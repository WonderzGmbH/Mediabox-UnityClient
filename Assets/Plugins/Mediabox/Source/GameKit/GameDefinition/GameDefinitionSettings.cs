using System.IO;
using UnityEngine;

namespace Mediabox.GameKit.GameDefinition {
	public class GameDefinitionSettings : ScriptableObject {
		const string PathWithinResources = "GameDefinitionSettings.asset";
		public const string SettingsPath = "Assets/Plugins/Mediabox/Resources/"+PathWithinResources;
		static string SettingsResourcePath => Path.ChangeExtension(PathWithinResources, null);
		public string gameDefinitionDirectoryPath = "./GameDefinitions/";
		public bool useGameDefinitionJsonFile = true;
		public string gameDefinitionFileName = "index.json";
		public ServerMode ServerMode;

		public static GameDefinitionSettings Load() {
			return Resources.Load<GameDefinitionSettings>(SettingsResourcePath) ?? CreateNewSettings();
		}

		static GameDefinitionSettings CreateNewSettings() {
			Debug.LogWarning($"No {nameof(GameDefinitionSettings)} found in Resource Folder at Path {SettingsResourcePath}. Creating new instance.");
			return CreateInstance<GameDefinitionSettings>();
		}
	}
}