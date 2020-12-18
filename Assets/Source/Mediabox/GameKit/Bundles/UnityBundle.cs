using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mediabox.GameKit.Bundles {
	public class UnityBundle : IBundle {
		readonly AssetBundle bundle;

		public async Task<T> LoadAssetAsync<T>(string path) where T: UnityEngine.Object {
			var asset = this.bundle.LoadAssetAsync<T>(path);
			await asset;
			return (T) asset.asset;
		}

		public T LoadAsset<T>(string path) where T: UnityEngine.Object {
			return this.bundle.LoadAsset<T>(path);
		}

		public UnityBundle(AssetBundle bundle) {
			this.bundle = bundle;
		}

		public void Unload() {
			this.bundle.Unload(true);
		}

		public async Task LoadScene(string scenePath, LoadSceneMode loadSceneMode = LoadSceneMode.Single) {
			await SceneManager.LoadSceneAsync(scenePath, loadSceneMode);
		}
	}
}