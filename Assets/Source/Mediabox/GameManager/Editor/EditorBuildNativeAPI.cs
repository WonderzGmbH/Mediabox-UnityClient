using System;
using System.IO;
using System.IO.Compression;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor.Build;
using Mediabox.GameManager.Editor.Utility;
using UnityEditor;

namespace Mediabox.GameManager.Editor {
	public class EditorBuildNativeAPI : EditorNativeAPI, IDisposable {
		readonly GameDefinitionBuildSettings buildSettings;
		protected override string ContentBundleFolder => Path.Combine(this.buildSettings.tempSimulationBuildPath, this.BundleName);

		public EditorBuildNativeAPI(string bundleName, GameDefinitionSettings settings, GameDefinitionBuildSettings buildSettings) : base (bundleName, settings) {
			this.buildSettings = buildSettings;
			PathUtility.EnsureEmptyDirectory(this.buildSettings.tempSimulationBuildPath);
		}

		protected override void PrepareContentBundle() {
			ExtractContentBundle();
		}

		void ExtractContentBundle() {
			var bundlePath = Path.ChangeExtension(Path.Combine(Path.Combine(this.buildSettings.gameDefinitionBuildPath, EditorUserBuildSettings.activeBuildTarget.ToString()), this.BundleName), "zip");
			PathUtility.EnsureEmptyDirectory(this.ContentBundleFolder);
			if (!File.Exists(bundlePath)) {
				throw new System.Exception($"No Build exists for Game {this.BundleName} at {bundlePath}. Please rebuild your Game Definitions before using the SimulationBuildMode.");
			}
			ZipFile.ExtractToDirectory(bundlePath, this.ContentBundleFolder);
		}

		void ReleaseUnmanagedResources() {
			PathUtility.DeleteDirectoryIfExists(this.buildSettings.tempSimulationBuildPath);
		}

		public void Dispose() {
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		~EditorBuildNativeAPI() {
			ReleaseUnmanagedResources();
		}
	}
}

