namespace Mediabox.GameManager.Simulation {
	public interface IPrefs {
		bool GetBool(string key, bool defaultValue);
		string GetString(string key, string defaultValue);
	}
}