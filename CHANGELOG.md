# Changelog

All notable changes to this project are documented in this file.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

## [1.0.10] - 2026-09-14

### Added

- Validate required repository files, navigation links, and Solution Items in CI.

- Development policy and repository guide under `docs/`, with required reading links in `AGENTS.md` and navigation from `README.md`.

- Automated tests for throttling, parameter forwarding, and replacement of pending debounce callbacks.

### Fixed

- Align the default dispatcher priority of generic debounce and throttle overloads with their implementations.

### Changed

- Standardize GitHub Actions workflow filenames and display names by responsibility.

- Derive package versions from stable `vMAJOR.MINOR.PATCH` tags with MinVer and validate packages before publishing.

- Separate continuous integration from manually triggered NuGet publishing.

- Update SonarAnalyzer.CSharp to 10.33.0.1635.

- Configure xUnit v3 test execution through Microsoft.Testing.Platform and fail test runs when no tests are discovered.

- Establish a shared EditorConfig baseline and use the repository-policy validator as the single structural CI check.

- Complete solution items for repository documents, configuration, workflows and maintenance scripts; document the shared layout.

- Standardize local and CI SDK selection on stable .NET 10.0 through global.json, restrict roll-forward to that major/minor line, and configure setup-dotnet to read the file.

- Show documentation in Visual Studio Solution Explorer under a `docs` solution folder with matching subfolders.
