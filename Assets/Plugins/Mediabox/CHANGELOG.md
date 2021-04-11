# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
- TBD
## [2021.412.1] - 2021-4-12
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