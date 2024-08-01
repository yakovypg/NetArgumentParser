<h1 align="center">NetArgumentParser</h1>
<p align="center">
  <img alt="netargumentparser" height="200" src="https://i.giphy.com/media/v1.Y2lkPTc5MGI3NjExOXFmc21tZjN4OGY2cXRyaTNqdzQwdHY3ZmRyOXdib240cmY5M2hsZCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/z5zWcuq8qDfrl9B3Tp/giphy.gif" />
</p>

<p align="center">
  <a href="https://github.com/yakovypg/NetArgumentParser/blob/main/LICENSE">
    <img src="https://img.shields.io/badge/License-GPLv3-darkyellow.svg" alt="license" />
  </a>
  <img src="https://img.shields.io/badge/Version-0.0.1-red.svg" alt="version" />
  <img src="https://img.shields.io/badge/C%23-.NET 8-blue" />
</p>

## About
**NetArgumentParser** is a cross-platform, free and open source library for parsing command-line options and arguments.

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
- Parse options starting with a minus (such as `-v`).
- Parse options starting with a double minus (such as `--version`).
- Parse options starting with a slash (such as `/v` or `/version`).
- Parse compound options (such as `-lah`).
- Parse long name options starting with a minus (such as `-version`).
- Extract extra arguments.
- Support custom converters.
- Configure command-line help generation.

## Quick Start
To start working with the library you need to build it and then connect it to your project.

### Build Project
You can [build](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build) the library with the following command, which should be run from the root of the library.
```
dotnet build
```

### Test Project
You can [test](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test) the library with the following command, which should be run from the root of the library.
```
dotnet test
```

### Connect Project
You can find instructions for connecting **NetArgumentParser** to your project [here](Docs/ConnectProject.md).

## Project Status And TODO List
**NetArgumentParser** is currently under development. There are some features that need to be added to the project:
- Add support of subcommands.
- Add support of reflection-based configuring option set using special attributes.
- Add support of restricting the set of values for an argument.
- Add support of hidden arguments and aliases.
- Add support of import and export JSON configuration.
- Add support of parent parsers.
- Add NuGet package.

## Documentation
You can find documentation in the [Docs](Docs) folder.

The main topics are:
- [Optional arguments](Docs/OptionalArguments.md)
- [Optional arguments config](Docs/OptionalArgumentsConfig.md)
- [Custom converters](Docs/CustomConverters.md)
- [Printing help](Docs/PrintingHelp.md)
- [Additional features](Docs/AdditionalFeatures.md)

## Development
The project is developed on the .NET 8.0 platform. To continue development you will need the .NET SDK and .NET Runtime of the appropriate version.

## Contributing
Contributions are welcome, have a look at the [CONTRIBUTING.md](CONTRIBUTING.md) document for more information.

## License
The project is available under the [GPL-3.0](LICENSE) license.
