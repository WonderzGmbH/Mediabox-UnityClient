using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor {
	public class MediaboxSampleTestMode : MonoBehaviour {
		const string sampleTestModeMenuItem = "MediaBox/Sample Test Mode";
		const string samplesPath = "Assets/Plugins/Mediabox/Samples";

		static bool isEnabled;

		[MenuItem(sampleTestModeMenuItem)]
		static void ToggleSampleTestMode() {
			if (Directory.Exists(samplesPath))
				Directory.Move(samplesPath, samplesPath + "~");
			else
				Directory.Move(samplesPath + "~", samplesPath);
		}

		[MenuItem(sampleTestModeMenuItem, true)]
		static bool PerformActionValidation() {
			Menu.SetChecked(sampleTestModeMenuItem, Directory.Exists(samplesPath));
			return true;
		}
	}
}