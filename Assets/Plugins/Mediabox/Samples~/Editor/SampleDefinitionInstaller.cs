using System;
using System.IO;
using System.Linq;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace Mediabox.Samples.Editor {
	[InitializeOnLoad]
	public static class SampleDefinitionInstaller {

		static bool FileForGUidExists(string guid) {
			var path = AssetDatabase.GUIDToAssetPath(guid);
			return !string.IsNullOrEmpty(path);
		}
		
		static bool ValidateInstallerIntegrity() {
			return InstallerGuids.All(FileForGUidExists);
		}
		
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

		static string[] InstallerGuids => new[] {
			sampleGameDefinitionsDirectoryGuid,
			sampleDefinitionsInstallerGuid,
			gameAGuid,
			gameBGuid,
			gameASceneGuid,
			gameBSceneGuid,
			startSceneGuid
		};

		const string sampleGameDefinitionsDirectoryGuid = "fcf7cdaef5f1f48d59f16112f65bb8a4";
		const string sampleDefinitionsInstallerGuid = "a20b3549f2c04406e9b4147d6d3d78f8";
		const string gameAGuid = "55ec951530e134ec3ad773c17327c81e";
		const string gameBGuid = "650fce85ea96447ed85936ac0c52dba8";
		const string gameASceneGuid = "602359a37ba394666a2e82055cc2b42a";
		const string gameBSceneGuid = "a1bc195cc966d473083779ddcc60c611";
		const string startSceneGuid = "d3fd893bb1dda45fdb74e49aaa202923";
		const string scenePathTag = "${SCENE_PATH}";

		static void UpdateGameDefinitionPaths() {
			UpdateGameDefinitionScenePath(gameAGuid, gameASceneGuid);
			UpdateGameDefinitionScenePath(gameBGuid, gameBSceneGuid);
		}

		static void UpdateGameDefinitionScenePath(string gameDefinitionGuid, string gameDefinitionSceneGuid) {
			var gamePath = AssetDatabase.GUIDToAssetPath(gameDefinitionGuid);
			File.WriteAllText(gamePath, File.ReadAllText(gamePath).Replace(scenePathTag, AssetDatabase.GUIDToAssetPath(gameDefinitionSceneGuid)));
		}

		static SampleDefinitionInstaller() {
			EditorApplication.update += InitAfterEditorStartup;
		}

		static void DeleteAssetAtGuid(string assetGuid) {
			var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
			if (string.IsNullOrEmpty(assetPath))
				return;
			if (!AssetDatabase.DeleteAsset(assetPath)) {
				Debug.LogWarning($"Error occured while deleting file at {assetPath} through Unity. Deleting the asset using File System instead.");
				PathUtility.DeleteAssetFileIfExists(assetPath);
			}
			AssetDatabase.Refresh();
		}

		static void InitAfterEditorStartup() {
			EditorApplication.update -= InitAfterEditorStartup;

			if (!ValidateInstallerIntegrity()) {
				EditorUtility.DisplayDialog("Installer incomplete", "Not all files required for Sample Installation exist. Most likely, they have been installed already. If you find any issues with the Sample Files, just Install the Sample Scripts again through the Package Manager or contact the Package Developer.", "OK");
				DeleteAssetAtGuid(sampleDefinitionsInstallerGuid);
				return;
			}
			
			UpdateGameDefinitionPaths();
			
			if (!HasGameDefinitions || MakeBackupOfGameDefinitions()) {
				var sampleDefinitionPath = AssetDatabase.GUIDToAssetPath(sampleGameDefinitionsDirectoryGuid);
				var path = GameDefinitionSettings.Load().gameDefinitionDirectoryPath;
				Directory.Move(sampleDefinitionPath, path);
				CleanUpMetaFiles(path);
				File.Delete(Path.ChangeExtension(sampleDefinitionPath, "meta"));
				GameDefinitionHub.OpenWindow();
			}

			if (EditorUtility.DisplayDialog("Change Build Scenes", $"In order for the Simulation Mode to work for these samples, we need to add a new Start-Scene to your Build Settings. Can we proceed?", "OK", "Cancel")) {
				GUID.TryParse(startSceneGuid, out var parsedStartSceneGUID);
				EditorBuildSettings.scenes = new[] {new EditorBuildSettingsScene(parsedStartSceneGUID, true)}.Concat(EditorBuildSettings.scenes).ToArray();
			}

			DeleteAssetAtGuid(sampleDefinitionsInstallerGuid);
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