using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build.Plugins {
	public class TitlePlugin : IGameDefinitionManagerPlugin {
		readonly string title;
		readonly GUIStyle style;
		const int fontSize = 20;
		public string Title => $"Title ({this.title})";
		public bool ToggleableWithTitleLabel => false;

		public TitlePlugin(string title) {
			this.title = title;
			this.style = new GUIStyle(GUI.skin.label) {
				fontSize = fontSize, 
				fontStyle = FontStyle.Bold
			};
		}

		public void Update() {
		}

		public bool Render() {
			EditorGUILayout.LabelField(this.title, this.style, GUILayout.MinHeight(fontSize));
			return true;
		}
	}
}