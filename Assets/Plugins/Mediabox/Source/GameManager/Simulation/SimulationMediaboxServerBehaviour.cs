using Mediabox.API;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameKit.GameManager;
using UnityEngine;

namespace Mediabox.GameManager.Simulation {
	/// <summary>
	/// Put this script on the same GameObject as your implementation of <see cref="GameManagerBase{TGameDefinition}"/>
	/// </summary>
	public class SimulationMediaboxServerBehaviour : MonoBehaviour, IMediaboxServerFactory {
		static readonly Rect windowDefaultSize = new Rect(150, 25, 150, 475);

		public bool show;
		public string bundleName;
		int selectedBundleIndex;
		Rect windowRect;
		public bool autoSimulate;
		ISimulationMediaboxServer mediaboxServer;
		SimulationModeRunner simulationRunner;
		public int serverPriority;

		const string windowPositionX = "Mediabox.GameManager.Simulation.SimulationMediaboxServerBehaviour.WindowRect.X";
		const string windowPositionY = "Mediabox.GameManager.Simulation.SimulationMediaboxServerBehaviour.WindowRect.Y";
		const string bundleNameKey = "Mediabox.GameManager.Simulation.SimulationMediaboxServerBehaviour.BundleName";

		void Start() {
			this.windowRect.x = PlayerPrefs.GetFloat(windowPositionX, 0f);
			this.windowRect.y = PlayerPrefs.GetFloat(windowPositionY, 0f);
			this.bundleName = PlayerPrefs.GetString(bundleNameKey, string.Empty);
		}

		void OnDestroy() {
			PlayerPrefs.SetFloat(windowPositionX, this.windowRect.x);
			PlayerPrefs.SetFloat(windowPositionY, this.windowRect.y);
			PlayerPrefs.SetString(bundleNameKey, this.bundleName);

			if (this.simulationRunner != null) {
				this.simulationRunner.OnDestroy();
				this.simulationRunner = null;
			}
		}

		void Update() {
			if (this.simulationRunner == null) {
				return;
			}

			this.simulationRunner.AutoSimulate = this.autoSimulate;
			this.simulationRunner.BundleName = this.bundleName;
			this.simulationRunner.Update();
		}

		void OnGUI() {
			if (this.simulationRunner == null) {
				return;
			}
			var ratio = Mathf.Clamp(Mathf.Min(Screen.width / 600.0f, Screen.height / 500.0f), 0.2f, float.MaxValue);
			var oldMatrix = GUI.matrix;
			GUI.matrix = Matrix4x4.Scale(new Vector3(ratio, ratio, 1f));
			this.windowRect.height = this.show ? windowDefaultSize.yMax : windowDefaultSize.yMin;
			this.windowRect.width = this.show ? windowDefaultSize.xMax : windowDefaultSize.xMin;
			this.windowRect.x = Mathf.Clamp(this.windowRect.x, 0f, Screen.width / ratio - this.windowRect.width);
			this.windowRect.y = Mathf.Clamp(this.windowRect.y, 0f, Screen.height / ratio - this.windowRect.height);
			this.windowRect = GUI.Window(0, this.windowRect, OnWindowGUI, "Build Native API");
			GUI.matrix = oldMatrix;
		}

		void OnWindowGUI(int id) {
			var labelRect = new Rect(8, 0, this.windowRect.width - 30, 20);
			var buttonRect = new Rect(this.windowRect.width - 24, 2, 20, 20);
			if (!this.show)
				GUI.Label(labelRect, "Build Native API");

			var buttonText = this.show ? "-" : "+";
			if (GUI.Button(buttonRect, buttonText))
				this.show = !this.show;

			this.autoSimulate = GUILayout.Toggle(this.autoSimulate, "Auto Simulate");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Bundle Name");
			this.bundleName = GUILayout.TextField(this.bundleName);
			GUILayout.EndHorizontal();
			DrawGameDefinitionSelector();
			this.mediaboxServer?.OnGUI(this.bundleName);
			GUI.DragWindow();
		}

		void DrawGameDefinitionSelector() {
			if (this.mediaboxServer?.AllAvailableGameDefinitions?.Length > 0) {
				var definitions = this.mediaboxServer.AllAvailableGameDefinitions;
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("<")) {
					this.selectedBundleIndex--;
				}

				this.selectedBundleIndex %= definitions.Length;
				if (GUILayout.Button(definitions[this.selectedBundleIndex])) {
					this.bundleName = definitions[this.selectedBundleIndex];
				}

				if (GUILayout.Button(">")) {
					this.selectedBundleIndex++;
				}

				GUILayout.EndHorizontal();
			}
		}

		int IMediaboxServerFactory.Priority => this.serverPriority;

		IMediaboxServer IMediaboxServerFactory.Create() {
			if (GameDefinitionSettings.Load().ServerMode != ServerMode.Simulation) {
				Debug.Log($"{nameof(GameDefinitionSettings)}.{nameof(GameDefinitionSettings.ServerMode)} is not {nameof(ServerMode)}.{nameof(ServerMode.Simulation)}. Returning null.");
				return null;
			}

			Debug.Log($"Creating new{nameof(StreamingAssetsMediaboxServer)}");

			if (this.simulationRunner == null) {
				this.simulationRunner = new SimulationModeRunner(new UnityPlayerPrefs(), () => true, () => this.mediaboxServer);
			}

			this.mediaboxServer = new StreamingAssetsMediaboxServer(bundleName, new GUIDialog());
			return this.mediaboxServer;
		}
	}
}