namespace Mediabox.GameManager.Simulation {
	public interface IPrefs {
		bool GetBool(string key, bool defaultValue);
		void SetBool(string key, bool value);
		string GetString(string key, string defaultValue);
		void SetString(string key, string value);
		T GetEnum<T>(string key, T defaultValue) where T : System.Enum;
		void SetEnum<T>(string key, T value);
	}
}