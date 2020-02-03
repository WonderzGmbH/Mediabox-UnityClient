using System.Threading.Tasks;
using UnityEngine;

namespace Mediabox.GameKit.Bundles {
	public static class BundleManager {
	
		const string editorBundlesEditorPrefKey = "Mediabox.GameKit.Bundles.EditorBundles";
		static bool _editorBundles;
		public static bool EditorBundles {
        	get => _editorBundles;
        	set {
        	#if UNITY_EDITOR
        		if (value == _editorBundles)
        			return;
        		_editorBundles = value;
        		UnityEditor.EditorPrefs.SetBool(editorBundlesEditorPrefKey, value);
        		#endif
   			}
        }
		
		static BundleManager() {
			#if UNITY_EDITOR
			_editorBundles = UnityEditor.EditorPrefs.GetBool(editorBundlesEditorPrefKey, true);
			#else
			_editorBundles = false;
			#endif
		}
		public static Task<IBundle> Load(string bundleName) {
			return EditorBundles ? LoadEditorBundle(bundleName) : LoadUnityBundle(bundleName);
		}

		static async Task<IBundle> LoadUnityBundle(string bundleName) {
			var bundle = AssetBundle.LoadFromFileAsync(bundleName);
			if (bundle == null) {
				return null;
			}
			await bundle;
			return new UnityBundle(bundle.assetBundle);
		}

		static Task<IBundle> LoadEditorBundle(string bundleName) {
			var bundle = new UnityEditorBundle(bundleName);
			return Task.FromResult<IBundle>(bundle);
		}
	}
}