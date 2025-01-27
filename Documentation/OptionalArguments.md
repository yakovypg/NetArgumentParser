# Optional Arguments
Let's consider **optional arguments**. Optional arguments start with `-`, `--` or `/`, e.g., `-h`, `--verbose` or `/m`.

## Table of Contents
*    [Flag Options](#flag-options)
     *    [Help Option](#help-option)
     *    [Version Option](#version-option)
     *    [Counter Option](#counter-option)
*    [Value Options](#value-options)
     *    [Multiple Value Options](#multiple-value-options)
     *    [Enum Value Options](#enum-value-options)
*    [Final Options](#final-options)
*    [Custom Options](#custom-options)
*    [Option Groups](#option-groups)
*    [Mutual Exclusion](#mutual-exclusion)

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

### Counter Option
**Counter option** is a special flag option that can be handled multiple times. It can help you to work with counter variable.

Here is an example of creating counter option and using it in the parser:

```cs
int level = default;
var option = new CounterOption(string.Empty, "V", increaseCounter: () => level++);

var parser = new ArgumentParser()
{
    RecognizeCompoundOptions = true
};

parser.AddOptions(option);
parser.Parse(new string[] { "-VVVV", "-V" });
// level: 5
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

## Final Options
**Final options** are options that are handled before all others. After the final option is handled, the remaining options (including final ones) aren't handled. Examples of final options are help option and version option.

Note that options of any type can be final. To make an option final, you need to set the corresponding value to the `isFinal` parameter of the option constructor.

Here is an example of creating final option and using it in the parser:

```cs
int? angle = null;
int? width = null;
int? final = null;

var angleOption = new ValueOption<int>("angle", "a",
    afterValueParsingAction: t => angle = t);

var widthOption = new ValueOption<int>("width", "w",
    afterValueParsingAction: t => width = t);

var finalOption = new ValueOption<int>("final", string.Empty,
    isFinal: true,
    afterValueParsingAction: t => final = t);

var parser = new ArgumentParser();
parser.AddOptions(angleOption, widthOption, finalOption);

parser.Parse(new string[] { "-a", "40", "--final", "100", "-w", "1920" });
// angle: null
// width: null
// final: 100
```

## Custom Options
You can create your own options. To do this you need to inherit your class from the `ICommonOption` interface and implement it. You can also use an existing option class as a base class. See examples of this kind of inheritance, for example, by looking at the implementation of the `FlagOption` and `EnumValueOption` classes. Next, you can use this class in the same way as the standard ones.

## Option Groups
Options can be divided into groups. This division may be useful when there are a lot of options.

```cs
var parser = new ArgumentParser();
parser.DefaultGroup.Header = "Default group:";

var option = new ValueOption<int>("angle");
var additionalOption = new FlagOption("verbose");

parser.AddOptions(option);

OptionGroup<ICommonOption> group = parser.AddOptionGroup("Additional group:", "Description\n");
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

## Mutual Exclusion
If you add options to the mutually exclusive group, **NetArgumentParser** will make sure that only one of the arguments in this group was present on the command line. Note that title and description of mutually exclusive group will not be displayed in the help output. Moreover, you cannot add options to the option set using mutually exclusive group. This group is intended only to mark options that are already added.

You can create mutually exclusive group using `AddMutuallyExclusiveOptionGroup()` method of the `ArgumentParser` class. Options can be added to this group by passing them to method `AddMutuallyExclusiveOptionGroup()` or by calling method `AddOptions()` of the created group.

```cs
string? name = default;

var nameOption = new ValueOption<string>("name", afterValueParsingAction: t => name = t);
var nickOption = new ValueOption<string>("nick", afterValueParsingAction: t => name = t);

var parser = new ArgumentParser();
parser.AddOptions(options);

MutuallyExclusiveOptionGroup<ICommonOption> group = parser.AddMutuallyExclusiveOptionGroup(
    "group",
    "description",
    [nameOption, nickOption]);

parser.Parse(new string[] { "--name", "John" }); // name: John
parser.Parse(new string[] { "--nick", "mr.john" }); // nick: mr.john
parser.Parse(new string[] { "--name", "John", "--nick", "mr.john" }); // Error
```
