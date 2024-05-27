# Optional Arguments
Let's consider **optional arguments**. Optional arguments start with `-`, `--` or `/`, e.g., `-h`, `--verbose` or `/m`.

## Table of Contents
*    [Flag Options](#flag-options)
     *    [Help Option](#help-option)
     *    [Version Option](#version-option)
*    [Value Options](#value-options)
     *    [Multiple Value Options](#multiple-value-options)
     *    [Enum Value Options](#enum-value-options)
*    [Custom Options](#custom-options)
*    [Option Groups](#option-groups)

## Flag Options
**Flag options** are options without value. They make working with boolean values ​​easier.

Here is an example of creating flag option and using it in the parser:

```cs
bool verbose = false;

var option = new FlagOption("verbose", "v",
    description: "be verbose",
    afterHandlingAction: () => verbose = true);

var parser = new ArgumentParser();
parser.AddOptions(option);

parser.Parse(new string[] { "--verbose" });
// verbose: true
```

### Help Option
**Help option** is a special final flag option whose default action is to print the help and exit the program.

This option is automatically added to the option set, so you don't have to add it explicitly:

```cs
var parser = new ArgumentParser();
parser.Parse(new string[] { "--help" });
```

You can disable automatic addition of the help option as follows:

```cs
var parser = new ArgumentParser()
{
    UseDefaultHelpOption = false
};
```

Here is an example of creating custom help option and using it in the parser:

```cs
var parser = new ArgumentParser()
{
    UseDefaultHelpOption = false
};

var option = new HelpOption("help", "h",
    description: "show command-line help",
    afterHandlingAction: () =>
    {
        Console.WriteLine(parser.GenerateProgramDescription());
        Environment.Exit(0);
    });

parser.AddOptions(option);
parser.Parse(new string[] { "--help" });
```

### Version Option
**Version option** is a special final flag option whose default action is to print the version information and exit the program.

This option is automatically added to the option set, so you don't have to add it explicitly:

```cs
var parser = new ArgumentParser();
parser.Parse(new string[] { "--version" });
```

You can disable automatic addition of the help option as follows:

```cs
var parser = new ArgumentParser()
{
    UseDefaultVersionOption = false
};
```

Here is an example of creating custom version option and using it in the parser:

```cs
var parser = new ArgumentParser()
{
    UseDefaultVersionOption = false
};

var option = new VersionOption("version", "v",
    description: "show version information",
    afterHandlingAction: () =>
    {
        Console.WriteLine(parser.ProgramVersion);
        Environment.Exit(0);
    });

parser.AddOptions(option);
parser.Parse(new string[] { "--version" });
```

## Value Options
**Value options** are options with value. The value type can be anything. However, for types that don't have a default converter, you will need to add a [custom converter](CustomConverters.md).

Here is an example of creating value option and using it in the parser:

```cs
int? angle = null;

var option = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    afterValueParsingAction: t => angle = t);

var parser = new ArgumentParser();
parser.AddOptions(option);

parser.Parse(new string[] { "--angle", "45" });
// angle: 45
```

### Multiple Value Options
**Multiple value options** are options whose value is a list. The single value type can be anything. However, for types that don't have a default converter, you will need to add a [custom converter](CustomConverters.md).

Here is an example of creating multiple value option and using it in the parser:

```cs
List<string> inputFiles = [];

var option = new MultipleValueOption<string>("input", "i",
    description: "files that need to be processed",
    contextCapture: new OneOrMoreContextCapture(),
    afterValueParsingAction: t => inputFiles = new List<string>(t));

var parser = new ArgumentParser();
parser.AddOptions(option);

parser.Parse(new string[] { "-i", "file0.txt", "file1.txt" });
// inputFiles: [file0.txt, file1.txt]
```

One of the important features of the multiple value option is context capture. You can find out more about it in the [corresponding](OptionalArgumentsConfig.md#context-capture) section of the documentation.

### Enum Value Options
**Enum value options** are options whose value is enum. The value type can be any enum.

Here is an example of creating enum value option and using it in the parser:

```cs
StringSplitOptions? splitOption = null;

var option = new EnumValueOption<StringSplitOptions>("split-option", string.Empty,
    description: "specifies how the String.Split method should split a string",
    afterValueParsingAction: t => splitOption = t)

var parser = new ArgumentParser();
parser.AddOptions(option);

parser.Parse(new string[] { "--split-option", "TrimEntries" });
// splitOption: StringSplitOptions.TrimEntries
```

## Custom Options
You can create your own options. To do this you need to inherit your class from the `ICommonOption` interface. You can also use an existing option class as a base class. See examples of this kind of inheritance, for example, by looking at the implementation of the `FlagOption` and `EnumValueOption` classes.

## Option Groups
Options can be divided into groups. This division may be useful when there are a lot of options.

```cs
var parser = new ArgumentParser();
parser.DefaultGroup.Header = "Default group:";

var option = new ValueOption<int>("angle");
var additionalOption = new FlagOption("verbose");

parser.AddOptions(option);

OptionGroup<ICommonOption> group = parser.AddOptionGroup("Additional group:");
group.AddOptions(additionalOption);

parser.Parse(new string[] { "--help" });
```

In particular, this division will be displayed in the help output. It will be like the following:

```
Usage: [--angle ANGLE] [--verbose] [--help] [--version]

Default group:
  --angle ANGLE  rotation angle
  --help, -h     show command-line help
  --version      show version information

Additional group:
  --verbose      be verbose
```
