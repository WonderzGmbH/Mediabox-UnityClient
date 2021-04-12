using System.IO;
using System.Linq;
using UnityEngine;

namespace Mediabox.GameManager.Simulation {
	public class StreamingAssetsNativeAPI : SimulationNativeAPI {
		public StreamingAssetsNativeAPI(string bundleName, IDialog dialog) : base(bundleName, dialog) {
		}

		protected override string GameDefinitionDirectoryPath => Path.Combine(Application.streamingAssetsPath, "GameDefinitions");

		protected override string[] GetAllAvailableGameDefinitions() {
			return new DirectoryInfo(this.GameDefinitionDirectoryPath).GetDirectories().Select(dir => dir.Name).ToArray();
		}

		protected override void StopApplication() {
			Application.Quit();
		}

		protected override void HandleLoadingFailed() {
			// no action required
		}

		protected override void HandleScreenshotUserChoice(string path, ScreenshotUserChoice choice) {
			switch (choice) {
				case ScreenshotUserChoice.Open:
					Application.OpenURL($"file://{Path.GetFullPath(path)}");
					break;
				case ScreenshotUserChoice.Delete:
					File.Delete(path);
					break;
				case ScreenshotUserChoice.Continue:
					break;
				default:
					this.dialog.Show("Not Supported.", $"Option {choice} is not supported at this time.");
					break;
			}
		}

		protected override void ValueTextField(string label, ref string value) {
			GUILayout.BeginHorizontal();
			GUILayout.Label(label);
			value = GUILayout.TextField(value);
			GUILayout.EndHorizontal();
		}
	}
}