using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using NetArgumentParser;
using NetArgumentParser.Attributes;
using NetArgumentParser.Converters;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Configuration;
using NetArgumentParser.Options.Context;

var generator = new ArgumentParserGenerator();
var parser = new ArgumentParser();
var config = new CustomParserConfig();

generator.ConfigureParser(parser, config);

ICommonOption? statusDateOptionCandidate = parser
    .FindOptions(t => t.LongName == "date", true)
    .FirstOrDefault();

if (statusDateOptionCandidate is ValueOption<DateTime> statusDateOption)
{
    /*statusDateOption.DefaultValue = new DefaultOptionValue<DateTime>(DateTime.Today);*/

    statusDateOption.ValueRestriction = new OptionValueRestriction<DateTime>(
        t => t > new DateTime(2000, 1, 1));

    statusDateOption.Converter = new ValueConverter<DateTime>(
        t => DateTime.Parse(t, CultureInfo.CurrentCulture));

    statusDateOption.ValueParsed += (sender, e) =>
        Console.WriteLine($"Status date: {e.Value}");

    statusDateOption.ChangeChoices([]);
}

ICommonOption? helpOptionCandidate = parser
    .FindOptions(t => t.LongName == "help", true)
    .FirstOrDefault();

if (helpOptionCandidate is HelpOption helpOption)
{
    helpOption.Handled += (_, _) =>
    {
        Console.WriteLine(parser.DescriptionGenerator?.GenerateDescription());
        Environment.Exit(0);
    };
}

_ = parser.Parse(["--help"]);

#pragma warning disable
[ParserConfig]
internal class CustomParserConfig
{
    public CustomParserConfig()
    {
        Status = new();
    }

    [HelpOption(
        longName: "help",
        shortName: "h",
        description: "show help",
        isHidden: false,
        aliases: [])]
    [OptionGroup("final", "Final options", "Final options descripton")]
    public bool ShowHelp { get; set; }

    [VersionOption(
        longName: "version",
        shortName: "v",
        description: "show version",
        isHidden: false,
        aliases: [])]
    [OptionGroup("final", "", "")]
    public bool ShowVersion { get; set; }

    [CounterOption(
        longName: "verbosity",
        shortName: "V",
        description: "verbosity level",
        isRequired: false,
        isHidden: false,
        isFinal: false,
        aliases: [])
    ]
    [MutuallyExclusiveOptionGroup("me-group", "ME options", "ME options descripton")]
    public BigInteger? VerbosityLevel { get; set; }

    [EnumValueOption<FileMode>(
        longName: "mode",
        shortName: "m",
        description: "file mode",
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
    [OptionGroup("complex-values", "", "")]
    public FileMode Mode { get; set; }

    [FlagOption(
        longName: "ignore-case",
        shortName: "",
        description: "ignore case or not",
        isRequired: false,
        isHidden: true,
        isFinal: false,
        aliases: [])]
    [MutuallyExclusiveOptionGroup("me-group", "", "")]
    public bool? IgnoreCase { get; set; }

    [MultipleValueOption<string>(
        longName: "files",
        shortName: "f",
        description: "input files",
        metaVariable: "",
        isRequired: true,
        isHidden: false,
        isFinal: false,
        aliases: ["input", "input-files"],
        contextCaptureType: ContextCaptureType.OneOrMore,
        addDefaultValueToDescription: false,
        valueRestriction: null)
    ]
    [OptionGroup("complex-values", "Complex value options", "Complex value options descripton")]
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
        description: "start point",
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
    [OptionGroup("values", "Value options", "Value options descripton")]
    public Point? Point { get; set; }

    [ValueOption<double>(
        defaultValue: 45,
        longName: "angle",
        shortName: "a",
        description: "rotation angle",
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
    [OptionGroup("values", "", "")]
    public double? Angle { get; set; }

    [Subcommand("status", "status interaction")]
    public StatusSubcommand Status { get; }
}

internal class StatusSubcommand
{
    public StatusSubcommand()
    {
        Update = new();
    }

    [ValueOption<DateTime>(
        longName: "date",
        shortName: "d",
        description: "status date",
        isRequired: false,
        isHidden: false,
        isFinal: true,
        aliases: [])]
    public DateTime StatusDate { get; set; }

    [FlagOption("configure")]
    [MutuallyExclusiveOptionGroup("me-group", "", "")]
    public bool Configure { get; set; }

    [Subcommand("update", "update status")]
    public UpdateSubcommand Update { get; }
}

internal class UpdateSubcommand
{
    [FlagOption("remote", "r")]
    public bool Remote { get; set; }
}

internal record Point(double X, double Y, double Z);
#pragma warning restore
