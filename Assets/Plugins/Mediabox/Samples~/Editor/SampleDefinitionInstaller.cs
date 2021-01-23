using System;
using System.IO;
using System.Linq;
using Mediabox.GameKit.GameDefinition;
using UnityEditor;

namespace Mediabox.Samples.Editor {
	[InitializeOnLoad]
	public static class SampleDefinitionInstaller {
		static bool HasGameDefinitions {
			get {
				var path = GameDefinitionSettings.Load().gameDefinitionDirectoryPath;
				return Directory.Exists(path) && new DirectoryInfo(path).GetDirectories().Length > 0;
			}
		}

		static bool MakeBackupOfGameDefinitions() {
			var path = GameDefinitionSettings.Load().gameDefinitionDirectoryPath;
			var dateString = DateTime.Now.ToString("yyyyMMdd-hhmmss");
			var backupPath = $"{Path.GetDirectoryName(path)}-{dateString}";
			if (!EditorUtility.DisplayDialog("GameDefinitions already Exist.", $"The sample scripts need to install into your GameDefinitions-Path in order to be executable. Your existing GameDefinitions will be moved to {backupPath} if you confirm. If you cancel, you can find the GameDefinitions in the Sample-Directory and move them manually.", "OK", "Cancel")) {
				return false;
			}

			Directory.Move(path, backupPath);
			return true;
		}

		const string sampleGameDefinitionsDirectoryGUID = "fcf7cdaef5f1f48d59f16112f65bb8a4";
		const string sampleDefinitionsInstallerGUID = "a20b3549f2c04406e9b4147d6d3d78f8";
		const string gameAGUID = "55ec951530e134ec3ad773c17327c81e";
		const string gameBGUID = "650fce85ea96447ed85936ac0c52dba8";
		const string gameASceneGUID = "602359a37ba394666a2e82055cc2b42a";
		const string gameBSceneGUID = "a1bc195cc966d473083779ddcc60c611";
		const string startSceneGUID = "d3fd893bb1dda45fdb74e49aaa202923";
		const string scenePathTag = "${SCENE_PATH}";

		static void UpdateGameDefinitionPaths() {
			File.WriteAllText(AssetDatabase.GUIDToAssetPath(gameAGUID), File.ReadAllText(AssetDatabase.GUIDToAssetPath(gameAGUID)).Replace(scenePathTag, AssetDatabase.GUIDToAssetPath(gameASceneGUID)));
			File.WriteAllText(AssetDatabase.GUIDToAssetPath(gameBGUID), File.ReadAllText(AssetDatabase.GUIDToAssetPath(gameBGUID)).Replace(scenePathTag, AssetDatabase.GUIDToAssetPath(gameBSceneGUID)));
		}
		
		static SampleDefinitionInstaller() {
		    EditorApplication.update += Init;
		}
		
		static void Init(){
		    EditorApplication.update -= Init;
			UpdateGameDefinitionPaths();
			if (!HasGameDefinitions || MakeBackupOfGameDefinitions()) {
				var sampleDefinitionPath = AssetDatabase.GUIDToAssetPath(sampleGameDefinitionsDirectoryGUID);
				var path = GameDefinitionSettings.Load().gameDefinitionDirectoryPath;
				Directory.Move(sampleDefinitionPath, path);
				CleanUpMetaFiles(path);
				File.Delete(Path.ChangeExtension(sampleDefinitionPath, "meta"));
				GameDefinitionHub.OpenWindow();
			}
			if (EditorUtility.DisplayDialog("Change Build Scenes", $"In order for the Simulation Mode to work for these samples, we need to add a new Start-Scene to your Build Settings. Can we proceed?", "OK", "Cancel")) {
			    GUID.TryParse(startSceneGUID, out var parsedStartSceneGUID);
			    EditorBuildSettings.scenes = new[] {new EditorBuildSettingsScene(parsedStartSceneGUID, true)}.Concat(EditorBuildSettings.scenes).ToArray();
			}

			AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(sampleDefinitionsInstallerGUID));
			AssetDatabase.Refresh();
	    }

		static void CleanUpMetaFiles(string path) {
			var directoryInfo = new DirectoryInfo(path);
			foreach (var file in directoryInfo.GetFiles("*.meta", SearchOption.AllDirectories)) {
				File.Delete(file.FullName);
			}
		}
	}
}