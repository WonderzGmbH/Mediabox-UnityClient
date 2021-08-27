using System;
using System.IO;
using Mediabox.GameManager.Editor.Build;
using Mediabox.GameManager.Simulation;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.HubPlugins {
	public class BuildPlugin : IHubPlugin {

		public string Title => "Build";
		public bool ToggleableWithTitleLabel => true;
		
		readonly SettingsPlugin settingsPlugin;
		readonly ManagementPlugin managementPlugin;
		readonly GameDefinitionHub manager;
		readonly EnumPref<BuildPlatformOption> buildPlatformOption;
		readonly BoolPref doNotAskForConfirmation;
		BuildTarget manualPlatform;

		public BuildPlugin(SettingsPlugin settingsPlugin, ManagementPlugin managementPlugin, GameDefinitionHub manager, IPrefs prefs) {
			this.settingsPlugin = settingsPlugin;
			this.managementPlugin = managementPlugin;
			this.manager = manager;
			this.buildPlatformOption = new EnumPref<BuildPlatformOption>(prefs, "Mediabox.GameManager.Editor.HubPlugins.BuildPlugin.BuildPlatformOption");
			this.doNotAskForConfirmation = new BoolPref(prefs, "Mediabox.GameManager.Editor.HubPlugins.BuildPlugin.DoNotAskForConfirmation");
		}

		public void Update() { }

		public bool Render() {
			var directories = this.managementPlugin.AllDirectories;
			DrawBuildPlatformsArea();
			GUILayout.Label($"Building: {string.Join(", ", GetSelectedBuildTargets())}");
			if (GUILayout.Button($"Build All")) {
				BuildGameDefinitions(directories, true, GetSelectedBuildTargets());
				return false;
			}

			if (this.manager != null & GUILayout.Button($"Build {Path.GetFileName(this.managementPlugin.SelectedDirectory)}")) {
				BuildGameDefinitions(new[] {this.managementPlugin.SelectedDirectory}, false, GetSelectedBuildTargets());
				return false;
			}

			return true;
		}


		enum BuildPlatformOption {
			AllSupportedPlatforms,
			CurrentPlatform,
			ManualPlatforms
		}

		BuildTarget[] GetSelectedBuildTargets() {
			switch (this.buildPlatformOption.Value) {
				case BuildPlatformOption.AllSupportedPlatforms:
					return this.settingsPlugin.buildSettings.supportedBuildTargets;
				case BuildPlatformOption.CurrentPlatform:
					return new[] {EditorUserBuildSettings.activeBuildTarget};
				case BuildPlatformOption.ManualPlatforms:
					return new[] {this.manualPlatform};
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void BuildGameDefinitions(string[] directories, bool clearDirectory, BuildTarget[] buildTargets) {
			if (!this.doNotAskForConfirmation.Value) {
				switch (EditorUtility.DisplayDialogComplex("Confirm Build", "This action might take a couple minutes to complete.", "OK", "Cancel", "Never ask again")) {
					case 0:
						break;
					case 1:
						return;
					case 2:
						this.doNotAskForConfirmation.Value = true;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			var build = CreateGameDefinitionBuild(directories, clearDirectory, buildTargets);
			build.Execute();
		}

		void DrawBuildPlatformsArea() {
			this.buildPlatformOption.Value = (BuildPlatformOption) EditorGUILayout.EnumPopup(this.buildPlatformOption.Value);
			switch (this.buildPlatformOption.Value) {
				case BuildPlatformOption.ManualPlatforms:
					this.manualPlatform = (BuildTarget) EditorGUILayout.EnumPopup(this.manualPlatform);
					break;
				case BuildPlatformOption.CurrentPlatform:
					GUILayout.Label("Current Platform: " + EditorUserBuildSettings.activeBuildTarget);
					break;
			}
		}

		/// <summary>
		/// Overload this method in order to inject your own Build Script.
		/// </summary>
		protected virtual GameDefinitionBuild CreateGameDefinitionBuild(string[] directories, bool clearDirectory, BuildTarget[] buildTargets) {
			return new GameDefinitionBuild(directories, clearDirectory, buildTargets, this.manager.GetGameDefinitionType(), this.settingsPlugin.settings, this.settingsPlugin.buildSettings);
		}
	}
}