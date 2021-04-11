using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mediabox.GameManager.Simulation {
	public class GUIDialog : IDialog {
		interface IDialog {
			bool Display();
		}
		class YesNoDialog : IDialog {
			readonly string title;
			readonly string text;
			readonly string ok;
			readonly string cancel;
			readonly Action<bool> callback;

			public YesNoDialog(string title, string text, string ok, string cancel, Action<bool> callback) {
				this.title = title;
				this.text = text;
				this.ok = ok;
				this.cancel = cancel;
				this.callback = callback;
			}

			public bool Display() {
				if(this.title != null)
					GUILayout.Label(title);
				if(this.text != null)
					GUILayout.Label(text);
				if (this.ok != null && GUILayout.Button(this.ok) && Event.current.type != EventType.Layout) {
					this.callback?.Invoke(true);
					return true;
				}
				if (this.cancel != null && GUILayout.Button(this.cancel) && Event.current.type != EventType.Layout) {
					this.callback?.Invoke(false);
					return true;
				}
				return false;
			}
		}
		class ComplexDialog : IDialog {
			readonly string title;
			readonly string text;
			readonly string[] options;
			readonly string ok;
			readonly string cancel;
			readonly Action<string> callback;

			public ComplexDialog(string title, string text, string[] options, Action<string> callback) {
				this.title = title;
				this.text = text;
				this.options = options;
				this.callback = callback;
			}

			public bool Display() {
				if(this.title != null)
					GUILayout.Label(title);
				if(this.text != null)
					GUILayout.Label(text);
				foreach (var option in this.options) {
					if(GUILayout.Button(option) && Event.current.type != EventType.Layout) {
						this.callback?.Invoke(option);
						return true;
					}
				}
				return false;
			}
		}

		readonly Queue<IDialog> dialogs = new Queue<IDialog>();
		IDialog activeYesNoDialog;

		public void Show(string title, string description, string ok, string cancel, Action<bool> callback = null) {
			this.dialogs.Enqueue(new YesNoDialog(title, description, ok, cancel, callback));
		}
		
		public void Show(string title, string description, Action<string> callback = null, params string[] options) {
			if (options.Length == 0)
				options = new[] {"OK"};
			this.dialogs.Enqueue(new ComplexDialog(title, description, options, callback));
		}

		public void Update() {
			if (this.activeYesNoDialog == null) {
				if (this.dialogs.Count == 0)
					return;
				this.activeYesNoDialog = this.dialogs.Dequeue();
			}

			if (this.activeYesNoDialog.Display()) {
				this.activeYesNoDialog = null;
			}
		}
	}
}

