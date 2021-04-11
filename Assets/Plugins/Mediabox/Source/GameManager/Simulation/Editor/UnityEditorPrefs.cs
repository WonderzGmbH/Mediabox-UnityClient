using UnityEditor;

namespace Mediabox.GameManager.Simulation.Editor {
	public class UnityEditorPrefs : IPrefs {
		public bool GetBool(string key, bool defaultValue) {
			return EditorPrefs.GetBool(key, defaultValue);
		}

		public string GetString(string key, string defaultValue) {
			return EditorPrefs.GetString(key, defaultValue);
		}
	}
}