﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Build.BuildStep;
using Mediabox.GameManager.Editor.Build.Provider;
using Mediabox.GameManager.Editor.Build.Validator;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build {
	public class GameDefinitionBuild {
		readonly string[] directories;
		readonly bool clearDirectory;
		readonly BuildTarget[] buildTargets;
		readonly Type gameDefinitionType;
		readonly GameDefinitionSettings settings;
		readonly GameDefinitionBuildSettings buildSettings;

		public GameDefinitionBuild(string[] directories, bool clearDirectory, BuildTarget[] buildTargets, System.Type gameDefinitionType, GameDefinitionSettings gameDefinitionSettings, GameDefinitionBuildSettings buildSettings) {
			this.directories = directories;
			this.clearDirectory = clearDirectory;
			this.buildTargets = buildTargets;
			this.gameDefinitionType = gameDefinitionType;
			this.settings = gameDefinitionSettings;
			this.buildSettings = buildSettings;
		}

		public bool GetInfo(out IEnumerable<IGrouping<BuildTarget, GameDefinitionBuildInfo>> gameDefinitionsPerPlatform) {
			var buildInfoResult = ProvideBuildInfo(this.directories);
			gameDefinitionsPerPlatform = GetGameDefinitionsPerPlatform(this.buildTargets, buildInfoResult.buildInfos);
			return !buildInfoResult.hadErrors;
		}

		public void Execute() {
			if (!GetInfo(out var gameDefinitionsPerPlatform) &&
			    !EditorUtility.DisplayDialog("There have been errors", "Some GameDefinitions had errors. Do you still want to continue?", "OK", "Cancel"))
				return;
			var buildSteps = CreateBuildSteps();

			buildSteps.ForEach(buildStep => buildStep.PreProcess());

			foreach (var group in gameDefinitionsPerPlatform) {
				var gameDefinitions = group.ToArray();
				buildSteps.ForEach(buildStep => buildStep.Execute(group.Key, gameDefinitions));
			}
			buildSteps.ForEach(buildStep => buildStep.PostProcess());
		}

		/// <summary>
		/// Overload this method in order to inject your own Build Validators
		/// </summary>
		protected virtual IGameDefinitionBuildValidator[] CreateGameDefinitionBuildValidators() {
			return new IGameDefinitionBuildValidator[] {
				new SceneGameDefinitionBuildValidator(),
				new BundleExistsGameDefinitionBuildValidator(),
				new BundleIsSpecifiedGameDefinitionBuildValidator(),
			};
		}

		/// <summary>
		/// Overload this method in order to inject your own Build Info Provider
		/// </summary>
		/// <returns></returns>
		protected virtual IGameDefinitionBuildInfoProvider[] CreateBuildInfoProvider() {
			return new IGameDefinitionBuildInfoProvider[] {
				new GameDefinitionBuildInfoProvider(),
			};
		}

		/// <summary>
		/// Overload this method in order to inject your own Build Steps
		/// </summary>
		/// <returns></returns>
		protected virtual IGameDefinitionBuildStep[] CreateBuildSteps() {
			return new IGameDefinitionBuildStep[] {
				new TempDirectoryGameDefinitionBuildStep(this.buildSettings),
				new AssetBundlesGameDefinitionBuildStep(this.buildSettings),
				new ArchiveGameDefinitionBuildStep(this.settings, this.buildSettings, this.clearDirectory),
			};
		}
		
		IEnumerable<IGrouping<BuildTarget, GameDefinitionBuildInfo>> GetGameDefinitionsPerPlatform(BuildTarget[] buildTargets, GameDefinitionBuildInfo[] gameDefinitions) {
			return gameDefinitions
				.SelectMany(gameDefinition => 
					GetBuildTargetsForGameDefinition(gameDefinition, buildTargets)
						.Select(buildTarget => (buildTarget, gameDefinition))
				)
				.GroupBy(touple => touple.buildTarget, touple => touple.gameDefinition);
		}

		BuildTarget[] GetBuildTargetsForGameDefinition(GameDefinitionBuildInfo gameDefinition, BuildTarget[] buildTargets) {
			var platformSettingsPath = Path.Combine(gameDefinition.directory, GameDefinitionBuildSettings.customPlatformSettings);
			var customPlatformSettings = File.Exists(platformSettingsPath) ? JsonUtility.FromJson<CustomPlatformSettings>(File.ReadAllText(platformSettingsPath)) : null;
			return buildTargets.Where(buildTarget => this.buildSettings.supportedBuildTargets.Concat(customPlatformSettings?.supportedPlatforms ?? new BuildTarget[0]).Contains(buildTarget) && customPlatformSettings?.unsupportedPlatforms?.Contains(buildTarget) != true).ToArray();
		}
		
		public GameDefinitionBuildInfoResult ProvideBuildInfo(string[] directories) {
			var results = CreateBuildInfoProvider().Select(provider => provider.Provide(directories, this.settings.gameDefinitionFileName, this.gameDefinitionType, CreateGameDefinitionBuildValidators())).ToArray();
			return new GameDefinitionBuildInfoResult {
				hadErrors = results.Any(result => result.hadErrors),
				buildInfos = results.SelectMany(result => result.buildInfos).ToArray()
			};
		}
	}
}