using System;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Utility {
	public class LayoutUtility {
		class HorizontalLayer : IDisposable {
			public void Dispose() {
				GUILayout.EndHorizontal();
			}
		}
		class VerticalLayer : IDisposable {
			public void Dispose() {
				GUILayout.EndVertical();
			}
		}
		class ScrollLayer : IDisposable {
			public void Dispose() {
				GUILayout.EndScrollView();
			}
		}
		static readonly HorizontalLayer horizontalLayer = new HorizontalLayer();
		static readonly VerticalLayer verticalLayer = new VerticalLayer();
		static readonly ScrollLayer scrollLayer = new ScrollLayer();

		public static IDisposable HorizontalStack() {
			GUILayout.BeginHorizontal();
			return horizontalLayer;
		}
		
		public static IDisposable VerticalStack() {
			GUILayout.BeginVertical();
			return verticalLayer;
		}

		public static IDisposable VerticalStack(GUIStyle style, params GUILayoutOption[] options) {
			GUILayout.BeginVertical(style, options);
			return verticalLayer;
		}

		public static IDisposable ScrollStack(ref Vector2 position) {
			position = GUILayout.BeginScrollView(position, false, false);
			return scrollLayer;
		}
	}
}