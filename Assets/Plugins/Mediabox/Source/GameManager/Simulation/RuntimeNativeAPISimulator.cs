using UnityEngine;

namespace Mediabox.GameManager.Simulation {
	public class RuntimeNativeAPISimulator : MonoBehaviour {
		
		[HideInInspector]
		public bool streamingAssetsGameDefinitionMode =
#if STREAMING_ASSETS_GAME_DEFINITIONS
            true;
#else
			false;
#endif
		
		public bool show;
		public string bundleName;
		int selectedBundleIndex;
		float windowHeight = 500;
		float windowWidth = 300;
		float windowMinHeight = 25;
		float windowMinWidth = 150;
		Rect windowRect;
		public bool autoSimulate;
		ISimulationNativeAPI nativeApi;
		SimulationModeRunner simulationRunner;

		const string windowPositionX = "Mediabox.GameManager.Simulation.PlayModeNativeAPI.WindowRect.X";
		const string windowPositionY = "Mediabox.GameManager.Simulation.PlayModeNativeAPI.WindowRect.Y";
		const string bundleNameKey = "Mediabox.GameManager.Simulation.PlayModeNativeAPI.BundleName";

		void Awake() {
			if (!streamingAssetsGameDefinitionMode)
				enabled = false;
		}
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
				// Not doing this on start, as it's more recompile-friendly this way.
				this.simulationRunner = new SimulationModeRunner(new UnityPlayerPrefs(), () => true, CreateSimulationNativeAPI);
			}
			this.simulationRunner.AutoSimulate = this.autoSimulate;
			this.simulationRunner.BundleName = this.bundleName;
			this.simulationRunner.Update();
		}

		void OnGUI() {
			var ratio = Mathf.Clamp(Mathf.Min(Screen.width / 600.0f, Screen.height / 500.0f), 0.2f, float.MaxValue);
			var oldMatrix = GUI.matrix;
			GUI.matrix = Matrix4x4.Scale(new Vector3(ratio, ratio, 1f));
			this.windowRect.height = this.show ? this.windowHeight : this.windowMinHeight;
			this.windowRect.width = this.show ? this.windowWidth : this.windowMinWidth;
			this.windowRect.x = Mathf.Clamp(this.windowRect.x, 0f, Screen.width/ratio - this.windowRect.width);
			this.windowRect.y = Mathf.Clamp(this.windowRect.y, 0f, Screen.height/ratio - this.windowRect.height);
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
			if (!this.simulationRunner.IsInSimulationMode) {
				this.simulationRunner.StartSimulationMode();
			}
			this.nativeApi?.OnGUI(this.bundleName);
			GUI.DragWindow();
		}

		void DrawGameDefinitionSelector() {
			if (this.nativeApi?.AllAvailableGameDefinitions?.Length > 0) {
				var definitions = this.nativeApi.AllAvailableGameDefinitions;
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

		ISimulationNativeAPI CreateSimulationNativeAPI() {
			this.nativeApi = new StreamingAssetsNativeAPI(bundleName, new GUIDialog());
			return this.nativeApi;
		}
	}
}