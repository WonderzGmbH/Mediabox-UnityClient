using System.Threading.Tasks;
using Mediabox.GameKit.GameManager;
using UnityEngine;

namespace Mediabox.Samples {
	/// <summary>
	/// An implementation of a game manager. Since the base class already handles all necessary scene and bundle loading for us, we didn't have to add any custom logic here.
	/// This class has been added to the project's StartScene. To Ensure that it's loaded after game startup.
	/// Details to this class can be found in the base class.
	/// </summary>
	public class GameManager : GameManagerBase<GameDefinition> {

		protected override Task OnStartGame(string contentBundleFolderPath, GameDefinition definition, string saveGamePath) {
			Debug.Log($"[GameManager] Starting Game: {JsonUtility.ToJson(definition)} at contentBundleFolderPath {contentBundleFolderPath} with saveGamePath {saveGamePath}");
			return Task.CompletedTask;
		}

	#if UNITY_ANDROID

		public UniversalRenderPipelineAsset pipelineAsset = null;

		// Android flicker bug resolution:
		void OnApplicationPause(bool pauseStatus) {
			if (pauseStatus) return;
			if (Time.time == 0) return;
			if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Vulkan)return;
			// Optional additional restriction. So far we have only seen Adreno chips with the error, 
			// but a lot of posts about Mali chips having other flickering bugs, we have opted out of 
			// implementing this restriction in case they are also effected.
			// if (!SystemInfo.graphicsDeviceName.ToLower().Contains("adreno")) return;
			ReflectionCallReleaseTargets();
		}

		void ReflectionCallReleaseTargets() {
			if (pipelineAsset == null) return;
			ScriptableRenderer renderer = pipelineAsset.GetRenderer(-1);
			if (renderer == null) return;
			var methodInfo = typeof(ScriptableRenderer).GetMethod("ReleaseRenderTargets", BindingFlags.Instance | BindingFlags.NonPublic);
			methodInfo?.Invoke(renderer, Array.Empty<object>());
		}		
	#endif
	}

}