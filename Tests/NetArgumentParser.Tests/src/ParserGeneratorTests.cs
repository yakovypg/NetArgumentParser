using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using NetArgumentParser.Converters;
using NetArgumentParser.Generators;
using NetArgumentParser.Informing;
using NetArgumentParser.Options;
using NetArgumentParser.Subcommands;
using NetArgumentParser.Tests.Models;
using NetArgumentParser.Tests.Models.Configurations;

namespace NetArgumentParser.Tests;

public class ParserGeneratorTests
{
    public static IReadOnlyCollection<object[]> NotValidOptionConfigs { get; } =
    [
        [new NotValidCounterOptionParserGeneratorConfig()],
        [new NotValidEnumValueOptionParserGeneratorConfig()],
        [new NotValidFlagOptionParserGeneratorConfig()],
        [new NotValidHelpOptionParserGeneratorConfig()],
        [new NotValidMultipleValueOptionParserGeneratorConfig()],
        [new NotValidValueOptionParserGeneratorConfig()],
        [new NotValidVersionOptionParserGeneratorConfig()]
    ];

    [Fact]
    public void ConfigureParser_FlagOptionsOnlyConfig_ConfiguredCorrectly()
    {
        var generator = new ArgumentParserGenerator();
        var argumentParser = new ArgumentParser();
        var config = new FlagOptionsOnlyParserGeneratorConfig();

        generator.ConfigureParser(argumentParser, config);

        Assert.Empty(argumentParser.MutuallyExclusiveOptionGroups);

        VerifyFlagOptionsOnlyParserGeneratorConfigQuantum(argumentParser);
    }

    [Fact]
    public void ConfigureParser_SubcommandsOnlyConfig_ConfiguredCorrectly()
    {
        var generator = new ArgumentParserGenerator();
        var argumentParser = new ArgumentParser();
        var config = new SubcommandsOnlyParserGeneratorConfig();

        generator.ConfigureParser(argumentParser, config);

        Assert.Empty(argumentParser.MutuallyExclusiveOptionGroups);

        VerifySubcommandsOnlyParserGeneratorConfigQuantum(argumentParser);
    }

