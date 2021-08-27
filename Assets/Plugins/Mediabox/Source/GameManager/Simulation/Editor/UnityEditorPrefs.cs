using System;
using UnityEditor;

namespace Mediabox.GameManager.Simulation.Editor {
	public class UnityEditorPrefs : IPrefs {
		public bool GetBool(string key, bool defaultValue)
			=> EditorPrefs.GetBool(key, defaultValue);

		public void SetBool(string key, bool value)
			=> EditorPrefs.SetBool(key, value);

		public string GetString(string key, string defaultValue)
			=> EditorPrefs.GetString(key, defaultValue);

		public void SetString(string key, string value)
			=> EditorPrefs.SetString(key, value);

		public T GetEnum<T>(string key, T defaultValue) where T : Enum
			=> (T)Enum.Parse(typeof(T), EditorPrefs.GetString(key, defaultValue.ToString()));

		public void SetEnum<T>(string key, T value)
			=> EditorPrefs.SetString(key, value.ToString());
	}
}