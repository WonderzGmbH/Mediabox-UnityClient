using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Mediabox.GameKit.Bundles {
	public interface IBundle {
		Task<T> LoadAssetAsync<T>(string path) where T: UnityEngine.Object;
		T LoadAsset<T>(string path) where T: UnityEngine.Object;
		void Unload();
		Task LoadScene(string scenePath, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
	}
}