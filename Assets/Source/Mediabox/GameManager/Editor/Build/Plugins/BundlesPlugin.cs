using Mediabox.GameKit.Bundles;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build.Plugins {
	public class BundlesPlugin : IGameDefinitionManagerPlugin {
		public string Title => "Bundles";
		public void Update() {
		}

		public bool Render() {
			BundleManager.ReleaseBundles = EditorGUILayout.Toggle("Use Editor Bundles", BundleManager.ReleaseBundles);
			EditorGUILayout.HelpBox("Using Editor Bundles allows you to test your changes directly in the editor. Disabling this feature requires (and allows you) to build and test built Bundles.", MessageType.Info);
			return true;
		}
	}
}