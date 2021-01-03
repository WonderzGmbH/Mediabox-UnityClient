using System;

namespace Mediabox.GameManager.Editor.Utility {
	public static class OpenInFileBrowser {
		public static void Open(string path) {
			if (IsWindows)
				OpenInWin(path);
			else if (IsMac)
				OpenInMac(path);
			else { // couldn't determine OS
				OpenInWin(path);
				OpenInMac(path);
			}
		}

		static bool IsMac => UnityEngine.SystemInfo.operatingSystem.IndexOf("Mac OS", StringComparison.Ordinal) != -1;

		static bool IsWindows => UnityEngine.SystemInfo.operatingSystem.IndexOf("Windows", StringComparison.Ordinal) != -1;

		[UnityEditor.MenuItem("Window/Test OpenInFileBrowser")]
		public static void Test() {
			Open(UnityEngine.Application.dataPath);
		}

		static void OpenInMac(string path) {
			var openInsidesOfFolder = false;

			var macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

			if (System.IO.Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
				openInsidesOfFolder = true;

			if (!macPath.StartsWith("\""))
				macPath = "\"" + macPath;

			if (!macPath.EndsWith("\""))
				macPath = macPath + "\"";

			var arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

			try {
				System.Diagnostics.Process.Start("open", arguments);
			} catch (System.ComponentModel.Win32Exception e) {
				// tried to open mac finder in windows
				// just silently skip error
				// we currently have no platform define for the current OS we are in, so we resort to this
				e.HelpLink = ""; // do anything with this variable to silence warning about not using it
			}
		}

		static void OpenInWin(string path) {
			var openInsidesOfFolder = false;

			// try windows
			var winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

			if (System.IO.Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
				openInsidesOfFolder = true;

			try {
				System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
			} catch (System.ComponentModel.Win32Exception e) {
				// tried to open win explorer in mac
				// just silently skip error
				// we currently have no platform define for the current OS we are in, so we resort to this
				e.HelpLink = ""; // do anything with this variable to silence warning about not using it
			}
		}
	}
}