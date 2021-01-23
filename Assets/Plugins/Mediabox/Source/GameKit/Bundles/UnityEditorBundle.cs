using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AsyncAwaitUtil;
using UnityEngine.SceneManagement;

namespace Mediabox.GameKit.Bundles {
	public class UnityEditorBundle : IBundle {
		
		class IgnoreCaseAssetsAndFileExtensionStringComparer : StringComparer {
			static readonly int[] charValues = {
				0,0,0,0,0, // 0
				0,0,0,0,0, // 5
				0,0,0,0,0, // 10
				0,0,0,0,0, // 15
				0,0,0,0,0, // 20
				0,0,0,0,0, // 25
				0,0,32,0,0, // 30
				0,0,0,0,0, // 35
				0,0,0,0,0, // 40
				45,46,47,48,49, // 45 -./01
				50,51,52,53,54, // 50
				55,56,57,0,0, // 55
				0,0,0,0,0, // 60
				1,2,3,4,5, // 65 A-E
				6,7,8,9,10, // 70
				11,12,13,14,15, // 75
				16,17,18,19,20, // 80
				21,22,23,24,25, // 85
				26,0,0,0,0, // 90
				0,0,1,2,3, // 95
				4,5,6,7,8, // 100
				9,10,11,12,13, // 105
				14,15,16,17,18, // 110
				19,20,21,22,23, // 115
				24,25,26,0,0, // 120
				0,0,0,0,0, // 125
			};
			const string ASSETS = "Assets/";
			const int ASSETS_LENGTH = 7;
			public override int Compare(string x, string y) {
				var xLength = x.Length;
				var yLength = y.Length;
				var startX = 0;
				var startY = 0;
				if (CompareOrdinalIgnoreCase(x, 0, ASSETS, 0, ASSETS_LENGTH)) {
					startX = ASSETS_LENGTH;
				}

				if (CompareOrdinalIgnoreCase(y, 0, ASSETS, 0, ASSETS_LENGTH)) {
					startY = ASSETS_LENGTH;
				}

				var endX = x.Length - 1;
				for (var i = endX; i >= startX; i--) {
					if (x[i] == '.') {
						endX = i - 1;
					} else if (x[i] == '/') {
						break;
					}
				}

				var endY = y.Length - 1;
				for (var i = endY; i >= startY; i--) {
					if (y[i] == '.') {
						endY = i - 1;
					} else if (y[i] == '/') {
						break;
					}
				}

				var deltaLength = endX - startX - endY + startY;
				var length = deltaLength < 0 ? endX - startX : endY - startY;
				var result = string.Compare(x, startX, y, startY, length, StringComparison.OrdinalIgnoreCase);
				if (result == 0) {
					return deltaLength;
				} else {
					return result;
				}
			}

			public override bool Equals(string x, string y) {
				var xLength = x.Length;
				var yLength = y.Length;
				var startX = 0;
				var startY = 0;
				if (CompareOrdinalIgnoreCase(x, 0, ASSETS, 0, ASSETS_LENGTH)) {
					startX = ASSETS_LENGTH;
				}

				if (CompareOrdinalIgnoreCase(y, 0, ASSETS, 0, ASSETS_LENGTH)) {
					startY = ASSETS_LENGTH;
				}

				var endX = x.Length - 1;
				for (var i = endX; i >= startX; i--) {
					if (x[i] == '.') {
						endX = i - 1;
					} else if (x[i] == '/') {
						break;
					}
				}

				var endY = y.Length - 1;
				for (var i = endY; i >= startY; i--) {
					if (y[i] == '.') {
						endY = i - 1;
					} else if (y[i] == '/') {
						break;
					}
				}

				var deltaLength = endX - startX - endY + startY;
				if (deltaLength != 0)
					return false;
				var length = deltaLength < 0 ? endX - startX + 1 : endY - startY + 1;
				var result = string.Compare(x, startX, y, startY, length, StringComparison.OrdinalIgnoreCase);
				return result == 0;
			}

			static bool CompareOrdinalIgnoreCase(string strA, int indexA, string strB, int indexB, int length) {
				if (strA.Length < indexA+length)
					return false;
				if (strB.Length < indexB+length)
					return false;
				for (var i = 0; i < length; i++) {
					if (charValues[strA[indexA + i]] != charValues[strB[indexB + i]])
						return false;
				}

				return true;
			}
			
			public override int GetHashCode(string x) {
				var startX = 0;
				if (CompareOrdinalIgnoreCase(x, 0, ASSETS, 0, ASSETS_LENGTH)) {
					startX = ASSETS_LENGTH;
				}
				var endX = x.Length;
				if (endX > 0) {
					for (var i = endX - 1; i >= startX; i--) {
						if (x[i] == '.') {
							endX = i;
						} else if (x[i] == '/') {
							break;
						}
					}
				}

				var result = 0;
				for (var i = startX; i < endX; i++) {
					result *= 31;
					result += charValues[x[i]];
				}

				return result;
			}
		}
		static readonly StringComparer assetPathComparer = new IgnoreCaseAssetsAndFileExtensionStringComparer();
		readonly string[] assets;

		string[] FindFullNames(string name) {
			/* search assets */
			var names = new List<string>();
			foreach (var asset in this.assets) {
				if (assetPathComparer.Equals(asset, name)) {
					names.Add(asset);
				}
			}
			return names.Count > 0 ? names.ToArray() : new string[0];
		}
		
		public Task<T> LoadAssetAsync<T>(string path) where T: UnityEngine.Object {
			return Task.FromResult(LoadAsset<T>(path));
		}

		public T LoadAsset<T>(string path) where T: UnityEngine.Object {
			var names = FindFullNames(path);
			foreach (var fullName in names) {
				#if UNITY_EDITOR
				var asset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(fullName);
				if (asset is UnityEditor.SceneAsset || asset is UnityEditor.DefaultAsset)
					continue;
				#else
				var asset = default(UnityEngine.Object);
				#endif
				if (asset == null)
					continue;
				return (T) asset;
			}
			return null;
		}

		public UnityEditorBundle(string name) {
			#if UNITY_EDITOR
			this.assets = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(name);
			#else
			this.assets = default;
			throw new System.Exception("SimulationBundles are only supported in Editor Mode.");
			#endif
		}

		public void Unload() {
		}

		public async Task LoadScene(string scenePath, LoadSceneMode loadSceneMode = LoadSceneMode.Single) {
			#if UNITY_EDITOR
			var asset = UnityEditor.AssetDatabase.LoadAssetAtPath(scenePath,typeof(UnityEngine.Object));
			if(asset == null)
				throw new Exception("No asset exists at path "+scenePath);
			var loadScene = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(loadSceneMode));
			#else
			var loadScene = default(UnityEngine.AsyncOperation);
			#endif
			loadScene.allowSceneActivation = true;
			await loadScene;
			var sceneLoaded = false;
			while (!sceneLoaded) {
				var scene = SceneManager.GetSceneByPath(scenePath);
				if (!scene.IsValid()) {
					throw new Exception("Scene at path "+scenePath+" could not be loaded.");
				}
				sceneLoaded = scene.isLoaded;
				await Task.Yield();
			}
		}
	}
}