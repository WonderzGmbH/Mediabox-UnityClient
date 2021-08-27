namespace Mediabox.GameManager.Simulation {
	public class EnumPref<T> : IPref<T> where T: System.Enum {
		readonly IPrefs prefs;
		readonly string key;
		readonly T defaultValue;

		public EnumPref(IPrefs prefs, string key, T defaultValue = default) {
			this.prefs = prefs;
			this.key = key;
			this.defaultValue = defaultValue;
		}

		public T Value {
			get => this.prefs.GetEnum(key, this.defaultValue); 
			set => this.prefs.SetEnum(this.key, value);
		}
	}
}