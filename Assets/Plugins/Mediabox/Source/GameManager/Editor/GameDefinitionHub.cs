using System;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.HubPlugins;
using Mediabox.GameManager.Editor.Utility;
using Mediabox.Samples;
using UnityEditor;
using UnityEngine;

namespace Mediabox.GameManager.Editor {
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
	public class GameDefinitionHub<TWindow, TGameDefinition> : GameDefinitionHub<TGameDefinition>
		where TWindow : GameDefinitionHub<TWindow, TGameDefinition>
		where TGameDefinition : class, IGameDefinition, new() {
		IHubPlugin[] plugins;
		static TWindow window;
		Vector2 scrollPosition;
		protected const string shortTitle = "Game Definition Hub";
		const string fullTitle = "Mediabox "+shortTitle;
		
		protected static TWindow ShowWindow() {
			window = GetWindow<TWindow>();
			window.titleContent = new GUIContent(shortTitle);
			window.Show();
			return window;
		}

		protected virtual IHubPlugin[] CreatePlugins() {
			var settings = new SettingsPlugin();
			var directory = new DirectoryPlugin(settings);
			var management = new ManagementPlugin(settings, this);

			return new IHubPlugin[] {
				new TitlePlugin(fullTitle),
				settings,
				directory,
				management,
				new EditorPlugin<TGameDefinition>(settings, management, this),
				new CustomPlatformSettingsPlugin(management, this),
				new BundlesPlugin(),
				new SimulationPlugin(management, settings),
				new BuildPlugin(settings, management, this)
			};
		}

		void OnGUI() {
			if (this.plugins == null) {
				this.plugins = CreatePlugins();
			}

			var scrollStack = LayoutUtility.ScrollStack(ref this.scrollPosition);
			for (var i = 0; i < this.plugins.Length; i++) {
				var plugin = this.plugins[i];
				using (LayoutUtility.VerticalStack(StyleUtility.GetColoredBoxStyle(this.plugins.Length, i))) {
					plugin.Update();
					var pluginVisible = UpdatePluginVisibility(plugin);
					if (!pluginVisible)
						continue;
					if (!plugin.Render())
						GUIUtility.ExitGUI();
				}
			}
			scrollStack.Dispose();
		}

		static bool UpdatePluginVisibility(IHubPlugin plugin) {
			if (!plugin.ToggleableWithTitleLabel) 
				return true;
			var prefKey = "Mediabox.GameManager.Editor.GameDefinitionManagerBase.Plugin::" + plugin;
			EditorPrefs.SetBool(prefKey, EditorGUILayout.Foldout(EditorPrefs.GetBool(prefKey, true), plugin.Title, StyleUtility.FoldoutStyle));
			return EditorPrefs.GetBool(prefKey);
		}
	}

	public class GameDefinitionHub<TGameDefinition> : GameDefinitionHub
		where TGameDefinition : class, IGameDefinition, new() {
		public TGameDefinition gameDefinition;

		public override IGameDefinition CreateGameDefinition() {
			return new TGameDefinition();
		}

		public override Type GetGameDefinitionType() {
			return typeof(TGameDefinition);
		}
	}

	public abstract class GameDefinitionHub : EditorWindow {
		public CustomPlatformSettings customPlatformSettings;
		public abstract IGameDefinition CreateGameDefinition();
		public abstract System.Type GetGameDefinitionType();
	}
}