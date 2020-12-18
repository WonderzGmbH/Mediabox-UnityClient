using System.Threading.Tasks;

namespace Mediabox.GameKit.GameManager {
	public interface IGameManager {
		void QuitApplication();
		bool HasContentBundle { get; }
		Task<T> LoadAssetFromContentBundle<T>(string assetPath) where T:UnityEngine.Object;
	}
}