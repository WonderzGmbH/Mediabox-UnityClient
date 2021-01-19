using System;
using System.IO;
using System.IO.Compression;

namespace Mediabox.GameManager.Editor.Utility {
	public static class PathUtility {
		public static void EnsureEmptyDirectory(string path) {
			DeleteDirectoryIfExists(path);
			EnsureDirectory(path);
		}

		public static bool EnsureDirectory(string path) {
			if (Directory.Exists(path))
				return false;
			Directory.CreateDirectory(path);
			return true;
		}

		public static void DeleteDirectoryIfExists(string path) {
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
			ZipFile.CreateFromDirectory(directoryPath, zipPath, CompressionLevel.Optimal, false);
			if (movedPlatformSettings)
				File.Move(tempIgnoreFilePath, ignoreFilePath);
		}

		public static void CopyFileToDirectory(string fileName, string fromDirectory, string toDirectory) {
			var toFilePath = Path.Combine(toDirectory, fileName);
			EnsureDirectory(Path.GetDirectoryName(toFilePath));
			File.Copy(Path.Combine(fromDirectory, fileName), Path.Combine(toDirectory, fileName), true);
		}
		
		public static bool MoveFileIfExists(string fromPath, string toPath) {
			if (!File.Exists(fromPath))
				return false;
			MoveFileToDirectory(fromPath, toPath);
			return true;
		}

		static void MoveFileToDirectory(string fromPath, string toPath) {
			EnsureDirectory(Path.GetDirectoryName(toPath));
			File.Move(fromPath, toPath);
		}

		public static void DeleteFileIfExists(string bundlePath) {
			if (File.Exists(bundlePath))
				File.Delete(bundlePath);
		}
	}
}