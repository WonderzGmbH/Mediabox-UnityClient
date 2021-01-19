using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Mediabox.GameKit.GameManager {
	public interface IGameManager {
		void QuitApplication();
		bool HasContentBundle { get; }
		Task<T> LoadAssetFromContentBundle<T>(string assetPath) where T:UnityEngine.Object;
		Task LoadSceneFromContentBundle<T>(string assetPath, LoadSceneMode loadSceneMode = LoadSceneMode.Single) where T:UnityEngine.Object;
	}
}