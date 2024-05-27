# NetArgumentParser
**NetArgumentParser** is a cross-platform, free and open source library for parsing command-line options and arguments.

## Table of contents
*    [Main Features](#main-features)
*    [Quick Start](#quick-start)
     *    [Build Project](#build-project)
     *    [Test Project](#test-project)
     *    [Connect Project](#connect-project)
     *    [Step-By-Step Connection](#step-by-step-connection)
*    [Project Status And TODO List](#project-status-and-todo-list)
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
At fitst, you need to [add reference](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-add-reference) to the library:
```
dotnet add reference /Path/To/Lib/Core/NetArgumentParser
```

Next, you need to add the usings you need:
```cs
using NetArgumentParser;
using NetArgumentParser.Converters;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Context;
```

Finally, you can work with the parser.

### Step-By-Step Connection
Let's consider this step-by-step instructions for creating a sample project and connecting this library to it.
- Step 1: Go to the directory with your projects.
```
cd ~/Repos
```
- Step 2: Create folder for your project and go to it.
```
mkdir MyProject && cd MyProject
```
- Step 3: Create solution.
```
dotnet new sln
```
- Step 4: Create your project.
```
dotnet new console -o MyProject
```
- Step 5: Add your project to the solution.
```
dotnet sln add ./MyProject
```
- Step 6: Add folder for external projects and go to it.
```
mkdir Vendor && cd Vendor
```
- Step 7: Clone **NetArgumentParser** repository.
```
git clone https://github.com/yakovypg/NetArgumentParser.git
```
- Step 8: Go back to the root folder.
```
cd ..
```
- Step 9: Add **NetArgumentParser** to the solution. 
```
dotnet sln add Vendor/NetArgumentParser/Core/NetArgumentParser
```
- Step 10: Go to your project folder.
```
cd MyProject
```
- Step 11: Add reference to the **NetArgumentParser**.
```
dotnet add reference ../Vendor/NetArgumentParser/Core/NetArgumentParser
```
- Step 12: Open Program.cs file and try using the **NetArgumentParser**.
```cs
using System;
using System.Collections.Generic;
using NetArgumentParser;
using NetArgumentParser.Options;

int? angle = default;

var option = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    afterValueParsingAction: t => angle = t);

var parser = new ArgumentParser();
parser.AddOptions(option);
parser.ParseKnownArguments(args, out List<string> extraArguments);

Console.WriteLine($"Angle: {angle}");
Console.WriteLine($"Extra arguments: {string.Join(' ', extraArguments)}");
```
- Step 13: Build the project.
```
dotnet build -c Release
```
- Step 14: Run the created application.
```
dotnet run --angle 45 A
```

## Project Status And TODO List
**NetArgumentParser** is currently under development. There are some features that need to be added to the project:
- Add support of subcommands.
- Add counter option.
- Add support of custom option prefix characters.
- Add support of custom assignment characters.
- Add support of reflection-based configuring option set using special attributes.
- Add support of restricting the set of values for an argument.
- Add support of hidden arguments and aliases.
- Add support of import and export JSON configuration.
- Add support of parent parsers.
- Add NuGet package.

## Documentation
You can find documentation in the [Docs](Docs) folder. 

## Development
The project is developed on the .NET 8.0 platform. To continue development you will need the .NET SDK and .NET Runtime of the appropriate version.

## Contributing
Contributions are welcome, have a look at the [CONTRIBUTING.md](CONTRIBUTING.md) document for more information.

## License
The project is available under the [GPL-3.0](LICENSE) license.
