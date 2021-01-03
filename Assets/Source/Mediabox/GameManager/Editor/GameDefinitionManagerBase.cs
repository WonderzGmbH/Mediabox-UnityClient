using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Build.Plugins;
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
	public class GameDefinitionManagerBase<TWindow, TGameDefinition> : GameDefinitionManagerBase<TGameDefinition>
		where TWindow : GameDefinitionManagerBase<TWindow, TGameDefinition>
		where TGameDefinition : class, IGameDefinition, new() {
		IGameDefinitionManagerPlugin[] plugins;
		static TWindow window;

		protected static TWindow ShowWindow() {
			window = GetWindow<TWindow>();
			window.titleContent = new GUIContent("Game Definition Manager");
			window.Show();
			return window;
		}

		protected virtual IGameDefinitionManagerPlugin[] CreatePlugins() {
			var settings = new SettingsPlugin();
			var directory = new DirectoryPlugin(settings);
			var management = new GameDefinitionManagementPlugin(settings, this);
			var editor = new GameDefinitionEditorPlugin<TGameDefinition>(settings, management, this);
			var platformSettings = new CustomPlatformSettingsPlugin(management, this);
			var bundles = new BundlesPlugin();
			var simulation = new SimulationPlugin(management);
			var build = new BuildPlugin<TGameDefinition>(settings, management, this);

			return new IGameDefinitionManagerPlugin[] {
				settings,
				directory,
				management,
				editor,
				platformSettings,
				bundles,
				simulation,
				build
			};
		}

		void OnGUI() {
			if (this.plugins == null) {
				this.plugins = CreatePlugins();
			}

			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, false, false);
			for (var i = 0; i < this.plugins.Length; i++) {
				var plugin = this.plugins[i];
				try {
					EditorGUILayout.BeginVertical(GetColoredBoxStyle(this.plugins.Length, i));
					var prefKey = "Mediabox.GameManager.Editor.GameDefinitionManagerBase.Plugin::" + plugin;
					var show = EditorGUILayout.Foldout(EditorPrefs.GetBool(prefKey, true), plugin.Title, FoldoutStyle);
					EditorPrefs.SetBool(prefKey, show);
					plugin.Update();
					if (!show)
						continue;
					if (!plugin.Render())
						break;
				} finally {
					EditorGUILayout.EndVertical();
				}
			}
			GUILayout.EndScrollView();
		}

		static GUIStyle _foldoutStyle;

		static GUIStyle FoldoutStyle {
			get {
				if (_foldoutStyle == null) {
					_foldoutStyle = new GUIStyle(EditorStyles.foldout) {
						fontStyle = FontStyle.Bold
					};
				} 
				return _foldoutStyle;
			}
		}

		public GUIStyle[] guiStyleArray;
		Vector2 scrollPosition;

		GUIStyle GetColoredBoxStyle(int count, int index) {
			if (this.guiStyleArray == null || this.guiStyleArray.Length != count || (this.guiStyleArray.Length > 0 && this.guiStyleArray[0].normal.background == null)) {
				this.guiStyleArray = new GUIStyle[count];
				for (var i = 0; i < this.guiStyleArray.Length; ++i) {
					var rgb = Color.HSVToRGB((float) i / count, 0.7f, 1f);
					rgb.a = 0.15f;
					var texture = CreateTexture(2, 2, rgb);
					this.guiStyleArray[i] = new GUIStyle(GUI.skin.box) {
						normal = {
							background = texture,
							scaledBackgrounds = new []{texture}
						}
					};
				}
			}

			return this.guiStyleArray[index];
		}

		static Texture2D CreateTexture(int width, int height, Color color) {
			var colors = new Color[width * height];
			for (var index = 0; index < colors.Length; ++index)
				colors[index] = color;
			var texture2D = new Texture2D(width, height);
			texture2D.SetPixels(colors);
			texture2D.Apply();
			return texture2D;
		}
	}

	public class GameDefinitionManagerBase<TGameDefinition> : GameDefinitionManagerBase
		where TGameDefinition : class, IGameDefinition, new() {
		public TGameDefinition gameDefinition;


		public override IGameDefinition CreateGameDefinition() {
			return new TGameDefinition();
		}
	}

	public abstract class GameDefinitionManagerBase : EditorWindow {
		public CustomPlatformSettings customPlatformSettings;

		public abstract IGameDefinition CreateGameDefinition();
	}
}