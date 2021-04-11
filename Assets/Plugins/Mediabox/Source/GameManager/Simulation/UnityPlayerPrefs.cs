using UnityEngine;

namespace Mediabox.GameManager.Simulation {
	public class UnityPlayerPrefs : IPrefs {
		public bool GetBool(string key, bool defaultValue) {
			return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;
		}

		public void SetBool(string key, bool value) {
			PlayerPrefs.SetInt(key, value ? 1 : 0);
		}

		public string GetString(string key, string defaultValue) {
			return PlayerPrefs.GetString(key, defaultValue);
		}

		public void SetString(string key, string value) {
			PlayerPrefs.SetString(key, value);
		}
	}
}