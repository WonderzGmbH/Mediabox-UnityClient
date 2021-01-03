using Mediabox.GameKit.Bundles;
using UnityEditor;

namespace Mediabox.GameManager.Editor.Build.Plugins {
	public class BundlesPlugin : IGameDefinitionManagerPlugin {
		public string Title => "Bundles";
		public bool ToggleableWithTitleLabel => true;
		public void Update() { }

		public bool Render() {
			BundleManager.UseEditorBundles = EditorGUILayout.Toggle("Use Editor Bundles", BundleManager.UseEditorBundles);
			EditorGUILayout.HelpBox("Using Editor Bundles allows you to test your changes directly in the editor. Disabling this feature requires you (and allows you) to build and test built Bundles.", MessageType.Info);
			return true;
		}
	}
}