using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Mediabox.GameManager.Editor {
	[InitializeOnLoad]
	public class LoadDefaultSceneOnPlayMode {

		const string loadFromMainPath = "MediaBox/LoadDefaultScene";
		const string lastScenePath = "Mediabox.GameManager.Editor.LoadDefaultSceneOnPlayMode.lastScenePath";
		public static bool enabled;

		static LoadDefaultSceneOnPlayMode() {
			EditorApplication.playModeStateChanged += PlayModeStateChanged;
			enabled = EditorPrefs.GetBool(loadFromMainPath, false);
		}

		static void PlayModeStateChanged(PlayModeStateChange obj) {
			if (!enabled) {
				ClearPrePlayModeScene();
				return;
			}
			if (!EditorApplication.isPlaying) {
				if (EditorApplication.isPlayingOrWillChangePlaymode) {
					if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
						SavePrePlayModeScene();
						LoadDefaultScene();
					} else {
						EditorApplication.isPlaying = false;
					}
				} else {
					if (HasSavedPrePlayModeScene()) {
						LoadPrePlayModeScene();
					}
				}
			}
		}

		static void LoadPrePlayModeScene() {
			EditorSceneManager.OpenScene(EditorPrefs.GetString(lastScenePath), OpenSceneMode.Single);
			ClearPrePlayModeScene();
		}

		static void ClearPrePlayModeScene() {
			EditorPrefs.DeleteKey(lastScenePath);
		}

		static bool HasSavedPrePlayModeScene() {
			return EditorPrefs.HasKey(lastScenePath);
		}

		static void LoadDefaultScene() {
			if (SceneManager.GetActiveScene().buildIndex != 0) {
				EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path, OpenSceneMode.Single);
			}
		}

		static void SavePrePlayModeScene() {
			EditorPrefs.SetString(lastScenePath, SceneManager.GetActiveScene().path);
		}

		// [MenuItem(loadFromMainPath, priority = 0)]
		// static void ToggleLoadFromMain() {
		// 	enabled = !enabled;
		// 	EditorPrefs.SetBool(loadFromMainPath, enabled);
		// }
		//
		// [MenuItem(loadFromMainPath, true)]
		// static bool ValidateLoadFromMain() {
		// 	Menu.SetChecked(loadFromMainPath, enabled);
		// 	return true;
		// }
	}
}