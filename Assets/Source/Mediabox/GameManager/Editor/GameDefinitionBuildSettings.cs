using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor {
	public class GameDefinitionBuildSettings : ScriptableObject {
		public const string SettingsPath = "Assets/Editor/Resources/GameDefinitionSettings.asset";
		const string resourcePath = "/Resources/";
		static string SettingsResourcePath => Path.ChangeExtension(SettingsPath, null).Substring(SettingsPath.IndexOf(resourcePath, StringComparison.Ordinal) + resourcePath.Length);
		public const string customPlatformSettings = "customPlatforms.json";
		public BuildTarget[] supportedBuildTargets = {BuildTarget.Android, BuildTarget.iOS};
	}
}