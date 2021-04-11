using UnityEditor;

namespace Mediabox.GameManager.Simulation.Editor {
	public class UnityEditorPrefs : IPrefs {
		public bool GetBool(string key, bool defaultValue) {
			return EditorPrefs.GetBool(key, defaultValue);
		}

		public void SetBool(string key, bool value) {
			EditorPrefs.SetBool(key, value);
		}

		public string GetString(string key, string defaultValue) {
			return EditorPrefs.GetString(key, defaultValue);
		}

		public void SetString(string key, string value) {
			EditorPrefs.SetString(key, value);
		}
	}
}