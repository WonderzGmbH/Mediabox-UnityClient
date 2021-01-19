
namespace Mediabox.GameManager.Editor.HubPlugins {
	public interface IHubPlugin {
		string Title { get; }
		void Update();
		bool Render();
		bool ToggleableWithTitleLabel { get; }
	}
}