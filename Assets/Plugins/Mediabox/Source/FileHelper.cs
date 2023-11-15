using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Mediabox {
	public static class FileHelper {
		public static bool Exists(string path)
		{
			if ((Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.WebGLPlayer) || !path.StartsWith(Application.streamingAssetsPath))
			{
				return File.Exists(path);
			}
			UnityWebRequest webRequest = UnityWebRequest.Get(path);
			webRequest.SendWebRequest();
			while (!webRequest.isDone) { }

#if UNITY_2020_1_OR_NEWER || UNITY_2020_OR_NEWER
			return webRequest.result == UnityWebRequest.Result.Success;
#else
			return !webRequest.isNetworkError && !webRequest.isHttpError;
#endif
		}

		public static string ReadAllText(string path)
		{
			if ((Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.WebGLPlayer) || !path.StartsWith(Application.streamingAssetsPath))
			{
				return File.ReadAllText(path);
			}
			UnityWebRequest webRequest = UnityWebRequest.Get(path);
			webRequest.SendWebRequest();
			while (!webRequest.isDone) { }

			return webRequest.downloadHandler.text;
		}
	}
}