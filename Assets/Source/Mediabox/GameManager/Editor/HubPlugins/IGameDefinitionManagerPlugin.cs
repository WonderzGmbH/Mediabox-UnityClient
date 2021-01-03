
namespace Mediabox.GameManager.Editor.HubPlugins {
	public interface IGameDefinitionManagerPlugin {
		string Title { get; }
		void Update();
		bool Render();
		bool ToggleableWithTitleLabel { get; }
	}
}