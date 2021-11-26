using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Build.Provider;
using Mediabox.GameManager.Editor.Build.Validator;
using Mediabox.GameManager.Editor.HubPlugins;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build {
	public class BuildGameEditorWindow : EditorWindow {
		enum State {
			Uninitialized,
			Initialized,
			Ready,
			Success,
			Error
		}

		State state;

		BuildTarget buildTarget;
		bool streamingAssetsGameDefinitionMode;
		bool autoRun;
		BuildPlayerOptions buildPlayerOptions;
		string error;
		ServerMode? oldIntegrationMode;
		SettingsPlugin settingsPlugin;

		void Initialize() {

			buildPlayerOptions.target = buildTarget;
			settingsPlugin = new SettingsPlugin();
			settingsPlugin.Update();

			var manifestPath = Path.ChangeExtension(settingsPlugin.buildSettings.GetAssetBundleManifestPath(buildTarget), null);

			buildPlayerOptions.assetBundleManifestPath = manifestPath;
			if (autoRun)
				buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;
			oldIntegrationMode = settingsPlugin.settings.ServerMode;
			if (streamingAssetsGameDefinitionMode) {
				Debug.Log($"Configuring {nameof(ServerMode)}.{nameof(ServerMode.Simulation)}");
				settingsPlugin.settings.ServerMode = ServerMode.Simulation;
				EditorUtility.SetDirty(settingsPlugin.settings);
			}

			if (!ValidateAssetBundleManifestFileExists(manifestPath))
				this.state = State.Initialized;
			else
				this.state = State.Ready;
		}

		void SetError(string error) {
			this.error = error;
			this.state = State.Error;

			if (this.oldIntegrationMode.HasValue) {
				settingsPlugin.settings.ServerMode = oldIntegrationMode.Value;
				EditorUtility.SetDirty(settingsPlugin.settings);
			}
		}

		void OnGUI() {
			switch (state) {
				case State.Uninitialized:
					if(Event.current.type != EventType.Layout)
						Initialize();
					break;
				case State.Error:
					EditorGUILayout.HelpBox(this.error, MessageType.Error);
					if (GUILayout.Button("OK") && Event.current.type != EventType.Layout) {
						Close();
					}

					break;
				case State.Initialized:
					if (GUILayout.Button("Proceed anyway") && Event.current.type != EventType.Layout) {
						this.state = State.Initialized;
					}

					if (GUILayout.Button("Cancel") && Event.current.type != EventType.Layout) {
						Close();
					}

					break;
				case State.Ready:
					if (GUILayout.Button("Build") && Event.current.type != EventType.Layout) {
						Build();
					}

					if (GUILayout.Button("Cancel") && Event.current.type != EventType.Layout) {
						Close();
					}
					break;
				case State.Success:
					GUILayout.Label("Success!");
					if (GUILayout.Button("Show Build Location")) {
						OpenInFileBrowser.Open(Path.GetDirectoryName(this.buildPlayerOptions.locationPathName));
					}

					if (GUILayout.Button("OK")) {
						Close();
					}

					break;
			}
		}

		void Build() {
			this.state = State.Success;
			return;
			var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
			if (report.summary.result == BuildResult.Cancelled && autoRun && EditorUtility.DisplayDialog("Build Cancelled", "Would you like to re-run the build without autoRun?", "Yes", "No")) {
				buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;
				BuildPipeline.BuildPlayer(buildPlayerOptions);
			}
			
			this.state = State.Success;
		}

		public void Build(BuildTarget buildTarget, bool streamingAssetsGameDefinitionMode = false, bool autoRun = true) {
			this.buildTarget = buildTarget;
			this.streamingAssetsGameDefinitionMode = streamingAssetsGameDefinitionMode;
			this.autoRun = autoRun;
			this.state = State.Uninitialized;
			
			try {
				buildPlayerOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(new BuildPlayerOptions() { target = buildTarget, targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget) });
			} catch (BuildPlayerWindow.BuildMethodException e) {
				SetError("BuildMethodException caught: " + e);
			}
		}

		AssetBundleManifest manifest;
		Dictionary<string, bool> gameDefinitionsFound;

		bool ValidateAssetBundleManifestFileExists(string manifestPath) {
			return true;
			// if (File.Exists(manifestPath)) {
			// 	AssetBundle.UnloadAllAssetBundles(true);
			// 	var manifestAssetBundle = AssetBundle.LoadFromFile(manifestPath);
			// 	manifest = manifestAssetBundle.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));
			// 	manifestAssetBundle.Unload(false);
			// 	var assetBundles = new HashSet<string>(this.manifest.GetAllAssetBundles());
			//
			// 	var buildInfoProvider = new GameDefinitionBuildInfoProvider();
			// 	var directories = Directory.GetDirectories(this.settingsPlugin.settings.gameDefinitionDirectoryPath);
			// 	var buildInfoResult = buildInfoProvider.Provide(directories, this.settingsPlugin.settings.gameDefinitionFileName, typeof(SGameDefinition), Array.Empty<IGameDefinitionBuildValidator>());
			// 	var gameDefinitions = GameDefinitionBuild.GetGameDefinitionsPerPlatform(new[] { this.buildTarget }, buildInfoResult.buildInfos).First();
			//
			// 	var nextState = State.Ready;
			// 	foreach (var gameDefinition in gameDefinitions) {
			// 		if (!assetBundles.Contains((gameDefinition.gameDefinition as SGameDefinition).BundleName)) {
			// 			Debug.Log("Asset Bundle Does not Exist: " + gameDefinition.directory);
			// 			nextState = State.Initialized;
			// 		} else {
			// 			Debug.Log("Asset Bundle Does Exist: " + gameDefinition.directory);
			// 		}
			// 	}
			//
			// 	this.state = nextState;
			// }
			//
			// return File.Exists(manifestPath) || EditorUtility.DisplayDialog("Warning", "It is recommended to build Asset Bundles first, using the GameDefinitionManager-Window. Continuing without may result in required code being stripped from the engine.", "OK", "Cancel");
		}
	}
}

public class SGameDefinition : IGameBundleSceneDefinition, IGameDefinition {
	public string game;
	public string skin;
	public string bundlePath;
	public string scenePath;
	public string BundleName => this.bundlePath;
	public string SceneName => this.scenePath;
	public string[] AdditionalBundles => new string[0];
}