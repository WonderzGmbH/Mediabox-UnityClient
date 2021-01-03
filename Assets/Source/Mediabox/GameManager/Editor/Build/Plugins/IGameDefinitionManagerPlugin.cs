
using UnityEngine;

namespace Mediabox.GameManager.Editor.Build.Plugins {
	public interface IGameDefinitionManagerPlugin {
		string Title { get; }
		void Update();
		bool Render();
		bool ToggleableWithTitleLabel { get; }
	}
}