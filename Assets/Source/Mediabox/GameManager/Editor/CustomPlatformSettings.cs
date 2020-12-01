using System;
using UnityEditor;

namespace Mediabox.GameManager.Editor {
	[Serializable]
	public class CustomPlatformSettings {
		public BuildTarget[] supportedPlatforms;
		public BuildTarget[] unsupportedPlatforms;
	}
}