    [Fact]
    public void ConfigureParser_ComplexConfig_ConfiguredCorrectly()
    {
        var generator = new ArgumentParserGenerator();
        var argumentParser = new ArgumentParser();
        var config = new ComplexParserGeneratorConfig();

        generator.ConfigureParser(argumentParser, config);

        Assert.Equal(1, argumentParser.MutuallyExclusiveOptionGroups.Count);

        var group = argumentParser.MutuallyExclusiveOptionGroups.FirstOrDefault(t =>
        {
            return t.Header == ComplexParserGeneratorConfig.MutuallyExclusiveOptionGroupHeader;
        });

        Assert.NotNull(group);
        Assert.Equal(2, group.Options.Count);

        Assert.Equal(
            ComplexParserGeneratorConfig.MutuallyExclusiveOptionGroupHeader,
            group.Header);

        Assert.Equal(
            ComplexParserGeneratorConfig.MutuallyExclusiveOptionGroupDescription,
            group.Description);

        ICommonOption? verbosityLevelOption = group.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.VerbosityLevelLongName;
        });

        Assert.NotNull(verbosityLevelOption);

        ICommonOption? ignoreCaseOption = group.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.IgnoreCaseLongName;
        });

        Assert.NotNull(ignoreCaseOption);

        VerifyComplexParserGeneratorConfigQuantum(argumentParser);
    }

    [Fact]
    public void ConfigureParser_ParseSpecificConfig_ConfiguredCorrectly()
    {
        var generator = new ArgumentParserGenerator();
        var argumentParser = new ArgumentParser();
        var config = new ParseSpecificParserGeneratorConfig();

        generator.ConfigureParser(argumentParser, config);

        VerifyParseSpecificParserGeneratorConfigQuantum(argumentParser);

        Assert.Equal(2, argumentParser.MutuallyExclusiveOptionGroups.Count);

        var finalGroup = argumentParser.MutuallyExclusiveOptionGroups.FirstOrDefault(t =>
        {
            return t.Header == ParseSpecificParserGeneratorConfig
                .MutuallyExclusiveFinalOptionGroupHeader;
        });

        Assert.NotNull(finalGroup);
        Assert.Equal(1, finalGroup.Options.Count);

        Assert.Equal(
            ParseSpecificParserGeneratorConfig.MutuallyExclusiveFinalOptionGroupHeader,
            finalGroup.Header);

        Assert.Equal(
            ParseSpecificParserGeneratorConfig.MutuallyExclusiveFinalOptionGroupDescription,
            finalGroup.Description);

        ICommonOption? showHelpOption = finalGroup.Options.FirstOrDefault(t =>
        {
            return t.LongName == ParseSpecificParserGeneratorConfig.ShowHelpLongName;
        });

        Assert.NotNull(showHelpOption);

        var complexGroup = argumentParser.MutuallyExclusiveOptionGroups.FirstOrDefault(t =>
        {
            return t.Header == ComplexParserGeneratorConfig
                .MutuallyExclusiveOptionGroupHeader;
        });

        Assert.NotNull(complexGroup);
        Assert.Equal(3, complexGroup.Options.Count);

        Assert.Equal(
            ComplexParserGeneratorConfig.MutuallyExclusiveOptionGroupHeader,
            complexGroup.Header);

        Assert.Equal(
            ComplexParserGeneratorConfig.MutuallyExclusiveOptionGroupDescription,
            complexGroup.Description);

        ICommonOption? showVersionOption = complexGroup.Options.FirstOrDefault(t =>
        {
            return t.LongName == ParseSpecificParserGeneratorConfig.ShowVersionLongName;
        });

        Assert.NotNull(showVersionOption);

        ICommonOption? verbosityLevelOption = complexGroup.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.VerbosityLevelLongName;
        });

        Assert.NotNull(verbosityLevelOption);

        ICommonOption? ignoreCaseOption = complexGroup.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.IgnoreCaseLongName;
        });

        Assert.NotNull(ignoreCaseOption);
    }

    [Fact]
    public void ConfigureParser_NotValidConfig_ThrowsExceptionIfSpecifiedDuplicateOptionName()
    {
        var generator = new ArgumentParserGenerator();
        var argumentParser = new ArgumentParser();
        var config = new DuplicateOptionNamesParserGeneratorConfig();

        Assert.Throws<OnlyUniqueOptionNameException>(() =>
        {
            generator.ConfigureParser(argumentParser, config);
        });
    }

    [Theory]
    [MemberData(nameof(NotValidOptionConfigs))]
    public void ConfigureParser_NotValidOptionConfig_ThrowsExceptionIfPropertyTypeNotValid(object config)
    {
        ExtendedArgumentNullException.ThrowIfNull(config, nameof(config));

        var generator = new ArgumentParserGenerator();
        var argumentParser = new ArgumentParser();

        Assert.Throws<CannotCreateOptionException>(() =>
        {
            generator.ConfigureParser(argumentParser, config);
        });
    }

    [Fact]
    public void ConfigureParser_HelpOption_OptionHandledActionsConfiguredCorrectly()
    {
        var generator = new ArgumentParserGenerator();
        var argumentParser = new ArgumentParser();
        var config = new ParseSpecificParserGeneratorConfig();

        generator.ConfigureParser(argumentParser, config);

        argumentParser.AddConverters(true, new ValueConverter<Margin>(t =>
        {
            int[] data = t.Split(',').Select(int.Parse).ToArray();
            return new Margin(data[0], data[1], data[2], data[3]);
        }));

        ParseArgumentsResult result = argumentParser.Parse(
        [
            $"--{ParseSpecificParserGeneratorConfig.ShowHelpLongName}",
            ParseSpecificParserGeneratorConfig.ComplexSubcommandName,
            $"--{ComplexParserGeneratorConfig.AngleLongName} 10",
            $"-{ComplexParserGeneratorConfig.MarginShortName} 5,10,3,4",
        ]);

        Assert.Empty(result.HandledSubcommands);
        Assert.Equal(1, result.HandledOptions.Count);

        Assert.True(config.ShowHelp);
        Assert.False(config.ShowVersion);

        Assert.Equal(default, config.ComplexSubcommand.Angle);
        Assert.Equal(default, config.ComplexSubcommand.Margin);
    }

    [Fact]
    public void ConfigureParser_VersionOption_OptionHandledActionsConfiguredCorrectly()
    {
        var generator = new ArgumentParserGenerator();
        var argumentParser = new ArgumentParser();
        var config = new ParseSpecificParserGeneratorConfig();

        generator.ConfigureParser(argumentParser, config);

        argumentParser.AddConverters(true, new ValueConverter<Margin>(t =>
        {
            int[] data = t.Split(',').Select(int.Parse).ToArray();
            return new Margin(data[0], data[1], data[2], data[3]);
        }));

        ParseArgumentsResult result = argumentParser.Parse(
        [
            $"--{ParseSpecificParserGeneratorConfig.ShowVersionLongName}",
            ParseSpecificParserGeneratorConfig.ComplexSubcommandName,
            $"--{ComplexParserGeneratorConfig.AngleLongName} 10",
            $"-{ComplexParserGeneratorConfig.MarginShortName} 5,10,3,4",
        ]);

        Assert.Empty(result.HandledSubcommands);
        Assert.Equal(1, result.HandledOptions.Count);

        Assert.False(config.ShowHelp);
        Assert.True(config.ShowVersion);

        Assert.Equal(default, config.ComplexSubcommand.Angle);
        Assert.Equal(default, config.ComplexSubcommand.Margin);
    }

    [Fact]
    public void ConfigureParser_AllOptionTypes_OptionHandledActionsConfiguredCorrectly()
    {
        var generator = new ArgumentParserGenerator();
        var argumentParser = new ArgumentParser();
        var config = new ParseSpecificParserGeneratorConfig();

        generator.ConfigureParser(argumentParser, config);

        argumentParser.AddConverters(true, new ValueConverter<Margin>(t =>
        {
            int[] data = t.Split(',').Select(int.Parse).ToArray();
            return new Margin(data[0], data[1], data[2], data[3]);
        }));

        const bool expectedShowHelp = false;
        const bool expectedShowVersion = false;
        var expectedVerbosityLevel = new BigInteger(3);
        const FileMode expectedMode = FileMode.Open;
        bool? expectedIgnoreCase = null;
        var expectedInputFiles = new List<string>() { "/file.txt", "relative.mkv" };
        var expectedMargin = new Margin(5, 10, 3, 4);
        const double expectedAngle = 45;
        const bool expectedShowGuide = true;
        const bool expectedOverwriteFile = true;
        const bool expectedOverwriteDirectory = false;

        ParseArgumentsResult result = argumentParser.Parse(
        [
            ParseSpecificParserGeneratorConfig.ComplexSubcommandName,
            $"--{ComplexParserGeneratorConfig.AngleLongName}",
            expectedAngle.ToString(CultureInfo.CurrentCulture),
            $"--{ComplexParserGeneratorConfig.VerbosityLevelLongName}",
            $"-{ComplexParserGeneratorConfig.MarginShortName}",
            expectedMargin.ToString(),
            $"-{ComplexParserGeneratorConfig.ModeShortName}",
            expectedMode.ToString(),
            $"--{ComplexParserGeneratorConfig.InputFilesLongName}",
            expectedInputFiles[0],
            expectedInputFiles[1],
            $"--{ComplexParserGeneratorConfig.VerbosityLevelLongName}",
            $"--{ComplexParserGeneratorConfig.VerbosityLevelLongName}",
            ComplexParserGeneratorConfig.SubcommandsOnlySubcommandName,
            SubcommandsOnlyParserGeneratorConfig.FlagsSubcommandName,
            $"--{FlagOptionsOnlyParserGeneratorConfig.OverwriteFileLongName}",
            $"-{FlagOptionsOnlyParserGeneratorConfig.ShowGuideShortName}"
        ]);

        Assert.Equal(3, result.HandledSubcommands.Count);
        Assert.Equal(9, result.HandledOptions.Count);

        Assert.Equal(expectedShowHelp, config.ShowHelp);
        Assert.Equal(expectedShowVersion, config.ShowVersion);

        Assert.Equal(expectedVerbosityLevel, config.ComplexSubcommand.VerbosityLevel);
        Assert.Equal(expectedMode, config.ComplexSubcommand.Mode);
        Assert.Equal(expectedIgnoreCase, config.ComplexSubcommand.IgnoreCase);
        Assert.Equal(expectedMargin, config.ComplexSubcommand.Margin);
        Assert.Equal(expectedAngle, config.ComplexSubcommand.Angle);

        Assert.NotNull(config.ComplexSubcommand.InputFiles);
        Assert.True(expectedInputFiles.SequenceEqual(config.ComplexSubcommand.InputFiles));

        Assert.Equal(
            expectedShowGuide,
            config.ComplexSubcommand.SubcommandsOnlySubcommand.Flags.ShowGuide);

        Assert.Equal(
            expectedOverwriteFile,
            config.ComplexSubcommand.SubcommandsOnlySubcommand.Flags.OverwriteFile);

        Assert.Equal(
            expectedOverwriteDirectory,
            config.ComplexSubcommand.SubcommandsOnlySubcommand.Flags.OverwriteDirectory);
    }

    private static void VerifyFlagOptionsOnlyParserGeneratorConfigQuantum(ParserQuantum quantum)
    {
        ExtendedArgumentNullException.ThrowIfNull(quantum, nameof(quantum));

        Assert.Empty(quantum.Subcommands);

        Assert.Equal(1, quantum.OptionGroups.Count);
        Assert.Equal(quantum.DefaultGroup, quantum.OptionGroups[0]);

        Assert.Equal(3, quantum.Options.Count);

        ICommonOption? showGuideOption = quantum.Options.FirstOrDefault(t =>
        {
            return t.LongName == FlagOptionsOnlyParserGeneratorConfig.ShowGuideLongName;
        });

        Assert.NotNull(showGuideOption);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.ShowGuideLongName,
            showGuideOption.LongName);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.ShowGuideShortName,
            showGuideOption.ShortName);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.ShowGuideDescription,
            showGuideOption.Description);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.ShowGuideIsRequired,
            showGuideOption.IsRequired);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.ShowGuideIsHidden,
            showGuideOption.IsHidden);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.ShowGuideIsFinal,
            showGuideOption.IsFinal);

        bool showGuideAliasesEqual = FlagOptionsOnlyParserGeneratorConfig
            .ShowGuideAliases
            .SequenceEqual(showGuideOption.Aliases);

        Assert.True(showGuideAliasesEqual);

        ICommonOption? overwriteFileOption = quantum.Options.FirstOrDefault(t =>
        {
            return t.LongName == FlagOptionsOnlyParserGeneratorConfig.OverwriteFileLongName;
        });

        Assert.NotNull(overwriteFileOption);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.OverwriteFileLongName,
            overwriteFileOption.LongName);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.OverwriteFileShortName,
            overwriteFileOption.ShortName);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.OverwriteFileDescription,
            overwriteFileOption.Description);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.OverwriteFileIsRequired,
            overwriteFileOption.IsRequired);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.OverwriteFileIsHidden,
            overwriteFileOption.IsHidden);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.OverwriteFileIsFinal,
            overwriteFileOption.IsFinal);

        bool overwriteFileAliasesEqual = FlagOptionsOnlyParserGeneratorConfig
            .OverwriteFileAliases
            .SequenceEqual(overwriteFileOption.Aliases);

        Assert.True(overwriteFileAliasesEqual);

        ICommonOption? overwriteDirectoryOption = quantum.Options.FirstOrDefault(t =>
        {
            return t.LongName == FlagOptionsOnlyParserGeneratorConfig.OverwriteDirectoryLongName;
        });

        Assert.NotNull(overwriteDirectoryOption);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.OverwriteDirectoryLongName,
            overwriteDirectoryOption.LongName);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.OverwriteDirectoryShortName,
            overwriteDirectoryOption.ShortName);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.OverwriteDirectoryDescription,
            overwriteDirectoryOption.Description);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.OverwriteDirectoryIsRequired,
            overwriteDirectoryOption.IsRequired);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.OverwriteDirectoryIsHidden,
            overwriteDirectoryOption.IsHidden);

        Assert.Equal(
            FlagOptionsOnlyParserGeneratorConfig.OverwriteDirectoryIsFinal,
            overwriteDirectoryOption.IsFinal);

        bool overwriteDirectoryAliasesEqual = FlagOptionsOnlyParserGeneratorConfig
            .OverwriteDirectoryAliases
            .SequenceEqual(overwriteDirectoryOption.Aliases);

        Assert.True(overwriteDirectoryAliasesEqual);
    }

    private static void VerifySubcommandsOnlyParserGeneratorConfigQuantum(ParserQuantum quantum)
    {
        ExtendedArgumentNullException.ThrowIfNull(quantum, nameof(quantum));

        Assert.Empty(quantum.Options);

        Assert.Equal(1, quantum.OptionGroups.Count);
        Assert.Equal(quantum.DefaultGroup, quantum.OptionGroups[0]);

        Assert.Equal(3, quantum.Subcommands.Count);

        Subcommand? brunchSubcommand = quantum.Subcommands.FirstOrDefault(t =>
        {
            return t.Name == SubcommandsOnlyParserGeneratorConfig.BrunchSubcommandName;
        });

        Assert.NotNull(brunchSubcommand);

        Assert.Equal(
            SubcommandsOnlyParserGeneratorConfig.BrunchSubcommandName,
            brunchSubcommand.Name);

        Assert.Equal(
            SubcommandsOnlyParserGeneratorConfig.BrunchSubcommandDescription,
            brunchSubcommand.Description);

        VerifyParserQuantumIsEmpty(brunchSubcommand);

        Subcommand? statusSubcommand = quantum.Subcommands.FirstOrDefault(t =>
        {
            return t.Name == SubcommandsOnlyParserGeneratorConfig.StatusSubcommandName;
        });

        Assert.NotNull(statusSubcommand);

        Assert.Equal(
            SubcommandsOnlyParserGeneratorConfig.StatusSubcommandName,
            statusSubcommand.Name);

        Assert.Equal(
            SubcommandsOnlyParserGeneratorConfig.StatusSubcommandDescription,
            statusSubcommand.Description);

        VerifyParserQuantumIsEmpty(statusSubcommand);

        Subcommand? flagsSubcommand = quantum.Subcommands.FirstOrDefault(t =>
        {
            return t.Name == SubcommandsOnlyParserGeneratorConfig.FlagsSubcommandName;
        });

        Assert.NotNull(flagsSubcommand);

        Assert.Equal(
            SubcommandsOnlyParserGeneratorConfig.FlagsSubcommandName,
            flagsSubcommand.Name);

        Assert.Equal(
            SubcommandsOnlyParserGeneratorConfig.FlagsSubcommandDescription,
            flagsSubcommand.Description);

        VerifyFlagOptionsOnlyParserGeneratorConfigQuantum(flagsSubcommand);
    }

    private static void VerifyComplexParserGeneratorConfigQuantum(ParserQuantum quantum)
    {
        ExtendedArgumentNullException.ThrowIfNull(quantum, nameof(quantum));

        Assert.Equal(3, quantum.OptionGroups.Count);

        OptionGroup<ICommonOption>? defaultGroup = quantum.OptionGroups.FirstOrDefault(t =>
        {
            return t.Header == quantum.DefaultGroup.Header;
        });

        Assert.NotNull(defaultGroup);
        Assert.Equal(1, defaultGroup.Options.Count);

        ICommonOption? verbosityLevelOptionInDefaultGroup = defaultGroup.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.VerbosityLevelLongName;
        });

        Assert.NotNull(verbosityLevelOptionInDefaultGroup);

        OptionGroup<ICommonOption>? flagOptions = quantum.OptionGroups.FirstOrDefault(t =>
        {
            return t.Header == ComplexParserGeneratorConfig.FlagOptionsGroupHeader;
        });

        Assert.NotNull(flagOptions);
        Assert.Equal(1, flagOptions.Options.Count);

        Assert.Equal(
            ComplexParserGeneratorConfig.FlagOptionsGroupHeader,
            flagOptions.Header);

        Assert.Equal(
            ComplexParserGeneratorConfig.FlagOptionsGroupDescription,
            flagOptions.Description);

        ICommonOption? ignoreCaseOptionInFlagOptions = flagOptions.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.IgnoreCaseLongName;
        });

        Assert.NotNull(ignoreCaseOptionInFlagOptions);

        OptionGroup<ICommonOption>? valueOptions = quantum.OptionGroups.FirstOrDefault(t =>
        {
            return t.Header == ComplexParserGeneratorConfig.ValueOptionsGroupHeader;
        });

        Assert.NotNull(valueOptions);
        Assert.Equal(4, valueOptions.Options.Count);

        Assert.Equal(
            ComplexParserGeneratorConfig.ValueOptionsGroupHeader,
            valueOptions.Header);

        Assert.Equal(
            ComplexParserGeneratorConfig.ValueOptionsGroupDescription,
            valueOptions.Description);

        ICommonOption? modeOptionInValueOptions = valueOptions.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.ModeLongName;
        });

        Assert.NotNull(modeOptionInValueOptions);

        ICommonOption? inputFilesOptionInValueOptions = valueOptions.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.InputFilesLongName;
        });

        Assert.NotNull(inputFilesOptionInValueOptions);

        ICommonOption? marginOptionInValueOptions = valueOptions.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.MarginLongName;
        });

        Assert.NotNull(marginOptionInValueOptions);

        ICommonOption? angleOptionInValueOptions = valueOptions.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.AngleLongName;
        });

        Assert.NotNull(angleOptionInValueOptions);

        Assert.Equal(2, quantum.Subcommands.Count);

        Subcommand? statusSubcommand = quantum.Subcommands.FirstOrDefault(t =>
        {
            return t.Name == ComplexParserGeneratorConfig.StatusSubcommandName;
        });

        Assert.NotNull(statusSubcommand);

        Assert.Equal(
            ComplexParserGeneratorConfig.StatusSubcommandName,
            statusSubcommand.Name);

        Assert.Equal(
            ComplexParserGeneratorConfig.StatusSubcommandDescription,
            statusSubcommand.Description);

        VerifyParserQuantumIsEmpty(statusSubcommand);

        Subcommand? subcommandsOnlySubcommand = quantum.Subcommands.FirstOrDefault(t =>
        {
            return t.Name == ComplexParserGeneratorConfig.SubcommandsOnlySubcommandName;
        });

        Assert.NotNull(subcommandsOnlySubcommand);

        Assert.Equal(
            ComplexParserGeneratorConfig.SubcommandsOnlySubcommandName,
            subcommandsOnlySubcommand.Name);

        Assert.Equal(
            ComplexParserGeneratorConfig.SubcommandsOnlySubcommandDescription,
            subcommandsOnlySubcommand.Description);

        VerifySubcommandsOnlyParserGeneratorConfigQuantum(subcommandsOnlySubcommand);

        Assert.Equal(6, quantum.Options.Count);

        ICommonOption? verbosityLevelOption = quantum.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.VerbosityLevelLongName;
        });

        Assert.NotNull(verbosityLevelOption);

        Assert.Equal(
            ComplexParserGeneratorConfig.VerbosityLevelLongName,
            verbosityLevelOption.LongName);

        Assert.Equal(
            ComplexParserGeneratorConfig.VerbosityLevelShortName,
            verbosityLevelOption.ShortName);

        Assert.Equal(
            ComplexParserGeneratorConfig.VerbosityLevelDescription,
            verbosityLevelOption.Description);

        Assert.Equal(
            ComplexParserGeneratorConfig.VerbosityLevelIsRequired,
            verbosityLevelOption.IsRequired);

        Assert.Equal(
            ComplexParserGeneratorConfig.VerbosityLevelIsHidden,
            verbosityLevelOption.IsHidden);

        Assert.Equal(
            ComplexParserGeneratorConfig.VerbosityLevelIsFinal,
            verbosityLevelOption.IsFinal);

        bool verbosityLevelAliasesEqual = ComplexParserGeneratorConfig
            .VerbosityLevelAliases
            .SequenceEqual(verbosityLevelOption.Aliases);

        Assert.True(verbosityLevelAliasesEqual);

        ICommonOption? modeOptionBase = quantum.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.ModeLongName;
        });

        var modeOption = modeOptionBase as EnumValueOption<FileMode>;

        Assert.NotNull(modeOption);

        Assert.Equal(
            ComplexParserGeneratorConfig.ModeLongName,
            modeOption.LongName);

        Assert.Equal(
            ComplexParserGeneratorConfig.ModeShortName,
            modeOption.ShortName);

        Assert.Equal(
            ComplexParserGeneratorConfig.ModeDescription,
            modeOption.Description);

        Assert.Equal(
            ComplexParserGeneratorConfig.ModeMetaVariable,
            modeOption.MetaVariable);

        Assert.Equal(
            ComplexParserGeneratorConfig.ModeIsRequired,
            modeOption.IsRequired);

        Assert.Equal(
            ComplexParserGeneratorConfig.ModeIsHidden,
            modeOption.IsHidden);

        Assert.Equal(
            ComplexParserGeneratorConfig.ModeIsFinal,
            modeOption.IsFinal);

        bool modeAliasesEqual = ComplexParserGeneratorConfig
            .ModeAliases
            .SequenceEqual(modeOption.Aliases);

        Assert.True(modeAliasesEqual);

        ICommonOption? ignoreCaseOption = quantum.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.IgnoreCaseLongName;
        });

        Assert.NotNull(ignoreCaseOption);

        Assert.Equal(
            ComplexParserGeneratorConfig.IgnoreCaseLongName,
            ignoreCaseOption.LongName);

        Assert.Equal(
            ComplexParserGeneratorConfig.IgnoreCaseShortName,
            ignoreCaseOption.ShortName);

        Assert.Equal(
            ComplexParserGeneratorConfig.IgnoreCaseDescription,
            ignoreCaseOption.Description);

        Assert.Equal(
            ComplexParserGeneratorConfig.IgnoreCaseIsRequired,
            ignoreCaseOption.IsRequired);

        Assert.Equal(
            ComplexParserGeneratorConfig.IgnoreCaseIsHidden,
            ignoreCaseOption.IsHidden);

        Assert.Equal(
            ComplexParserGeneratorConfig.IgnoreCaseIsFinal,
            ignoreCaseOption.IsFinal);

        bool ignoreCaseAliasesEqual = ComplexParserGeneratorConfig
            .IgnoreCaseAliases
            .SequenceEqual(ignoreCaseOption.Aliases);

        Assert.True(ignoreCaseAliasesEqual);

        ICommonOption? inputFilesOptionBase = quantum.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.InputFilesLongName;
        });

        var inputFilesOption = inputFilesOptionBase as MultipleValueOption<string>;

        Assert.NotNull(inputFilesOption);

        Assert.Equal(
            ComplexParserGeneratorConfig.InputFilesLongName,
            inputFilesOption.LongName);

        Assert.Equal(
            ComplexParserGeneratorConfig.InputFilesShortName,
            inputFilesOption.ShortName);

        Assert.Equal(
            ComplexParserGeneratorConfig.InputFilesDescription,
            inputFilesOption.Description);

        Assert.Equal(
            ComplexParserGeneratorConfig.InputFilesMetaVariable,
            inputFilesOption.MetaVariable);

        Assert.Equal(
            ComplexParserGeneratorConfig.InputFilesIsRequired,
            inputFilesOption.IsRequired);

        Assert.Equal(
            ComplexParserGeneratorConfig.InputFilesIsHidden,
            inputFilesOption.IsHidden);

        Assert.Equal(
            ComplexParserGeneratorConfig.InputFilesIsFinal,
            inputFilesOption.IsFinal);

        bool inputFilesAliasesEqual = ComplexParserGeneratorConfig
            .InputFilesAliases
            .SequenceEqual(inputFilesOption.Aliases);

        Assert.True(inputFilesAliasesEqual);

        ICommonOption? marginOptionBase = quantum.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.MarginLongName;
        });

        var marginOption = marginOptionBase as ValueOption<Margin>;

        Assert.NotNull(marginOption);

        Assert.Equal(
            ComplexParserGeneratorConfig.MarginLongName,
            marginOption.LongName);

        Assert.Equal(
            ComplexParserGeneratorConfig.MarginShortName,
            marginOption.ShortName);

        Assert.Equal(
            ComplexParserGeneratorConfig.MarginDescription,
            marginOption.Description);

        Assert.Equal(
            ComplexParserGeneratorConfig.MarginMetaVariable,
            marginOption.MetaVariable);

        Assert.Equal(
            ComplexParserGeneratorConfig.MarginIsRequired,
            marginOption.IsRequired);

        Assert.Equal(
            ComplexParserGeneratorConfig.MarginIsHidden,
            marginOption.IsHidden);

        Assert.Equal(
            ComplexParserGeneratorConfig.MarginIsFinal,
            marginOption.IsFinal);

        bool marginAliasesEqual = ComplexParserGeneratorConfig
            .MarginAliases
            .SequenceEqual(marginOption.Aliases);

        Assert.True(marginAliasesEqual);

        ICommonOption? angleOptionBase = quantum.Options.FirstOrDefault(t =>
        {
            return t.LongName == ComplexParserGeneratorConfig.AngleLongName;
        });

        var angleOption = angleOptionBase as ValueOption<double>;

        Assert.NotNull(angleOption);

        Assert.Equal(
            ComplexParserGeneratorConfig.AngleLongName,
            angleOption.LongName);

        Assert.Equal(
            ComplexParserGeneratorConfig.AngleShortName,
            angleOption.ShortName);

        Assert.Equal(
            ComplexParserGeneratorConfig.AngleDescription,
            angleOption.Description);

        Assert.Equal(
            ComplexParserGeneratorConfig.AngleMetaVariable,
            angleOption.MetaVariable);

        Assert.Equal(
            ComplexParserGeneratorConfig.AngleIsRequired,
            angleOption.IsRequired);

        Assert.Equal(
            ComplexParserGeneratorConfig.AngleIsHidden,
            angleOption.IsHidden);

        Assert.Equal(
            ComplexParserGeneratorConfig.AngleIsFinal,
            angleOption.IsFinal);

        Assert.Equal(
            ComplexParserGeneratorConfig.AngleDefaultValue,
            angleOption.DefaultValue?.Value);

        bool angleAliasesEqual = ComplexParserGeneratorConfig
            .AngleAliases
            .SequenceEqual(angleOption.Aliases);

        Assert.True(angleAliasesEqual);
    }

    private static void VerifyParseSpecificParserGeneratorConfigQuantum(ParserQuantum quantum)
    {
        ExtendedArgumentNullException.ThrowIfNull(quantum, nameof(quantum));

        Assert.Equal(1, quantum.OptionGroups.Count);
        Assert.Equal(quantum.DefaultGroup, quantum.OptionGroups[0]);

        Assert.Equal(1, quantum.Subcommands.Count);

        Subcommand? complexSubcommand = quantum.Subcommands.FirstOrDefault(t =>
        {
            return t.Name == ParseSpecificParserGeneratorConfig.ComplexSubcommandName;
        });

        Assert.NotNull(complexSubcommand);

        Assert.Equal(
            ParseSpecificParserGeneratorConfig.ComplexSubcommandName,
            complexSubcommand.Name);

        Assert.Equal(
            ParseSpecificParserGeneratorConfig.ComplexSubcommandDescription,
            complexSubcommand.Description);

        VerifyComplexParserGeneratorConfigQuantum(complexSubcommand);

        Assert.Equal(2, quantum.Options.Count);

        ICommonOption? showHelpOption = quantum.Options.FirstOrDefault(t =>
        {
            return t.LongName == ParseSpecificParserGeneratorConfig.ShowHelpLongName;
        });

        Assert.NotNull(showHelpOption);

        Assert.Equal(
            ParseSpecificParserGeneratorConfig.ShowHelpLongName,
            showHelpOption.LongName);

        Assert.Equal(
            ParseSpecificParserGeneratorConfig.ShowHelpShortName,
            showHelpOption.ShortName);

        Assert.Equal(
            ParseSpecificParserGeneratorConfig.ShowHelpDescription,
            showHelpOption.Description);

        Assert.Equal(
            ParseSpecificParserGeneratorConfig.ShowHelpIsHidden,
            showHelpOption.IsHidden);

        bool showHelpAliasesEqual = ParseSpecificParserGeneratorConfig
            .ShowHelpAliases
            .SequenceEqual(showHelpOption.Aliases);

        Assert.True(showHelpAliasesEqual);

        ICommonOption? showVersionOption = quantum.Options.FirstOrDefault(t =>
        {
            return t.LongName == ParseSpecificParserGeneratorConfig.ShowVersionLongName;
        });

        Assert.NotNull(showVersionOption);

        Assert.Equal(
            ParseSpecificParserGeneratorConfig.ShowVersionLongName,
            showVersionOption.LongName);

        Assert.Equal(
            ParseSpecificParserGeneratorConfig.ShowVersionShortName,
            showVersionOption.ShortName);

        Assert.Equal(
            ParseSpecificParserGeneratorConfig.ShowVersionDescription,
            showVersionOption.Description);

        Assert.Equal(
            ParseSpecificParserGeneratorConfig.ShowVersionIsHidden,
            showVersionOption.IsHidden);

        bool showVersionAliasesEqual = ParseSpecificParserGeneratorConfig
            .ShowVersionAliases
            .SequenceEqual(showVersionOption.Aliases);

        Assert.True(showVersionAliasesEqual);
    }

    private static void VerifyParserQuantumIsEmpty(ParserQuantum quantum)
    {
        ExtendedArgumentNullException.ThrowIfNull(quantum, nameof(quantum));

        Assert.Empty(quantum.Subcommands);
        Assert.Empty(quantum.Options);

        Assert.Equal(1, quantum.OptionGroups.Count);
        Assert.Equal(quantum.DefaultGroup, quantum.OptionGroups[0]);
    }
}
