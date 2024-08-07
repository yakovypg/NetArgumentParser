<h1 align="center">NetArgumentParser</h1>
<p align="center">
  <img alt="netargumentparser" height="200" src="https://i.giphy.com/media/v1.Y2lkPTc5MGI3NjExOXFmc21tZjN4OGY2cXRyaTNqdzQwdHY3ZmRyOXdib240cmY5M2hsZCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/z5zWcuq8qDfrl9B3Tp/giphy.gif" />
</p>

<p align="center">
  <a href="https://github.com/yakovypg/NetArgumentParser/blob/main/LICENSE">
    <img src="https://img.shields.io/badge/License-GPLv3-darkyellow.svg" alt="license" />
  </a>
  <img src="https://img.shields.io/badge/Version-1.0.0-red.svg" alt="version" />
  <img src="https://img.shields.io/badge/C%23-.NET 8-blue" />
</p>

## About
**NetArgumentParser** is a cross-platform, free and open source library for parsing command-line options, arguments and subcommands.

[![Contributors](https://img.shields.io/github/contributors/yakovypg/NetArgumentParser)](https://github.com/yakovypg/NetArgumentParser/graphs/contributors)
[![Build Status](https://img.shields.io/github/actions/workflow/status/yakovypg/NetArgumentParser/dotnet.yml?branch=main)](https://github.com/yakovypg/NetArgumentParser/actions/workflows/dotnet.yml?query=branch%3Amain)

## Table of contents
*    [Main Features](#main-features)
*    [Quick Start](#quick-start)
     *    [Build Project](#build-project)
     *    [Test Project](#test-project)
     *    [Connect Project](#connect-project)
*    [Project Status And TODO List](#project-status-and-todo-list)
*    [Documentation](#documentation)
*    [Development](#development)
*    [Contributing](#contributing)
*    [License](#license)

## Main Features
This library supports the following main features:
- Parse short-named options (such as `-v`).
- Parse long-named options (such as `--version` or `-version`).
- Parse windows-based options (such as `/v` or `/version`).
- Parse compound options (such as `-lah`).
- Parse nested subcommands (such as `app subcommand subsubcommand`).
- Extract extra arguments.
- Provide a lot of default option types.
- Support custom options and converters.
- Configure command-line help generation and output stream.

Many other features you can find in [documentation](#documentation).

## Quick Start
To start working with the library you need to [connect](#connect-project) it to your project. If you are going to connect a library cloned from a repository, you may want to [build](#build-project) and [test](#test-project) it before doing so.

### Build Project
You can [build](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build) the library with the following command, which should be run from the root of the library project.
```
dotnet build
```

### Test Project
You can [test](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test) the library with the following command, which should be run from the root of the library project.
```
dotnet test
```

### Connect Project
You can find instructions for connecting **NetArgumentParser** to your project [here](Documentation/ConnectProject.md).

## Project Status And TODO List
**NetArgumentParser** is currently under development. There are some features that need to be added to the project:
- Add support of reflection-based configuring option set using special attributes.
- Add NuGet package.

## Documentation
You can read our documentation in the [DOCUMENTATION.md](DOCUMENTATION.md).

## Development
The project is developed on the .NET 8.0 platform. To continue development you will need the .NET SDK and .NET Runtime of the appropriate version.

## Contributing
Contributions are welcome, have a look at the [CONTRIBUTING.md](CONTRIBUTING.md) document for more information.

## License
The project is available under the [GPLv3](LICENSE) license.
