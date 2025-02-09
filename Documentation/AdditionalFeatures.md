# Additional Features
**NetArgumentParser** supports some useful options, which you will learn about below.

## Table of Contents
*    [Negative Numbers & Scientific Notation](#negative-numbers--scientific-notation)
*    [Use `option=value` Syntax](#use-optionvalue-syntax)
*    [Parse Known Arguments](#parse-known-arguments)
*    [Skip Arguments](#skip-arguments)
*    [Getting Info About Handled Options And Subcommands](#getting-info-about-handled-options-and-subcommands)
*    [Add Choices To Description](#add-choices-to-description)

## Negative Numbers & Scientific Notation
As you can see here, **NetArgumentParser** supports negative numbers and scientific notation.

```cs
int? offset = null;
double? angle = null;

var offsetOption = new ValueOption<int>("offset",
    afterValueParsingAction: t => offset = t);

var angleOption = new ValueOption<double>("angle", "a",
    description: "angle by which you want to rotate the image",
    afterValueParsingAction: t => angle = t);

var parser = new ArgumentParser();
parser.AddOptions(offsetOption, angleOption);

parser.Parse(new string[] { "--offset", "-45", "--angle", "-1e-1" });
// offset: -45
// angle: -0.1
```

## Use `option=value` Syntax
**NetArgumentParser** supports `option=value` syntax. Please note that this syntax doesn't combine with the ability to capture multiple values.

```cs
int? angle = null;
List<string> inputFiles = [];

var angleOption = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    afterValueParsingAction: t => angle = t);

var filesOption = new MultipleValueOption<string>("input", "i",
    description: "files that need to be processed",
    contextCapture: new OneOrMoreContextCapture(),
    afterValueParsingAction: t => inputFiles = new List<string>(t));

var parser = new ArgumentParser();
parser.AddOptions(angleOption, filesOption);

parser.ParseKnownArguments(
    new string[] { "--angle=45", "--input=file0.txt", "file1.txt" },
    out List<string> extraArguments);

// angle: 45
// inputFiles: [file0.txt]
// extraArguments: [file1.txt]
```

## Parse Known Arguments
Sometimes a program may need to parse only part of the command-line arguments and pass the remaining arguments to another program. In this case, the `ParseKnownArguments` function may be useful. It works in much the same way as the `Parse` function, except that it doesn't throw an exception if there are additional arguments. Instead, it allows you to get a list of the extra arguments.

```cs
bool verbose = false;

var option = new FlagOption("verbose", "v",
    description: "be verbose",
    afterHandlingAction: () => verbose = true);

var parser = new ArgumentParser();
parser.AddOptions(option);

parser.ParseKnownArguments(new string[] { "-v", "10", "-a" }, out List<string> extraArguments);
// extraArguments: [10, -a]
```

## Skip Arguments
Sometimes a program may need to skip some arguments from the beginning of the argument list and handle them in a special way. In this case, the `NumberOfArgumentsToSkip` property may be useful. With it, you can specify the number of arguments to be skipped.

```cs
var parser = new ArgumentParser()
{
    NumberOfArgumentsToSkip = 1
};

// ...
// parser.AddOptions(...);

parser.Parse(new string[] { "merge", "./first.txt", "./second.txt" });
// Only ["./first.txt", "./second.txt"] will be parsed
```

## Getting Info About Handled Options And Subcommands
You can get information about options and subcommands that were handled during argument parsing. To do this, get the `ParseArgumentsResult` and access its properties. `HandledOptions` contains the options in the order they are handled. `HandledSubcommands` contains the subcommands in the order they are handled.

```cs
var verboseOption = new FlagOption("verbose", "v");
var angleOption = new ValueOption<int>("angle", "a");
var offsetOption = new ValueOption<int>("offset", "o");

var filesOption = new MultipleValueOption<string>(
    "input", "i", contextCapture: new OneOrMoreContextCapture());

var parser = new ArgumentParser();
parser.AddOptions(verboseOption, angleOption);

Subcommand compressSubcommand = parser.AddSubcommand("compress", "compress images");
compressSubcommand.AddOptions(filesOption, offsetOption);

string[] args = ["--verbose", "compress", "-i", "img.jpg", "img.png"];
ParseArgumentsResult result = parser.ParseKnownArguments(args, out _);

// result.HandledOptions: verboseOption (without value), filesOption (with values img.jpg, img.png)
// result.HandledSubcommands: compressSubcommand
```

## Add Choices To Description
You can add choices to the value option description automatically. To do this, you can use `AddChoicesToDescription()` method as follows:

```cs
var angleOption = new ValueOption<int>(
    longName: "angle",
    shortName: "a",
    description: "Angle",
    choices: [0, 45, 90]);

angleOption.AddChoicesToDescription();

var namesOption = new MultipleValueOption<string>(
    longName: "input",
    shortName: "i",
    description: "Names",
    contextCapture: new OneOrMoreContextCapture(),
    choices: [["John", "Max"], ["James", "Michael"]]);

namesOption.AddChoicesToDescription(
    separator: ", ",
    prefix: " (",
    postfix: ")",
    arraySeparator: "; ",
    arrayPrefix: "[",
    arrayPostfix: "]");

var fileModeOption = new EnumValueOption<FileMode>(
    longName: "mode",
    shortName: "m",
    description: "File mode",
    useDefaultChoices: true);

fileModeOption.AddChoicesToDescription(
    separator: " | ",
    prefix: ". Choices: { ",
    postfix: " }");

// Angle (0, 45, 90)
Console.WriteLine(angleOption.Description);

// Names ([John, Max]; [James, Michael])
Console.WriteLine(namesOption.Description);

// File mode. Choices: { CreateNew | Create | Open | OpenOrCreate | Truncate | Append }
Console.WriteLine(fileModeOption.Description);
```

All value options implement interface `IValueOptionDescriptionDesigner`, so if you don't want to manually call method `AddChoicesToDescription()` for each option, you can get all options from parser using `GetAllOptions()` method, filter them and then use a loop.

```cs
var angleOption = new ValueOption<int>(
    longName: "angle",
    shortName: "a",
    description: "Angle",
    choices: [0, 45, 90]);

var namesOption = new MultipleValueOption<string>(
    longName: "input",
    shortName: "i",
    description: "Names",
    contextCapture: new OneOrMoreContextCapture(),
    choices: [["John", "Max"], ["James", "Michael"]]);

var fileModeOption = new EnumValueOption<FileMode>(
    longName: "mode",
    shortName: "m",
    description: "File mode",
    useDefaultChoices: true);

var parser = new ArgumentParser();
parser.AddOptions(angleOption, namesOption, fileModeOption);

var designers = parser.GetAllOptions()
    .Where(t => t is IValueOptionDescriptionDesigner)
    .Cast<IValueOptionDescriptionDesigner>();

foreach (var designer in designers)
{
    designer.AddChoicesToDescription();
}

// Angle (0, 45, 90)
Console.WriteLine(angleOption.Description);

// Names ([John, Max]; [James, Michael])
Console.WriteLine(namesOption.Description);

// File mode (CreateNew, Create, Open, OpenOrCreate, Truncate, Append)
Console.WriteLine(fileModeOption.Description);
```
