﻿using System.Linq;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor;
using Mediabox.GameManager.Editor.HubPlugins;
using Mediabox.GameManager.Simulation.Editor.HubPlugins;

namespace Mediabox.GameManager.Simulation.Editor {
	/// <summary>
	/// Implement this class to add a typed Game Definition Manager Editor Window. for your project.
	/// Implement it like this:
	/// ATTENTION! Place this script within an Editor-Folder in your Unity-Project. Your project won't build if you don't.
	/// <code>
	/// public class GameDefinitionManager : GameDefinitionManagerBase<GameDefinitionManager, GameDefinition> {
	/// 	[MenuItem("MediaBox/Game Definition Manager")]
	/// 	static GameDefinitionManager OpenWindow() {
	/// 		return ShowWindow();
	/// 	}
	/// }
	/// </code>
	/// </summary>
	/// <typeparam name="TWindow">Your inherited class type.</typeparam>
	/// <typeparam name="TGameDefinition">The type of GameDefinition that you're using.</typeparam>
	public class SimulationGameDefinitionHub<TWindow, TGameDefinition> : GameDefinitionHub<TWindow, TGameDefinition>
		where TWindow : GameDefinitionHub<TWindow, TGameDefinition>
		where TGameDefinition : class, IGameDefinition, new() {

		protected override IHubPlugin[] CreatePlugins() {
			var basePlugins = base.CreatePlugins();
			var settings = basePlugins
				.OfType<SettingsPlugin>()
				.FirstOrDefault() ?? new SettingsPlugin();
			var management = basePlugins
				.OfType<ManagementPlugin>()
				.FirstOrDefault() ?? new ManagementPlugin(settings, this);
			var simulation = new SimulationPlugin(management, settings);
			return basePlugins.Concat(new[] {simulation}).ToArray();
		}
	}
}