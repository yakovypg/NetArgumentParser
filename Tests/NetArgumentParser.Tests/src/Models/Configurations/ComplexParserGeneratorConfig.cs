using System.Collections.Generic;
using System.IO;
using System.Numerics;
using NetArgumentParser.Attributes;
using NetArgumentParser.Options.Context;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal class ComplexParserGeneratorConfig
{
    public const string VerbosityLevelLongName = "verbosity";
    public const string VerbosityLevelShortName = "";
    public const string VerbosityLevelDescription = "verbosity description";
    public const bool VerbosityLevelIsRequired = false;
    public const bool VerbosityLevelIsHidden = false;
    public const bool VerbosityLevelIsFinal = false;

    public const string ModeLongName = "mode";
    public const string ModeShortName = "m";
    public const string ModeDescription = "mode description";
    public const string ModeMetaVariable = "M";
    public const bool ModeIsRequired = true;
    public const bool ModeIsHidden = false;
    public const bool ModeIsFinal = false;
    public const bool ModeUseDefaultChoices = true;

    public const string IgnoreCaseLongName = "ignore-case";
    public const string IgnoreCaseShortName = "i";
    public const string IgnoreCaseDescription = "ignore-case description";
    public const bool IgnoreCaseIsRequired = false;
    public const bool IgnoreCaseIsHidden = true;
    public const bool IgnoreCaseIsFinal = false;

    public const string InputFilesLongName = "files";
    public const string InputFilesShortName = "f";
    public const string InputFilesDescription = "files description";
    public const string InputFilesMetaVariable = "IF";
    public const bool InputFilesIsRequired = true;
    public const bool InputFilesIsHidden = false;
    public const bool InputFilesIsFinal = false;

    public const string MarginLongName = "margin";
    public const string MarginShortName = "M";
    public const string MarginDescription = "margin description";
    public const string MarginMetaVariable = "M";
    public const bool MarginIsRequired = false;
    public const bool MarginIsHidden = true;
    public const bool MarginIsFinal = false;

    public const string AngleLongName = "angle";
    public const string AngleShortName = "a";
    public const string AngleDescription = "angle description";
    public const string AngleMetaVariable = "A";
    public const bool AngleIsRequired = false;
    public const bool AngleIsHidden = false;
    public const bool AngleIsFinal = false;
    public const double AngleDefaultValue = 45;

    public const string SubcommandsOnlySubcommandName = "subcommands-only";
    public const string SubcommandsOnlySubcommandDescription = "subcommands-only description";

    public const string StatusSubcommandName = "status";
    public const string StatusSubcommandDescription = "status description";

    public const string FlagOptionsGroupId = "FlagOptionsId";
    public const string FlagOptionsGroupHeader = "FlagOptions";
    public const string FlagOptionsGroupDescription = "FlagOptions description";

    public const string ValueOptionsGroupId = "ValueOptionsId";
    public const string ValueOptionsGroupHeader = "ValueOptions";
    public const string ValueOptionsGroupDescription = "ValueOptions description";

    public const string MutuallyExclusiveOptionGroupId = "group1Id";
    public const string MutuallyExclusiveOptionGroupHeader = "group1";
    public const string MutuallyExclusiveOptionGroupDescription = "group1 d";

    public ComplexParserGeneratorConfig()
    {
        SubcommandsOnlySubcommand = new SubcommandsOnlyParserGeneratorConfig();
    }

    public static IReadOnlyList<string> VerbosityLevelAliases { get; } = [];
    public static IReadOnlyList<string> ModeAliases { get; } = [];
    public static IReadOnlyList<string> IgnoreCaseAliases { get; } = ["ig1"];
    public static IReadOnlyList<string> InputFilesAliases { get; } = ["i1", "i2", "i3"];
    public static IReadOnlyList<string> MarginAliases { get; } = [];
    public static IReadOnlyList<string> AngleAliases { get; } = [];

    public static IReadOnlyList<FileMode> ModeChoices { get; } = [FileMode.Create, FileMode.Open];
    public static IReadOnlyList<double> AngleChoices { get; } = [0, 45, 90, 135, 180];

    [FlagOption("ignored", "i")]
    [OptionGroup(
        FlagOptionsGroupId,
        FlagOptionsGroupHeader,
        FlagOptionsGroupDescription)]
    public bool IgnoredBySetAccessorOption { get; }

    [CounterOption(
        VerbosityLevelLongName,
        VerbosityLevelShortName,
        VerbosityLevelDescription,
        VerbosityLevelIsRequired,
        VerbosityLevelIsHidden,
        VerbosityLevelIsFinal,
        [])
    ]
    [MutuallyExclusiveOptionGroup(
        MutuallyExclusiveOptionGroupId,
        MutuallyExclusiveOptionGroupHeader,
        MutuallyExclusiveOptionGroupDescription)]
    public BigInteger? VerbosityLevel { get; set; }

    [EnumValueOption<FileMode>(
        ModeLongName,
        ModeShortName,
        ModeDescription,
        ModeMetaVariable,
        ModeIsRequired,
        ModeIsHidden,
        ModeIsFinal,
        ModeUseDefaultChoices,
        [],
        [FileMode.Create, FileMode.Open])
    ]
    [OptionGroup(
        ValueOptionsGroupId,
        ValueOptionsGroupHeader,
        ValueOptionsGroupDescription)]
    public FileMode? Mode { get; set; }

    [FlagOption(
        IgnoreCaseLongName,
        IgnoreCaseShortName,
        IgnoreCaseDescription,
        IgnoreCaseIsRequired,
        IgnoreCaseIsHidden,
        IgnoreCaseIsFinal,
        ["ig1"])]
    [OptionGroup(
        FlagOptionsGroupId,
        FlagOptionsGroupHeader,
        FlagOptionsGroupDescription)]
    [MutuallyExclusiveOptionGroup(
        MutuallyExclusiveOptionGroupId,
        MutuallyExclusiveOptionGroupHeader,
        MutuallyExclusiveOptionGroupDescription)]
    public bool? IgnoreCase { get; set; }

    [MultipleValueOption<string>(
        InputFilesLongName,
        InputFilesShortName,
        InputFilesDescription,
        InputFilesMetaVariable,
        InputFilesIsRequired,
        InputFilesIsHidden,
        InputFilesIsFinal,
        ["i1", "i2", "i3"],
        ContextCaptureType.OneOrMore)
    ]
    [OptionGroup(
        ValueOptionsGroupId,
        ValueOptionsGroupHeader,
        ValueOptionsGroupDescription)]
    public List<string>? InputFiles { get; set; }

    [ValueOption<Margin>(
        MarginLongName,
        MarginShortName,
        MarginDescription,
        MarginMetaVariable,
        MarginIsRequired,
        MarginIsHidden,
        MarginIsFinal,
        [])
    ]
    [OptionGroup(
        ValueOptionsGroupId,
        ValueOptionsGroupHeader,
        ValueOptionsGroupDescription)]
    public Margin? Margin { get; set; }

    [ValueOption<double>(
        AngleDefaultValue,
        AngleLongName,
        AngleShortName,
        AngleDescription,
        AngleMetaVariable,
        AngleIsRequired,
        AngleIsHidden,
        AngleIsFinal,
        [],
        [0, 45, 90, 135, 180])
    ]
    [OptionGroup(
        ValueOptionsGroupId,
        ValueOptionsGroupHeader,
        ValueOptionsGroupDescription)]
    public double Angle { get; set; }

    [Subcommand(SubcommandsOnlySubcommandName, SubcommandsOnlySubcommandDescription)]
    public SubcommandsOnlyParserGeneratorConfig SubcommandsOnlySubcommand { get; }

    [Subcommand(StatusSubcommandName, StatusSubcommandDescription)]
    public int Status { get; }

    [Subcommand("ignored", "i")]
    protected int IgnoredByProtectedModifierSubcommand { get; }
}
