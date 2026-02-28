# Parser Generation Using Attributes
You can generate `ArgumentParser` using special class provided parser configuration. The configuration is specified in the form of properties and special attributes. This allows you to quickly, easily and conveniently configure the parser without the need to manually create lots of options and objects/variables that store the result of command-line arguments parsing.

## Table of Contents
*    [Attributes](#attributes)
     *    [Configuration Attributes](#configuration-attributes)
     *    [Option Attributes](#option-attributes)
          *    [Add value restriction](#add-value-restriction)
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
        addDefaultValueToDescription: false,
        valueRestriction: null)
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
        contextCaptureType: ContextCaptureType.OneOrMore,
        addDefaultValueToDescription: false,
        valueRestriction: null)
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
        numberOfItemsToCapture: 3,
        addDefaultValueToDescription: false,
        valueRestriction: null)
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
        aliases: [],
        beforeParseChoices: null,
        addChoicesToDescription: false,
        addBeforeParseChoicesToDescription: false,
        addDefaultValueToDescription: false,
        valueRestriction: null)
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
        addDefaultValueToDescription: true,
        valueRestriction: null)
    ]
    public double? Angle { get; set; }
}

internal record Point(double X, double Y, double Z);
```

Note that attributes allow you to add default value, choices and before parse choices to the option description (if the corresponding option supports it) using `addDefaultValueToDescription`, `addChoicesToDescription`, and `addBeforeParseChoicesToDescription` parameters, so you don't need to find the generated options to do this.

#### Add value restriction
You can add a common value restriction using `valueRestriction` parameter. This is specified through a string that will be converted into an actual restriction by a special parser. The format of this string is as follows:

```
predicate1_name parameter1 ... parameterN
...
logical_operator predicateK_name parameter1 ... parameterM
?value_not_satisfy_restriction_message
```

In other words, a string consists of one or more predicates, each separated by a newline character `\n`. Following the predicate name, its parameters are listed. A logical connective (`AND` or `OR`) may precede the predicate name; if omitted, it defaults to `AND`. A line that begins with the `?` character specifies a message to be displayed if the option value doesn't satisfy the restriction. However, this message can be omitted.

It is also important to note that parentheses are not supported, and logical connectives will be evaluated in the order in which they are specified. Thus, `x OR y AND z` will actually be interpreted as `(x OR y) AND z`. Additionally, logical connectives have aliases: for `OR`, they are `||` and `|`, while for `AND`, they are `&&` and `&`.

The following predicates are available:
1. `equal` (`==`, `=`): takes a single parameter. The option value must equal this parameter. The parameter type must be double, and the option value type must have the overloaded `==` operator.
2. `notequal` (`!=`, `<>`): takes a single parameter. The option value must differ from this parameter. The parameter type must be double, and the option value type must have the overloaded `!=` operator.
3. `less` (`<`): takes a single parameter. The option value must be less than this parameter. The parameter type must be double, and the option value type must have the overloaded `<` operator.
4. `lessorequal` (`<=`): takes a single parameter. The option value must be less than or equal to this parameter. The parameter type must be double, and the option value type must have the overloaded `<=` operator.
5. `greater` (`>`): takes a single parameter. The option value must be greater than this parameter. The parameter type must be double, and the option value type must have the overloaded `>` operator.
6. `greaterorequal` (`>=`): takes a single parameter. The option value must be greater than or equal to this parameter. The parameter type must be double, and the option value type must have the overloaded `>=` operator.
7. `inrange` (`minmax`): takes two parameters. The option value must be greater than or equal to the first parameter and less than or equal to the second parameter. The parameter types must be double, and the option value type must have the overloaded `>=` and `<=` operators.
8. `oneof` (`inlist`): takes one or more parameters. The option value, converted to a string, must equal one of the specified parameters.
9. `match` (`regex`): takes a single parameter. The option value, converted to a string, must match the specified parameter representing a regular expression. Anything written after the first space will be avaluated as a regular expression, so it can contain spaces.
10. `directoryexists` (`directory`): takes no parameters. The option value must be a string representing the path to an existing directory.
11. `fileexists` (`file`): takes no parameters. The option value must be a string representing the path to an existing file.
12. `maxfilesize` (`maxsize`): takes a single parameter. The option value must be a string representing the path to a file whose size is less than or equal to this parameter (in bytes).
13. `extension` (`ext`): takes one or more parameters. The option value must be a string representing the path to a file whose extension matches one of the specified parameters. The dot in the file extension is optional, and its case (uppercase or lowercase) doesn't matter.

Examples of simple restrictions are provided below:
1. `== 5`: the option value must be equal to 5.
2. `!= 5`: the option value must be different from 5.
3. `< 5`: the option value must be less than 5.
4. `<= 5`: the option value must be less than or equal to 5.
5. `> 5`: the option value must be greater than 5.
6. `>= 5`: the option value must be greater than or equal to 5.
7. `inrange 0 5`: the option value must be within the range from 0 to 5.
8. `oneof 1 3 6`: the option value must be one of the values: 1, 3 or 6.
9. `match ^[A-Z][a-z]*$`: the option value must match the regular expression `^[A-Z][a-z]*$`.
10. `directoryexists`: the option value must be a string representing the path to an existing directory.
11. `fileexists`: the option value must be a string representing the path to an existing file.
12. `maxfilesize 10240`: the option value must be a string representing the path to a file whose size is less than or equal to 10240 bytes.
13. `extension jpg png`: the option value must be a string representing the path to a file whose extension matches either `jpg` or `png`.

Examples of complex restrictions are provided below:
1. `< -100\nOR > 100\nOR oneof 1 5 7 10\nAND inrange -200 200`.
2. `fileexists\n&& extension jpg png\n?file must exists and be an image`.

Finally, here is an example of creating an option with a restriction using an attribute:

```cs
var generator = new ArgumentParserGenerator();
var parser = new ArgumentParser();
var config = new CustomParserConfig();

generator.ConfigureParser(parser, config);

parser.Parse(["--width", "1000"]); // config.Width: 1000
parser.Parse(["--width", "2000"]); // Error

[ParserConfig]
internal class CustomParserConfig
{
    [ValueOption<int>("width", valueRestriction: "> 0\n&& <= 1920\n?width must be in (0; 1920]")]
    public int Width { get; set; }
}
```

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
- Not common value restriction.
- Custom converter.
- Default value for some types.
- `ValueOption` choices for some types.
- `MultipleValueOption` choices.
- Custom after handing action and after value parsing action.

However, you can find the option using `FindOptions()`, `FindFirstOptionByLongName()`, or a similar method, and then configure it.

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
