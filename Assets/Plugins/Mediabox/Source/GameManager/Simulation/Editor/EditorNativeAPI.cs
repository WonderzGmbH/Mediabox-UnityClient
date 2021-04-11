using System;
using System.IO;
using Mediabox.GameKit.GameDefinition;
using UnityEditor;

namespace Mediabox.GameManager.Simulation.Editor {
	public class EditorNativeAPI : GameNativeAPI {
		readonly GameDefinitionSettings settings;
		protected override string GameDefinitionDirectoryPath => this.settings.gameDefinitionDirectoryPath;

		public EditorNativeAPI(string bundleName, GameDefinitionSettings settings) : base(bundleName, new EditorGUIDialog()) {
			this.settings = settings;
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

