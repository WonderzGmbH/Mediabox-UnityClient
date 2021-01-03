using System.Threading.Tasks;
using UnityEngine;

namespace Mediabox.GameKit.Bundles {
	public static class BundleManager {
	
		const string releaseBundlesEditorPrefKey = "Mediabox.GameKit.Bundles.UseEditorBundles";
		static bool _useEditorBundles;
		public static bool UseEditorBundles {
        	get => _useEditorBundles;
        	set {
#if UNITY_EDITOR
        		if (value == _useEditorBundles)
        			return;
                _useEditorBundles = value;
        		UnityEditor.EditorPrefs.SetBool(releaseBundlesEditorPrefKey, value);
#endif
   			}
        }
		
		static BundleManager() {
#if UNITY_EDITOR
			_useEditorBundles = UnityEditor.EditorPrefs.GetBool(releaseBundlesEditorPrefKey, true);
#else
			_useEditorBundles = false;
#endif
		}
		public static Task<IBundle> Load(string bundleName) {
			return UseEditorBundles ? LoadEditorBundle(bundleName) : LoadUnityBundle(bundleName);
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