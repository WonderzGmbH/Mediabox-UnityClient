using System;
using System.IO;
using System.IO.Compression;

namespace Mediabox.GameManager.Editor.Utility {
	public static class PathHelper {
		public static void EnsureEmptyDirectory(string path) {
			SafeDeleteDirectory(path);
			SafeCreateDirectory(path);
		}

		public static bool SafeCreateDirectory(string path) {
			if (Directory.Exists(path))
				return false;
			Directory.CreateDirectory(path);
			return true;
		}

		public static void SafeDeleteDirectory(string path) {
			if (Directory.Exists(path))
				Directory.Delete(path, true);
		}

		public static string GetRelativePath(string path, string relativeToPath) {
			var uri = new Uri(Path.GetFullPath(path));
			var relativeToUri = new Uri(Path.GetFullPath(relativeToPath));
			return relativeToUri.MakeRelativeUri(uri).ToString();
		}

		public static void ZipDirectoryWithExcludeFile(string directoryPath, string zipPath, string ignoreFilePath, string temporaryCachePath) {
			var tempIgnoreFilePath = Path.Combine(temporaryCachePath, ignoreFilePath);
			var movedPlatformSettings = MoveFileIfExists(ignoreFilePath, tempIgnoreFilePath);
			System.IO.Compression.ZipFile.CreateFromDirectory(directoryPath, zipPath, CompressionLevel.Optimal, false);
			if (movedPlatformSettings)
				File.Move(tempIgnoreFilePath, ignoreFilePath);
		}

		public static void SafeCopyFile(string fileName, string fromDirectory, string toDirectory) {
			if (!Directory.Exists(toDirectory))
				Directory.CreateDirectory(toDirectory);
			File.Copy(Path.Combine(fromDirectory, fileName), Path.Combine(toDirectory, fileName), true);
		}
		
		public static bool MoveFileIfExists(string fromPath, string toPath) {
			if (!File.Exists(fromPath))
				return false;
			SafeCreateDirectory(Path.GetDirectoryName(toPath));
			File.Move(fromPath, toPath);
			return true;
		}

		public static void SafeDeleteFile(string bundlePath) {
			if (File.Exists(bundlePath))
				File.Delete(bundlePath);
		}
	}
}