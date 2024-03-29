# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Calendar Versioning](https://calver.org).

## [2023.1127.1] - 2023-11-27
### Added
- Better error reporting during build pipelines
### Fixed
- Version Number to enforce update
- `Score` won't reset occasionally anymore

## [2023.1121.1] - 2023-11-21
### Added
- `UserScore`: Added support for User Score reporting to `GameManagerBase` and `GameBase`. Use `GameBase.API.ReportNewUserScore` to report a new score and `GameBase.Score` to read the current score. The score will be persisted between sessions
- `ReapplyShaders`: Whenever packaged Unity Bundles are loaded during Editor Runtime, all Shaders will be reapplied to Materials on known types like `Image`, and `Renderer`. If you need this feature for `Text Mesh Pro` Texts as well, you need to wait for a more sophisticated solution for adding custom Material Scanners through configuration.
- `Build`: Better reporting if build fails due to missing modules
- `UserScore`: A sample was added to `GameA`
- `ReapplyShaders`: This will fix broken shaders when e.g. using built Android bundles in the editor
### Fixed
- `GameManagerBase.PauseApplication`: Multiple invocations of the functions before unpausing caused the application to freeze. This has been fixed and a warning is printed instead.
### Removed
- `GameManagerBase.QuitApplication`: Use `IGameAPI.Quit` instead.
- `GameManagerBase.HasContentBundle`: Was probably never needed
- `GameManagerBase.Instance`: Use `GameBase.API` instead. This change includes usages of `QuitApplication`, `LoadAssetFromBundle<T>` and `LoadSceneFromBundle`
- `SimulationRun`: wouldn't work in the editor anymore with Editor Bundles. Fixed now.

## [2023.1115.1] - 2023-11-15
### Fixed
- `Android`: Fixed `Debug Run` to correctly execute on Android devices. For now, this requires you to manually enter the Bundle Name that you wish to launch. This can be improved later.

## [2022.1124.1] - 2022-11-24
### Fixed
- `BuildAssetBundlesGameDefinitionBuildStep` sending false alarms on Bundle Builds.

## [2022.1123.1] - 2022-11-23
### Added
- `PauseSynchronizationHandler` to `GameManager` to synchronize Pause States in case of multiple Pause() and Unpause Events.
  - You can call `Pause` to pause the Game and retrieve a `PauseHandle`
  - You can then `UnPause` the Game using that `PauseHandle`
  - `Reset` resets all `PauseHandles`
- `PauseActions` to `Game` that allow adding your custom OnPause and OnUnpause logic to Games.
  - They will synchronize with the `PauseSynchronizationHandler`, even if the Pause was already active when the `PauseHandler` was added.

## [2022.115.1] - 2022-01-15
### Fixed
- Improved some namings and spellings.

## [2021.1126.1] - 2021-11-26
### Added
- `Release` Plugin to `GameDefinitionHub` for Building the Unity Client. This method ensures, that all `GameDefinitions`' Asset Bundles have been built before building the client to avoid issues with Unity's Engine Code Stripping. Note: This does not ensure, that all `GameDefinitions` have been rebuilt in case of new Changes. It is recommended to use the `Rebuild Game Definitions` Option whenever you can afford the extra time for ensuring that no problems occur.
### Removed
- `Build Game` Menu Item. Please use the `GameDefinitionHub`'s `Release`-Plugin for Building the Unity Client.
### Fixed
- `Auto Run` is now correctly disabled if asked to Re-Run a Cancelled Build with `Auto Run` enabled before.

## [2021.903.1] - 2021-9-03
### Fixed
- A wrong reference in `UnityPlayerPrefs` that prevent the player from building successfully.

## [2021.827.1] - 2021-8-27
### Improved
- `BuildPlugin` of `GameDefinitionHub` now includes a confirmation popup.
### Fixed
- `GameWithSaveGame` not correctly loading existing SaveGames.

## [2021.423.1] - 2021-4-23
### Added
- The Option to use the `Run`-Plugin to run the Game without `autoRunEnabled`
### Fixed
- Correctly hides the Simulation GUI in Native Mode.
- Build Errors when Building for a Platform that's not your active Platform in the Unity Editor.


## [2021.420.1] - 2021-4-20
### Removed
- `GameDefinitionSettings.asset` and `GameDefinitionBuildSettings.asset` from the package. It only caused duplicate assets.

## [2021.418.1] - 2021-4-18
### Fixed
- the `Standalone-Build` Build-Pipeline. Adds the `Server Mode`-Setting to `GameDefinitionSettings` that enables you to switch between `Native` and `Simulation` `IMediaboxServer`. When using the `GameDefinitionHub`'s `Run`-Plugin, the setting is switched automatically.

## [2021.412.2] - 2021-4-12
### Fixed
- Ensures, that the `saveData`-Directory passed into `IMediaboxCallbacks.SetSaveDataFolder` is created before calling said event.
- Also passes the correct `path` into `IMediaboxCallbacks.WriteSaveData`

## [2021.413.1] - 2021-4-13
### Added
- `Standalone-Build` for faster iterations: Use `Game Definition Hub`'s new `Run`-Plugin to Execute your `Game Definitions` directly on your desired Target Platform. Uses Unity's `Streaming Assets` Technology.
- `PlayModeNativeAPI` for `Standalone-Builds`: It's an interactive GUI built into your application when doing a Standalone Build to allow you to switch between `Game Definitions` the way you're used from the `Simulation Mode`.
### Fixed
- `Engine Stripping` no longer removes scripts used in bundles when using the `Build Game` dialog
- `Game Definitions` for other platforms are no longer cleared when building all Game Definitions for one platform
- `NativeAPI` no longer gets stuck when an error happens during the saving process.
- Various errors within the `Sample Installer`. Some of which occured with newer Unity versions.
- An eror in `Pause Handler` that could get your game stuck for good when calling `Pause()` twice in a row.


## [2021.221.2] - 2021-2-21
### Added
- `Build Asset Bundle Options` to `GameDefinitionBuildSettings`. This allows you to specify your Asset Bundle Build Options to be used by the Game Definition Hub to build your Game Definition Bundles
- `CHANGELOG.md` to the package root to keep track of upcoming changes
- `README.md` to the repository root to include a quick introduction to the plugin