using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Mediabox {
    public static class DirectoryHelper {
		
        public static bool Exists(string path)
        {
            if ((Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.WebGLPlayer) || !path.StartsWith(Application.streamingAssetsPath))
            {
                return File.Exists(path);
            }
            UnityWebRequest webRequest = UnityWebRequest.Get(path);
            webRequest.SendWebRequest();
            while (!webRequest.isDone) { }

            return true; // TODO: FIX!!
            /*
#if UNITY_2020_1_OR_NEWER || UNITY_2020_OR_NEWER
            return webRequest.result == UnityWebRequest.Result.Success;
#else
            return !webRequest.isNetworkError && !webRequest.isHttpError;
#endif
*/
        }

        public static IEnumerable<string> GetDirectories(string path)
        {
            if ((Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.WebGLPlayer) || !path.StartsWith(Application.streamingAssetsPath))
            {
                return new DirectoryInfo(path).GetDirectories().Select(it => it.Name);
            }

            DirectoryInfo info = default;
            UnityWebRequest webRequest = UnityWebRequest.Get(path);
            webRequest.SendWebRequest();
            while (!webRequest.isDone) { }

            string[] lines = webRequest.downloadHandler.text.Split('\n');

            Debug.LogError("Response: " + webRequest.downloadHandler.text);
			
            return lines.Select(it => it.Trim()).Where(it => !string.IsNullOrEmpty(it));
        }
    }
}