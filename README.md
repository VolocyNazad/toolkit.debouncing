# Toolkit.Debouncing

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![VolocyNazad](https://img.shields.io/badge/VolocyNazad-blue.svg)](https://github.com/VolocyNazad)

> WPF dispatcher-based helpers for debouncing and throttling callbacks.

`Toolkit.Debouncing` collapses bursts of rapid callbacks on a WPF `Dispatcher`. It is a regular .NET
library with no Revit API dependency.

## Features

- `Debounce` runs the callback only after the interval has passed without another call, so only the
  last call in a burst runs.
- `Throttle` runs the callback immediately and then at most once per interval.
- Generic overloads forward a parameter to the callback.
- Optional `DispatcherPriority` and `Dispatcher` arguments; the application dispatcher is the
  default.
- `IDebounceDispatcher` allows dependency injection and test doubles.

## Installation

```
dotnet add package VolocyNazad.Toolkit.Debouncing
```

## Usage

```csharp
using System;
using System.Windows.Threading;
using Toolkit.Debouncing;

using var dispatcher = new DebounceDispatcher();

// Coalesce rapid text changes into one search.
dispatcher.Debounce(300, () => Search());

// Run a progress update at most once per second.
dispatcher.Throttle<string>(1000, message => Status = message, "Working...");

// Choose the priority explicitly when the callback must run before rendering.
dispatcher.Debounce(100, () => Invalidate(), DispatcherPriority.Render);
```

Register `IDebounceDispatcher` in a dependency-injection container to inject the helper into view
models; dispose the dispatcher to stop a pending callback.

## Requirements

- .NET SDK 10.0.103+ (see `global.json`)
- `net48`, `net8.0-windows` or `net9.0-windows`

## License

MIT, see [LICENSE.txt](LICENSE.txt).

## Development documentation

- [Repository policy index](docs/policies/README.md)
- [Repository guide and technology stack](docs/repository.md)

## Contributing

Read [CONTRIBUTING.md](CONTRIBUTING.md) before submitting changes.
