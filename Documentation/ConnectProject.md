# Connect Project
Here you can see instructions for connecting **NetArgumentParser** to your project.

## General
At fitst, you need to clone **NetArgumentParser** repository and [add reference](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-add-reference) to the library:
```
dotnet add path_to_your_project.csproj reference path_to_lib/Core/NetArgumentParser
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

## Step-By-Step Connection
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