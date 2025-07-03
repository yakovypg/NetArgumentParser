using System.Collections.Generic;
using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal class ParseSpecificParserGeneratorConfig
{
    public const string ShowHelpLongName = "help";
    public const string ShowHelpShortName = "h";
    public const string ShowHelpDescription = "help description";
    public const bool ShowHelpIsHidden = false;

    public const string ShowVersionLongName = "version";
    public const string ShowVersionShortName = "v";
    public const string ShowVersionDescription = "version description";
    public const bool ShowVersionIsHidden = true;

    public const string ComplexSubcommandName = "complex";
    public const string ComplexSubcommandDescription = "complex description";

    public const string MutuallyExclusiveFinalOptionGroupId = "finalId";
    public const string MutuallyExclusiveFinalOptionGroupHeader = "final";
    public const string MutuallyExclusiveFinalOptionGroupDescription = "final d";

    public ParseSpecificParserGeneratorConfig()
    {
        ComplexSubcommand = new ComplexParserGeneratorConfig();
    }

    public static IReadOnlyList<string> ShowHelpAliases { get; } = ["h1", "h2"];
    public static IReadOnlyList<string> ShowVersionAliases { get; } = ["v1"];

    [HelpOption(
        ShowHelpLongName,
        ShowHelpShortName,
        ShowHelpDescription,
        ShowHelpIsHidden,
        ["h1", "h2"])]
    [MutuallyExclusiveOptionGroup(
        MutuallyExclusiveFinalOptionGroupId,
        MutuallyExclusiveFinalOptionGroupHeader,
        MutuallyExclusiveFinalOptionGroupDescription)]
    public bool ShowHelp { get; set; }

    [VersionOption(
        ShowVersionLongName,
        ShowVersionShortName,
        ShowVersionDescription,
        ShowVersionIsHidden,
        ["v1"])]
    [MutuallyExclusiveOptionGroup(
        ComplexParserGeneratorConfig.MutuallyExclusiveOptionGroupId,
        ComplexParserGeneratorConfig.MutuallyExclusiveOptionGroupHeader,
        ComplexParserGeneratorConfig.MutuallyExclusiveOptionGroupDescription)]
    public bool ShowVersion { get; set; }

    [Subcommand(ComplexSubcommandName, ComplexSubcommandDescription)]
    public ComplexParserGeneratorConfig ComplexSubcommand { get; }
}
