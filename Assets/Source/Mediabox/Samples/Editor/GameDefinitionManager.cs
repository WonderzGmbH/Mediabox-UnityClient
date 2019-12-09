using Mediabox.GameManager.Editor;
using UnityEditor;

namespace Mediabox.Samples.Editor {
	/// <summary>
	/// A sample game definition manager implementation.
	/// The MenuItem-Attribute ensures that you can open this window using Unity's MenuBar in the top of the window.
	/// Details to this class can be found in the base class.
	/// </summary>
	public class GameDefinitionManager : GameDefinitionManagerBase<GameDefinitionManager, GameDefinition> {
		[MenuItem("MediaBox/Game Definition Manager")]
		static GameDefinitionManager OpenWindow() {
			return ShowWindow();
		}
	}
}