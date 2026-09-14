# Repository guide

Paths in this document are relative to the repository root.
Read and follow the [development policy](policies/development.md) alongside this guide.

## About

`VolocyNazad.Toolkit.Debouncing` provides WPF dispatcher-based debounce and throttle helpers through `DebounceDispatcher` and `IDebounceDispatcher`.

## Repository structure

- `Toolkit.Debouncing.sln` - solution.
- `src/Toolkit.Debouncing/` - library, dispatcher implementation, and abstractions.
- `tests/Toolkit.Debouncing.Tests/` - WPF dispatcher tests.
- `.github/workflows/` - CI and publishing workflows.

## Technology stack

- `Microsoft.NET.Sdk` with WPF and Windows targeting enabled.
- Target frameworks as declared in the project: `net48;net8-windows;net9-windows`.
- C# 13, nullable reference types, implicit usings.
- AutoConstructor and SonarAnalyzer.CSharp; package versions are specified inline in the project.
- The package version is derived by MinVer from stable `vMAJOR.MINOR.PATCH` Git tags; publish only tagged commits.
- Dispatcher scheduling uses `DispatcherTimer`; the implementation has separate `NET9_0` and fallback branches for locking.
- No Revit API dependency.

## Documentation layout

- `AGENTS.md` links to the required repository guidance.
- `docs/policies/development.md` contains the development policy.
- `docs/repository.md` describes the project, repository structure, and technology stack.
- `CHANGELOG.md` records changes under `Unreleased` before release.

The root solution exposes the documentation files under a `docs` solution folder in Visual Studio, preserving their subfolder structure. When adding documentation files, also add them as solution items; solution folders do not automatically include new files.

The root `global.json` selects stable .NET SDK 10.0 (minimum `10.0.103`, `rollForward: latestFeature`). CI and publishing install the SDK from this file. Additional SDK installations may provide older test runtimes. See the [SDK selection policy](policies/development.md#net-sdk-selection).

## Solution items

The root solution exposes repository-level documents and configuration under `solutionItems`, GitHub files and maintenance scripts in matching subfolders, and documentation under `docs/`. The list is explicit, not a filesystem glob; keep links up to date when files change. See the [solution items policy](policies/development.md#solution-items).
## Repository validation

`scripts/Validate-Repository.ps1` enforces the required repository documents,
their navigation links, and complete, valid Solution Items. The
`.github/workflows/repository-policy.yml` workflow runs it for pushes and pull
requests. See the [development policy](policies/development.md#repository-validation).

## Formatting

The root `.editorconfig` defines the portable formatting baseline. Existing repositories may add stricter C# or analyzer-specific settings. See the [development policy](policies/development.md#formatting-baseline).

## Testing

The test suite is in `tests/Toolkit.Debouncing.Tests` and exercises throttling, parameter forwarding, and replacement of pending debounce callbacks. Because it exercises WPF dispatching, CI runs it on Windows.

## Versioning and release tags

The library uses MinVer 8 with stable tags in the `vMAJOR.MINOR.PATCH` format. The tag without its `v` prefix is the NuGet package version. The migration from the former explicit `1.0.9` version starts with `v1.0.10` (or a higher stable version). Manual publishing accepts exactly one matching tag at HEAD and verifies the package ID and version before pushing to NuGet.
