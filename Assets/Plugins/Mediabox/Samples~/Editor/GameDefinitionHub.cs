using Mediabox.GameManager.Editor;
using UnityEditor;

namespace Mediabox.Samples.Editor {
	/// <summary>
	/// A sample game definition manager implementation.
	/// The MenuItem-Attribute ensures that you can open this window using Unity's MenuBar in the top of the window.
	/// Details to this class can be found in the base class.
	/// </summary>
	public class GameDefinitionHub : SimulationGameDefinitionHub<GameDefinitionHub, GameDefinition> {
		[MenuItem("MediaBox/"+shortTitle)]
		public static GameDefinitionHub OpenWindow() {
			return ShowWindow();
		}
	}
}