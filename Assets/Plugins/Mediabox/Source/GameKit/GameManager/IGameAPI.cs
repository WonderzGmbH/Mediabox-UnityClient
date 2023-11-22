using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Mediabox.GameKit.GameManager {
	public interface IGameAPI {
		void Quit();
		void ReportNewUserScore(float newScore);
		Task<T> LoadAssetFromContentBundle<T>(string assetPath) where T:UnityEngine.Object;
		Task LoadSceneFromContentBundle<T>(string assetPath, LoadSceneMode loadSceneMode = LoadSceneMode.Single) where T:UnityEngine.Object;
	}
}