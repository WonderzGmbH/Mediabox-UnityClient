using System;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Simulation.Editor {
	public class EditorGUIDialog : IDialog {
		public void Show(string title, string description, string ok, string cancel, Action<bool> callback = null) {
			throw new NotImplementedException();
		}

		public void Show(string title, string description, Action<string> callback = null, params string[] options) {
			switch (options.Length) {
				case 0:
					EditorUtility.DisplayDialog(title, description, "OK");
					break;
				case 1:
					EditorUtility.DisplayDialog(title, description, options[0]);
					callback?.Invoke(options[0]);
					break;
				case 2:
					if (EditorUtility.DisplayDialog(title, description, options[0], options[1]))
						callback?.Invoke(options[0]);
					else
						callback?.Invoke(options[1]);
					break;
				default:
					if (options.Length > 3) {
						Debug.LogWarning($"More than 3 options are currently not supported by {this}. Omitting any further option.");
					}
					var result = EditorUtility.DisplayDialogComplex(title, description, options[0], options[1], options[2]);
					callback?.Invoke(options[result]);
					break;
			}
		}

		public void Update() {}
	}
}