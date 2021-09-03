using System;
using UnityEngine;

namespace Mediabox.GameManager.Simulation {
	public class UnityPlayerPrefs : IPrefs {
		public bool GetBool(string key, bool defaultValue)
			=> PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;

		public void SetBool(string key, bool value)
			=> PlayerPrefs.SetInt(key, value ? 1 : 0);

		public string GetString(string key, string defaultValue)
			=> PlayerPrefs.GetString(key, defaultValue);

		public void SetString(string key, string value)
			=> PlayerPrefs.SetString(key, value);

		public T GetEnum<T>(string key, T defaultValue) where T : Enum
			=> (T)Enum.Parse(typeof(T), PlayerPrefs.GetString(key, defaultValue.ToString()));

		public void SetEnum<T>(string key, T value)
			=> PlayerPrefs.SetString(key, value.ToString());
	}
}