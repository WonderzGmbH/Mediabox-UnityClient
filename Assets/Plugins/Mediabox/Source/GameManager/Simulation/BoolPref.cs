namespace Mediabox.GameManager.Simulation {
	public class BoolPref : IPref<bool> {
		readonly IPrefs prefs;
		readonly string key;
		readonly bool defaultValue;

		public BoolPref(IPrefs prefs, string key, bool defaultValue = default) {
			this.prefs = prefs;
			this.key = key;
			this.defaultValue = defaultValue;
		}

		public bool Value {
			get => this.prefs.GetBool(key, this.defaultValue); 
			set => this.prefs.SetBool(this.key, value);
		}
	}
}