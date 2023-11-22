using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mediabox.GameKit.Bundles {
	public class UnityBundle : IBundle {
		readonly AssetBundle bundle;

		public async Task<T> LoadAssetAsync<T>(string path) where T: UnityEngine.Object {
			var asset = this.bundle.LoadAssetAsync<T>(path);
			await asset;
			if (asset.asset is GameObject gameObject)
				ReapplyShaders(gameObject);
			return (T) asset.asset;
		}

		public T LoadAsset<T>(string path) where T: UnityEngine.Object {
			var asset = this.bundle.LoadAsset<T>(path);
			if (asset is GameObject gameObject)
				ReapplyShaders(gameObject);
			return asset;
		}

		public UnityBundle(AssetBundle bundle) {
			this.bundle = bundle;
		}

		public void Unload() {
			this.bundle.Unload(true);
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		void ReapplyShadersOnAllLoadedGameObjects()
		{
			foreach (var gameObject in SceneManager.GetActiveScene().GetRootGameObjects())
			{
				ReapplyShaders(gameObject);
			}
		}
		
		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		void ReapplyShaders(GameObject gameObject) {
			var renderers = gameObject.GetComponentsInChildren<Renderer>(true);
			foreach (var rend in renderers) {
				var materials = rend.sharedMaterials;

				foreach (var material in materials) {
					if (material != null) {
						material.shader = Shader.Find(material.shader.name);
					}
				}
			}

#if TMPRO
			var texts = gameObject.GetComponentsInChildren<TMPro.TMP_Text>(true);
			foreach (var text in texts) {
				var materials = text.fontSharedMaterials;

				foreach (var material in materials) {
					if (material != null) {
						material.shader = Shader.Find(material.shader.name);
					}
				}
			}
#endif

			var images = gameObject.GetComponentsInChildren<UnityEngine.UI.Image>(true);
			foreach (var image in images) {
				var material = image.material;
				if (material != null) {
					material.shader = Shader.Find(material.shader.name);
				}
			}
		}

		public async Task LoadScene(string scenePath, LoadSceneMode loadSceneMode = LoadSceneMode.Single) {
			await SceneManager.LoadSceneAsync(scenePath, loadSceneMode);
			ReapplyShadersOnAllLoadedGameObjects();
		}
	}
}