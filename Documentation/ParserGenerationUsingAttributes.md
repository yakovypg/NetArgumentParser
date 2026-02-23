# Parser Generation Using Attributes
You can generate `ArgumentParser` using special class provided parser configuration. The configuration is specified in the form of properties and special attributes. This allows you to quickly, easily and conveniently configure the parser without the need to manually create lots of options and objects/variables that store the result of command-line arguments parsing.

## Table of Contents
*    [Attributes](#attributes)
     *    [Configuration Attributes](#configuration-attributes)
     *    [Option Attributes](#option-attributes)
     *    [Group Attributes](#group-attributes)
     *    [Subcommand Attributes](#subcommand-attributes)
*    [Argument Parser Generation](#argument-parser-generation)
*    [Limitations](#limitations)

## Attributes
Properties marked by special attributes will provide configuration for options and subcommands. These properties must be public and have `get` accessor. Furthermore, properties for options must have `set` accessor.

### Configuration Attributes

To mark class as parser generator configuration provider you should use `ParserConfigAttribute` attribute. Classes without this attrubute cannot be used as config.

```cs
[ParserConfig]
internal class CustomParserConfig
{
}
```

### Option Attributes

There are attributes for all types of options. Their creation is no different from the creation of the corresponding options. Only some minor changes in input parameters are possible. However, there are some limitations described in a special [section](#limitations).

Below is an example of configuration with attributes for all option types.

```cs
[ParserConfig]
internal class CustomParserConfig
{
    [HelpOption(
        longName: "help",
        shortName: "h",
        description: "",
        isHidden: false,
        aliases: [])]
    public bool ShowHelp { get; set; }

    [VersionOption(
        longName: "version",
        shortName: "v",
        description: "",
        isHidden: false,
        aliases: [])]
    public bool ShowVersion { get; set; }

    [CounterOption(
        longName: "verbosity",
        shortName: "V",
        description: "",
        isRequired: false,
        isHidden: false,
        isFinal: false,
        aliases: [])
    ]
    public BigInteger? VerbosityLevel { get; set; }

    [EnumValueOption<FileMode>(
        longName: "mode",
        shortName: "m",
        description: "",
        metaVariable: "M",
        isRequired: true,
        isHidden: false,
        isFinal: false,
        useDefaultChoices: false,
        aliases: ["file-mode"],
        choices: [FileMode.Create, FileMode.Open],
        beforeParseChoices: ["Create", "Open"],
        addChoicesToDescription: false,
        addBeforeParseChoicesToDescription: true,
        addDefaultValueToDescription: false)
    ]
    public FileMode Mode { get; set; }

    [FlagOption(
        longName: "ignore-case",
        shortName: "",
        description: "",
        isRequired: false,
        isHidden: true,
        isFinal: false,
        aliases: [])]
    public bool? IgnoreCase { get; set; }

    [MultipleValueOption<string>(
        longName: "files",
        shortName: "f",
        description: "",
        metaVariable: "",
        isRequired: true,
        isHidden: false,
        isFinal: false,
        aliases: ["input", "input-files"],
        contextCaptureType: ContextCaptureType.OneOrMore)
    ]
    public List<string> InputFiles { get; set; }

    [MultipleValueOption<string>(
        longName: "persons",
        shortName: "P",
        description: "persons",
        metaVariable: "",
        isRequired: false,
        isHidden: false,
        isFinal: false,
        ignoreCaseInChoices: true,
        ignoreOrderInChoices: true,
        aliases: ["ps"],
        contextCaptureType: ContextCaptureType.Fixed,
        numberOfItemsToCapture: 3)
    ]
    [OptionGroup("complex-values", "", "")]
    public List<string> Persons { get; set; }

    [ValueOption<Point>(
        longName: "point",
        shortName: "p",
        description: "",
        metaVariable: "",
        isRequired: false,
        isHidden: false,
        isFinal: false,
        aliases: [])
    ]
    public Point? Point { get; set; }

    [ValueOption<double>(
        defaultValue: 45,
        longName: "angle",
        shortName: "a",
        description: "",
        metaVariable: "A",
        isRequired: false,
        isHidden: false,
        isFinal: false,
        aliases: [],
        choices: [0, 45, 90],
        beforeParseChoices: ["0", "45", "90"],
        addChoicesToDescription: true,
        addBeforeParseChoicesToDescription: false,
        addDefaultValueToDescription: true)
    ]
    public double? Angle { get; set; }
}

internal record Point(double X, double Y, double Z);
```

Note that attributes allow you to add default value, choices and before parse choices to the option description (if the corresponding option supports it) using `addDefaultValueToDescription`, `addChoicesToDescription`, and `addBeforeParseChoicesToDescription` parameters, so you don't need to find the generated options to do this.

### Group Attributes
Goups can be configured using `OptionGroupAttribute` attribute. In addition to specifying the group header and description, you should specify the group ID. It is necessary for the correct placement of options, since groups can have the same header. Options that you want to put in the same group must be marked with an attribute with the same ID. You should't specify header and description for all group attributes with same id. It is enough to do this for only one attribute.

```cs
[ParserConfig]
internal class CustomParserConfig
{
    [FlagOption("first-name", "n")]
    [OptionGroup("id1", "Name flags", "descripton-1")]
    public bool ShowFirstName { get; set; }

    [FlagOption("second-name", "N")]
    [OptionGroup("id1", "", "")]
    public bool ShowSecondName { get; set; }

    [FlagOption("guide", "g")]
    [OptionGroup("id2", "Other options", "")]
    public bool ShowGuide { get; set; }

    [CounterOption("verbosity", "v")]
    [OptionGroup("id2", "", "")]
    public int Verbosity { get; set; }
}
```

Attributes for mutually exclusive groups are set in a similar manner. But unlike regular groups, mutually exclusive groups can contain options from different levels of subcommands.

```cs
[ParserConfig]
internal class CustomParserConfig
{
    [FlagOption("first-name", "n")]
    [MutuallyExclusiveOptionGroup("id", "name", "descripton")]
    public bool ShowFirstName { get; set; }

    [FlagOption("second-name", "N")]
    [MutuallyExclusiveOptionGroup("id", "", "")]
    public bool ShowSecondName { get; set; }

    [Subcommand("status", "description")]
    public StatusSubcommand Status { get; }
}

internal class StatusSubcommand
{
    [FlagOption("guide", "g")]
    [MutuallyExclusiveOptionGroup("id", "", "")]
    public bool ShowGuide { get; set; }
}
```

### Subcommand Attributes
Subcommands can be configured using `SubcommandAttribute` attribute. The corresponding property should contain instance of custom class that provide configuration for subcommand options and nested subcommands.

```cs
[ParserConfig]
internal class CustomParserConfig
{
    public CustomParserConfig()
    {
        Status = new();
    }

    [Subcommand("status", "description")]
    public StatusSubcommand Status { get; }
}

internal class StatusSubcommand
{
    public StatusSubcommand()
    {
        Update = new();
    }

    [CounterOption("verbosity", "v")]
    public int Verbosity { get; set; }

    [Subcommand("update", "description")]
    public UpdateSubcommand Update { get; }
}

internal class UpdateSubcommand
{
    [FlagOption("remote", "r")]
    public bool Remote { get; set; }
}
```

## Argument Parser Generation

At first, you need to create custom class and mark it by `ParserConfigAttribute` attribute.

```cs
[ParserConfig]
internal class CustomParserConfig
{
}
```

Then you can add properties with special attributes that will provide configuration for options and subcommands.

```cs
[ParserConfig]
internal sealed class CustomParserConfig
{
    [ValueOption<DateTime>("date", "d")]
    public DateTime BirthDate { get; set; }
}
```

Finally, you should create new instances of argument parser, argument parser generator and parser configuration classes and perform `ConfigureParser()` method.

```cs
var generator = new ArgumentParserGenerator();
var parser = new ArgumentParser();
var config = new CustomParserConfig();

generator.ConfigureParser(parser, config);
```

After this, the parser will be configured and can be used for your own purposes.

```cs
parser.Parse(["--date", "01.01.2025"]);
// config.BirthDate: 1/1/2025 12:00:00AM
```

## Limitations
Using attributes in C# imposes a restriction on their argument types. An attribute argument must be a constant value or an array of such values. So the following option configurations cannot be defined directly via attributes:
- Value restriction.
- Custom converter.
- Default value for some types.
- `ValueOption` choices for some types.
- `MultipleValueOption` choices.
- Custom after handing action and after value parsing action.

However, you can find the option using `FindOptions()` method and further configure it.

```cs
var generator = new ArgumentParserGenerator();
var parser = new ArgumentParser();
var config = new CustomParserConfig();

generator.ConfigureParser(parser, config);

ICommonOption? foundOption = parser
    .FindOptions(t => t.LongName == "date", true)
    .FirstOrDefault();

if (foundOption is ValueOption<DateTime> birthDateOption)
{
    birthDateOption.ValueRestriction = new OptionValueRestriction<DateTime>(t => true);
    birthDateOption.Converter = new ValueConverter<DateTime>(t => default);
    birthDateOption.DefaultValue = new DefaultOptionValue<DateTime>(default);
    birthDateOption.ValueParsed += (_, _) => Console.WriteLine("Parsed");

    birthDateOption.ChangeChoices([default]);
    birthDateOption.ChangeBeforeParseChoices(["01.01.2025"]);
}

parser.Parse(["--date", "01.01.2025"]);
// config.BirthDate: 1/1/0001 12:00:00AM

[ParserConfig]
internal sealed class CustomParserConfig
{
    [ValueOption<DateTime>("date", "d")]
    public DateTime BirthDate { get; set; }
}
```
