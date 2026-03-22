using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser;
using NetArgumentParser.Attributes;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Context;
using NetArgumentParser.Subcommands;

var generator = new ArgumentParserGenerator();
var parser = new ArgumentParser(name: CustomParserConfig.ParserName);
var config = new CustomParserConfig();

generator.ConfigureParser(parser, config);

var optionConfigurationsFactory = new OptionConfigurationsFactory();
var optionConfigurations = optionConfigurationsFactory.Create();

var optionConfigurationSetter = new OptionConfigurationSetter(optionConfigurations);
optionConfigurationSetter.SetOptionConfigurations(parser);

string[] arguments =
[
    $"--{CustomParserConfig.WidthOptionLongName}",
    "-500",
    CustomParserConfig.CustomSubcommandName,
    $"--{CustomSubcommand.FilesOptionLongName}",
    "file1.txt",
    "file2.jpg",
    "file3.mp4",
];

parser.Parse(arguments); // Ok

/*
string[] argumentsWithError =
[
    CustomParserConfig.CustomSubcommandName,
    $"--{CustomSubcommand.FilesOptionLongName}",
    "file",
];

parser.Parse(argumentsWithError); // Error: Option value 'file' doesn't satisfy the restriction
*/

string parsedFiles = string.Join(" ", config.CustomSubcommand.Files);

Console.WriteLine(config.Width);    // 500
Console.WriteLine(config.Height);   // 1080
Console.WriteLine(parsedFiles);     // file1.txt file2.jpg file3.mp4

#pragma warning disable
public class RootOptionValueConverters : OptionValueConverterProvider
{
    protected override IReadOnlyCollection<Action<ParserQuantum>> ConfigurationProviders =>
    [
        AddValueConverterForWidthOption
    ];

    private static void AddValueConverterForWidthOption(ParserQuantum parserQuantum)
    {
        AddValueConverter<int>(
            parserQuantum,
            CustomParserConfig.WidthOptionLongName,
            t => Math.Abs(int.Parse(t)));
    }
}

public class RootOptionDefaultValues : OptionDefaultValueProvider
{
    protected override IReadOnlyCollection<Action<ParserQuantum>> ConfigurationProviders =>
    [
        AddDefaultValueForWidthOption,
        AddDefaultValueForHeightOption
    ];

    private static void AddDefaultValueForWidthOption(ParserQuantum parserQuantum)
    {
        AddDefaultValue<int>(parserQuantum, CustomParserConfig.WidthOptionLongName, 1920);
    }

    private static void AddDefaultValueForHeightOption(ParserQuantum parserQuantum)
    {
        AddDefaultValue<int>(parserQuantum, CustomParserConfig.HeightOptionLongName, 1080);
    }
}

public class CustomSubcommandOptionRestrictions : OptionRestrictionProvider
{
    protected override IReadOnlyCollection<Action<ParserQuantum>> ConfigurationProviders =>
    [
        AddRestrictionForFilesOption
    ];

    private static void AddRestrictionForFilesOption(ParserQuantum parserQuantum)
    {
        AddRestrictionToMultipleValueOption<string>(
            parserQuantum,
            CustomSubcommand.FilesOptionLongName,
            t => t.All(x => x.Contains('.')));
    }
}

public class OptionConfigurationsFactory
{
    public Dictionary<string, List<IOptionConfigurationProvider>> Create()
    {
        return new()
        {
            { CustomParserConfig.ParserName, [new RootOptionValueConverters(), new RootOptionDefaultValues()] },
            { CustomParserConfig.CustomSubcommandName, [new CustomSubcommandOptionRestrictions()] }
        };
    }
}

[ParserConfig]
public sealed class CustomParserConfig
{
    public const string ParserName = nameof(ArgumentParser);
    public const string WidthOptionLongName = "width";
    public const string HeightOptionLongName = "height";
    public const string CustomSubcommandName = "subcommand";

    [ValueOption<int>(WidthOptionLongName)]
    public int Width { get; set; }

    [ValueOption<int>(HeightOptionLongName)]
    public int Height { get; set; }

    [Subcommand(CustomSubcommandName, "")]
    public CustomSubcommand CustomSubcommand { get; } = new();
}

public class CustomSubcommand
{
    public const string FilesOptionLongName = "files";

    [MultipleValueOption<string>(FilesOptionLongName, contextCaptureType: ContextCaptureType.OneOrMore)]
    public List<string> Files { get; set; } = [];
}
#pragma warning restore
