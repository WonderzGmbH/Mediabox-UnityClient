using UnityEngine;

namespace Mediabox.GameKit.GameDefinition {
	public class GameDefinitionSettings : ScriptableObject {
		public const string SettingsPath = "Assets/Resources/GameDefinitionSettings.asset";
		static string SettingsResourcePath => SettingsPath.Replace("Resources/", "").Replace(".asset", "").Replace("Assets/", "");
		public string gameDefinitionDirectoryPath = "./GameDefinitions/";
		public bool useGameDefinitionJsonFile = true;
		public string gameDefinitionFileName = "index.json";

		public static GameDefinitionSettings Load() {
			return Resources.Load<GameDefinitionSettings>(SettingsResourcePath) ?? CreateInstance<GameDefinitionSettings>();
		}
	}
}
