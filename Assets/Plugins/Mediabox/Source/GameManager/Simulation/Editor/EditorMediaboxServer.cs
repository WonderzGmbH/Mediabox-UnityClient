using System;
using System.IO;
using System.Linq;
using Mediabox.GameKit.GameDefinition;
using UnityEditor;

namespace Mediabox.GameManager.Simulation.Editor {
	/// <summary>
	/// Use this Native API, if you want to simulate the game using raw GameDefinition-Configurations
	/// </summary>
	public class EditorMediaboxServer : SimulationMediaboxServer {
		readonly GameDefinitionSettings settings;
		protected override string GameDefinitionDirectoryPath => this.settings.gameDefinitionDirectoryPath;

		public EditorMediaboxServer(string bundleName, GameDefinitionSettings settings) : base(bundleName, new EditorGUIDialog()) {
			this.settings = settings;
		}
		
		protected override string[] GetAllAvailableGameDefinitions() {
			return new DirectoryInfo(GameDefinitionDirectoryPath).GetDirectories().Select(dir => dir.Name).ToArray();
		}

		protected override void HandleScreenshotUserChoice(string path, ScreenshotUserChoice choice) {
			switch (choice) {
				case ScreenshotUserChoice.Open:
					EditorUtility.RevealInFinder(path);
					break;
				case ScreenshotUserChoice.Delete:
					File.Delete(path);
					break;
				case ScreenshotUserChoice.Continue:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(choice), choice, "Value not expected.");
			}
		}

		protected override void ValueTextField(string label, ref string value) {
			value = EditorGUILayout.TextField(label, value);
		}

		protected override void StopApplication() {
			EditorApplication.ExitPlaymode();
		}

		protected override void HandleLoadingFailed() {
			StopApplication();
		}
	}
}

