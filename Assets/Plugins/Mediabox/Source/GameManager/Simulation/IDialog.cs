using System;

namespace Mediabox.GameManager.Simulation {
	public interface IDialog {
		void Show(string title, string description, string ok, string cancel, Action<bool> callback = null);
		void Show(string title, string description, Action<string> callback = null, params string[] options);
		void Update();
	}
}