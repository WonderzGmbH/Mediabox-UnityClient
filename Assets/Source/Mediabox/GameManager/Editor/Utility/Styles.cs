using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Utility {
	public class Styles {
		
		static GUIStyle _foldoutStyle;
		static GUIStyle[] guiStyleArray;
		
		public static GUIStyle FoldoutStyle {
			get {
				if (_foldoutStyle == null) {
					_foldoutStyle = new GUIStyle(EditorStyles.foldout) {
						fontStyle = FontStyle.Bold
					};
				} 
				return _foldoutStyle;
			}
		}

		public static GUIStyle GetColoredBoxStyle(int count, int index) {
			if (guiStyleArray == null || guiStyleArray.Length != count || (guiStyleArray.Length > 0 && guiStyleArray[0].normal.background == null)) {
				guiStyleArray = new GUIStyle[count];
				for (var i = 0; i < guiStyleArray.Length; ++i) {
					var rgb = Color.HSVToRGB((float) i / count, 0.7f, 1f);
					rgb.a = 0.15f;
					var texture = CreateTexture(2, 2, rgb);
					guiStyleArray[i] = new GUIStyle(GUI.skin.box) {
						normal = {
							background = texture,
							scaledBackgrounds = new []{texture}
						}
					};
				}
			}
			return guiStyleArray[index];
		}

		static Texture2D CreateTexture(int width, int height, Color color) {
			var colors = new Color[width * height];
			for (var index = 0; index < colors.Length; ++index)
				colors[index] = color;
			var texture2D = new Texture2D(width, height);
			texture2D.SetPixels(colors);
			texture2D.Apply();
			return texture2D;
		}
	}
}