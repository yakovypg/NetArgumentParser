using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NetArgumentParser.Informing;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Context;
using NetArgumentParser.Subcommands;
using NetArgumentParser.Tests.Extensions;
using NetArgumentParser.Tests.Models;

namespace NetArgumentParser.Tests;

public class ArgumentParserSubcommandTests
{
    [Fact]
    public void Parse_FirstDepthSubcommands_OptionsHandledCorrectly()
    {
        const string subcommand1Name = "subcommand1";
        const string subcommand2Name = "subcommand2";

        const string dateTimeValue = "1 January 2024";
        const StringSplitOptions expectedSplitOptions = StringSplitOptions.RemoveEmptyEntries;

        const int angleValue = 45;
        const int subcommand2AngleOffset = 90;
        const int expectedAngle = angleValue + subcommand2AngleOffset;

        const string leftMargin = "10";
        const string topMargin = "20";
        const string rightMargin = "30";
        const string bottomMargin = "40";

        var expectedExtraArguments = new string[]
        {
            "--height",
            "900",
            "24",
            "-x",
            "-125",
            "None"
        };

        var arguments = new string[]
        {
            expectedExtraArguments[0],
            expectedExtraArguments[1],
            "-l",
            expectedExtraArguments[2],
            "-s", expectedSplitOptions.ToString(),
            subcommand2Name,
            expectedExtraArguments[3],
            "--date", dateTimeValue,
            "-m", leftMargin, topMargin, rightMargin, bottomMargin,
            expectedExtraArguments[4],
            "-a", angleValue.ToString(CultureInfo.CurrentCulture),
            expectedExtraArguments[5]
        };

        bool savaLog = default;
        StringSplitOptions splitOption = default;
        DateTime recievedDateTime = default;
        Margin? margin = default;
        int angle = default;

        var options = new ICommonOption[]
        {
            new FlagOption(
                "save-log",
                "l",
                afterHandlingAction: () => savaLog = true),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t)
        };

        var subcommand1Options = new ICommonOption[]
        {
            new ValueOption<DateTime>(
                "date",
                string.Empty,
                afterValueParsingAction: t => recievedDateTime = t),

            new MultipleValueOption<int>(
                string.Empty,
                "m",
                contextCapture: new FixedContextCapture(4),
                afterValueParsingAction: t => margin = new Margin(t[0], t[1], t[2], t[3])),

            new ValueOption<int>(
                "angle",
                "a",
                afterValueParsingAction: t => angle = t)
        };

        var subcommand2Options = new ICommonOption[]
        {
            new ValueOption<DateTime>(
                "date",
                string.Empty,
                afterValueParsingAction: t => recievedDateTime = t),

            new MultipleValueOption<int>(
                string.Empty,
                "m",
                contextCapture: new FixedContextCapture(4),
                afterValueParsingAction: t => margin = new Margin(t[0], t[1], t[2], t[3])),

            new ValueOption<int>(
                "angle",
                "a",
                afterValueParsingAction: t => angle = t + subcommand2AngleOffset)
        };

        var parser = new ArgumentParser();
        parser.AddOptions(options);

        Subcommand subcommand1 = parser.AddSubcommand(subcommand1Name, string.Empty);
        subcommand1.AddOptions(subcommand1Options);

        Subcommand subcommand2 = parser.AddSubcommand(subcommand2Name, string.Empty);
        subcommand2.AddOptions(subcommand2Options);

        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        var expectedMargin = new Margin(
            int.Parse(leftMargin, CultureInfo.CurrentCulture),
            int.Parse(topMargin, CultureInfo.CurrentCulture),
            int.Parse(rightMargin, CultureInfo.CurrentCulture),
            int.Parse(bottomMargin, CultureInfo.CurrentCulture));

        Assert.True(savaLog);
        Assert.Equal(expectedSplitOptions, splitOption);
        Assert.Equal(DateTime.Parse(dateTimeValue, CultureInfo.CurrentCulture), recievedDateTime);
        Assert.Equal(expectedMargin, margin);
        Assert.Equal(expectedAngle, angle);

        VerifyExtraArguments(expectedExtraArguments, extraArguments);
    }

    [Fact]
    public void Parse_SecondDepthSubcommands_OptionsHandledCorrectly()
    {
        const string subcommand1Name = "foo";
        const string subcommand2Name = "bar";
        const string subsubcommand1Name = "bar";
        const string subsubcommand2Name = "foo";

        const int angleValue = 45;
        const int widthValue = 1920;
        const int heightValue = 1080;

        const int subcommand1Offset = 10;
        const int subcommand2Offset = 20;
        const int subsubcommand1Offset = 30;
        const int subsubcommand2Offset = 40;

        const int expectedAngle = angleValue;
        const int expectedWidth = widthValue + subcommand2Offset;
        const int expectedHeight = heightValue + subsubcommand2Offset;

        var expectedExtraArguments = new string[]
        {
            "--height",
            "900",
            "24",
            "-x",
            "-125",
            "None"
        };

        var arguments = new string[]
        {
            expectedExtraArguments[0],
            expectedExtraArguments[1],
            "-A", angleValue.ToString(CultureInfo.CurrentCulture),
            expectedExtraArguments[2],
            subcommand2Name,
            expectedExtraArguments[3],
            "-W", widthValue.ToString(CultureInfo.CurrentCulture),
            subsubcommand2Name,
            expectedExtraArguments[4],
            "-H", heightValue.ToString(CultureInfo.CurrentCulture),
            expectedExtraArguments[5]
        };

        int angle = default;
        int width = default;
        int height = default;

        var options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t)
        };

        var subcommand1Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subcommand1Offset)
        };

        var subcommand2Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subcommand2Offset)
        };

        var subsubcommand1Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subsubcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subsubcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subsubcommand1Offset)
        };

        var subsubcommand2Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subsubcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subsubcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subsubcommand2Offset)
        };

        var parser = new ArgumentParser();
        parser.AddOptions(options);

        Subcommand subcommand1 = parser.AddSubcommand(subcommand1Name, string.Empty);
        subcommand1.AddOptions(subcommand1Options);

        Subcommand subcommand2 = parser.AddSubcommand(subcommand2Name, string.Empty);
        subcommand2.AddOptions(subcommand2Options);

        Subcommand subsubcommand1 = subcommand2.AddSubcommand(subsubcommand1Name, string.Empty);
        subsubcommand1.AddOptions(subsubcommand1Options);

        Subcommand subsubcommand2 = subcommand2.AddSubcommand(subsubcommand2Name, string.Empty);
        subsubcommand2.AddOptions(subsubcommand2Options);

        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.Equal(expectedAngle, angle);
        Assert.Equal(expectedWidth, width);
        Assert.Equal(expectedHeight, height);

        VerifyExtraArguments(expectedExtraArguments, extraArguments);
    }

    [Fact]
    public void Parse_ThirdDepthSubcommands_OptionsHandledCorrectly()
    {
        const string subcommand0Name = "s0";
        const string subcommand1Name = "s1";
        const string subcommand2Name = "s2";
        const string subsubcommand1Name = "ss1";
        const string subsubcommand2Name = "ss2";
        const string subsubcommand3Name = "ss1";
        const string subsubcommand4Name = "ss2";
        const string subsubsubcommand1Name = "s1";
        const string subsubsubcommand2Name = "s2";
        const string subsubsubcommand3Name = "s1";
        const string subsubsubcommand4Name = "s2";

        const int angleValue = 45;
        const int widthValue = 1920;
        const int heightValue = 1080;
        const int opacityValue = 1;

        const int subcommand1Offset = 10;
        const int subcommand2Offset = 20;
        const int subsubcommand1Offset = 30;
        const int subsubcommand2Offset = 40;
        const int subsubcommand3Offset = 50;
        const int subsubcommand4Offset = 60;
        const int subsubsubcommand1Offset = 70;
        const int subsubsubcommand2Offset = 80;
        const int subsubsubcommand3Offset = 90;
        const int subsubsubcommand4Offset = 100;

        const int expectedAngle = angleValue;
        const int expectedWidth = widthValue + subcommand2Offset;
        const int expectedHeight = heightValue + subsubcommand3Offset;
        const int expectedOpacity = opacityValue + subsubsubcommand1Offset;

        var expectedExtraArguments = new string[]
        {
            "--height",
            "900",
            "24",
            "-x",
            subcommand0Name,
            "-125",
            subcommand0Name
        };

        var arguments = new string[]
        {
            expectedExtraArguments[0],
            expectedExtraArguments[1],
            "-A", angleValue.ToString(CultureInfo.CurrentCulture),
            expectedExtraArguments[2],
            subcommand2Name,
            expectedExtraArguments[3],
            "-W", widthValue.ToString(CultureInfo.CurrentCulture),
            subsubcommand3Name,
            expectedExtraArguments[4],
            "-H", heightValue.ToString(CultureInfo.CurrentCulture),
            expectedExtraArguments[5],
            subsubsubcommand1Name,
            "-O", opacityValue.ToString(CultureInfo.CurrentCulture),
            expectedExtraArguments[6]
        };

        int angle = default;
        int width = default;
        int height = default;
        int opacity = default;

        var options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t),

            new ValueOption<int>(
                string.Empty,
                "O",
                afterValueParsingAction: t => opacity = t)
        };

        var subcommand0Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t),

            new ValueOption<int>(
                string.Empty,
                "O",
                afterValueParsingAction: t => opacity = t)
        };

        var subcommand1Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "O",
                afterValueParsingAction: t => opacity = t + subcommand1Offset)
        };

        var subcommand2Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "O",
                afterValueParsingAction: t => opacity = t + subcommand2Offset)
        };

        var subsubcommand1Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subsubcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subsubcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subsubcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "O",
                afterValueParsingAction: t => opacity = t + subsubcommand1Offset)
        };

        var subsubcommand2Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subsubcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subsubcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subsubcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "O",
                afterValueParsingAction: t => opacity = t + subsubcommand2Offset)
        };

        var subsubcommand3Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subsubcommand3Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subsubcommand3Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subsubcommand3Offset),

            new ValueOption<int>(
                string.Empty,
                "O",
                afterValueParsingAction: t => opacity = t + subsubcommand3Offset)
        };

        var subsubcommand4Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subsubcommand4Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subsubcommand4Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subsubcommand4Offset),

            new ValueOption<int>(
                string.Empty,
                "O",
                afterValueParsingAction: t => opacity = t + subsubcommand4Offset)
        };

        var subsubsubcommand1Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subsubsubcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subsubsubcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subsubsubcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "O",
                afterValueParsingAction: t => opacity = t + subsubsubcommand1Offset)
        };

        var subsubsubcommand2Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subsubsubcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subsubsubcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subsubsubcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "O",
                afterValueParsingAction: t => opacity = t + subsubsubcommand2Offset)
        };

        var subsubsubcommand3Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subsubsubcommand3Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subsubsubcommand3Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subsubsubcommand3Offset),

            new ValueOption<int>(
                string.Empty,
                "O",
                afterValueParsingAction: t => opacity = t + subsubsubcommand3Offset)
        };

        var subsubsubcommand4Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subsubsubcommand4Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subsubsubcommand4Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subsubsubcommand4Offset),

            new ValueOption<int>(
                string.Empty,
                "O",
                afterValueParsingAction: t => opacity = t + subsubsubcommand4Offset)
        };

        var parser = new ArgumentParser();
        parser.AddOptions(options);

        Subcommand subcommand0 = parser.AddSubcommand(subcommand0Name, string.Empty);
        subcommand0.AddOptions(subcommand0Options);

        Subcommand subcommand1 = parser.AddSubcommand(subcommand1Name, string.Empty);
        subcommand1.AddOptions(subcommand1Options);

        Subcommand subcommand2 = parser.AddSubcommand(subcommand2Name, string.Empty);
        subcommand2.AddOptions(subcommand2Options);

        Subcommand subsubcommand1 = subcommand1.AddSubcommand(subsubcommand1Name, string.Empty);
        subsubcommand1.AddOptions(subsubcommand1Options);

        Subcommand subsubcommand2 = subcommand1.AddSubcommand(subsubcommand2Name, string.Empty);
        subsubcommand2.AddOptions(subsubcommand2Options);

        Subcommand subsubcommand3 = subcommand2.AddSubcommand(subsubcommand3Name, string.Empty);
        subsubcommand3.AddOptions(subsubcommand3Options);

        Subcommand subsubcommand4 = subcommand2.AddSubcommand(subsubcommand4Name, string.Empty);
        subsubcommand4.AddOptions(subsubcommand4Options);

        Subcommand subsubsubcommand1 = subsubcommand3.AddSubcommand(subsubsubcommand1Name, string.Empty);
        subsubsubcommand1.AddOptions(subsubsubcommand1Options);

        Subcommand subsubsubcommand2 = subsubcommand3.AddSubcommand(subsubsubcommand2Name, string.Empty);
        subsubsubcommand2.AddOptions(subsubsubcommand2Options);

        Subcommand subsubsubcommand3 = subsubcommand4.AddSubcommand(subsubsubcommand3Name, string.Empty);
        subsubsubcommand3.AddOptions(subsubsubcommand3Options);

        Subcommand subsubsubcommand4 = subsubcommand4.AddSubcommand(subsubsubcommand4Name, string.Empty);
        subsubsubcommand4.AddOptions(subsubsubcommand4Options);

        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.Equal(expectedAngle, angle);
        Assert.Equal(expectedWidth, width);
        Assert.Equal(expectedHeight, height);
        Assert.Equal(expectedOpacity, opacity);

        VerifyExtraArguments(expectedExtraArguments, extraArguments);
    }

    [Fact]
    public void Parse_HelpOption_OtherArgumentsSkipped()
    {
        const string subcommand1Name = "subcommand1";
        const string subcommand2Name = "subcommand2";

        bool help = default;
        bool verbose = default;
        double angle = default;
        StringSplitOptions splitOption = default;
        List<string>? files = default;

        var arguments = new string[]
        {
            "-v",
            subcommand1Name,
            "--angle", "100.5",
            subcommand2Name,
            "--help",
            "-s", StringSplitOptions.RemoveEmptyEntries.ToString(),
            "-f", "file1", "file2", "file3"
        };

        var options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true,
                afterHandlingAction: () => verbose = true),

            new HelpOption("help", afterHandlingAction: () => help = true),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand1Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true,
                afterHandlingAction: () => verbose = true),

            new HelpOption("help", afterHandlingAction: () => help = true),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand2Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true,
                afterHandlingAction: () => verbose = true),

            new HelpOption("help", afterHandlingAction: () => help = true),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeSlashOptions = true
        };

        Subcommand subcommand1 = parser.AddSubcommand(subcommand1Name, string.Empty);
        subcommand1.AddOptions(subcommand1Options);

        Subcommand subcommand2 = parser.AddSubcommand(subcommand2Name, string.Empty);
        subcommand2.AddOptions(subcommand2Options);

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.True(help);

        Assert.Equal(default, verbose);
        Assert.Equal(default, angle);
        Assert.Equal(default, splitOption);
        Assert.Equal(default, files);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void Parse_VersionOption_OtherArgumentsSkipped()
    {
        const string subcommand1Name = "subcommand1";
        const string subcommand2Name = "subcommand2";

        bool version = default;
        bool verbose = default;
        double angle = default;
        StringSplitOptions splitOption = default;
        List<string>? files = default;

        var arguments = new string[]
        {
            "-v",
            subcommand1Name,
            "--angle", "100.5",
            subcommand2Name,
            "--version",
            "-s", StringSplitOptions.RemoveEmptyEntries.ToString(),
            "-f", "file1", "file2", "file3"
        };

        var options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true,
                afterHandlingAction: () => verbose = true),

            new VersionOption("version", afterHandlingAction: () => version = true),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand1Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true,
                afterHandlingAction: () => verbose = true),

            new VersionOption("version", afterHandlingAction: () => version = true),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand2Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true,
                afterHandlingAction: () => verbose = true),

            new VersionOption("version", afterHandlingAction: () => version = true),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeSlashOptions = true
        };

        Subcommand subcommand1 = parser.AddSubcommand(subcommand1Name, string.Empty);
        subcommand1.AddOptions(subcommand1Options);

        Subcommand subcommand2 = parser.AddSubcommand(subcommand2Name, string.Empty);
        subcommand2.AddOptions(subcommand2Options);

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.True(version);

        Assert.Equal(default, verbose);
        Assert.Equal(default, angle);
        Assert.Equal(default, splitOption);
        Assert.Equal(default, files);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void Parse_FinalOption_OtherArgumentsSkipped()
    {
        const string subcommand1Name = "subcommand1";
        const string subcommand2Name = "subcommand2";

        int finalOptionValue = default;
        bool verbose = default;
        double angle = default;
        StringSplitOptions splitOption = default;
        List<string>? files = default;

        const int expectedfinalOptionValue = -900;

        var arguments = new string[]
        {
            "-v",
            subcommand1Name,
            "--angle", "100.5",
            subcommand2Name,
            "--final", expectedfinalOptionValue.ToString(CultureInfo.CurrentCulture),
            "-s", StringSplitOptions.RemoveEmptyEntries.ToString(),
            "-f", "file1", "file2", "file3"
        };

        var options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true,
                afterHandlingAction: () => verbose = true),

            new ValueOption<int>(
                "final",
                isFinal: true,
                afterValueParsingAction: t => finalOptionValue = t),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand1Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true,
                afterHandlingAction: () => verbose = true),

            new ValueOption<int>(
                "final",
                isFinal: true,
                afterValueParsingAction: t => finalOptionValue = t),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand2Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true,
                afterHandlingAction: () => verbose = true),

            new ValueOption<int>(
                "final",
                isFinal: true,
                afterValueParsingAction: t => finalOptionValue = t),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeSlashOptions = true
        };

        Subcommand subcommand1 = parser.AddSubcommand(subcommand1Name, string.Empty);
        subcommand1.AddOptions(subcommand1Options);

        Subcommand subcommand2 = parser.AddSubcommand(subcommand2Name, string.Empty);
        subcommand2.AddOptions(subcommand2Options);

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.Equal(expectedfinalOptionValue, finalOptionValue);

        Assert.Equal(default, verbose);
        Assert.Equal(default, angle);
        Assert.Equal(default, splitOption);
        Assert.Equal(default, files);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void AddSubcommand_Subcommands_ThrowsExceptionIfSubcommandsHasSameName()
    {
        const string name1 = "s1";
        const string name2 = "s2";

        var parser = new ArgumentParser();

        Subcommand? s1 = null;
        Subcommand? s2 = null;
        Subcommand? ss1 = null;
        Subcommand? ss2 = null;

        Exception? ex1 = Record.Exception(
            () => s1 = parser.AddSubcommand(name1, string.Empty));

        Assert.Null(ex1);
        Assert.NotNull(s1);

        Exception? ex2 = Record.Exception(
            () => parser.AddSubcommand(name1, string.Empty));

        Assert.NotNull(ex2);

        Exception? ex3 = Record.Exception(
            () => s2 = parser.AddSubcommand(name2, string.Empty));

        Assert.Null(ex3);
        Assert.NotNull(s2);

        Exception? ex4 = Record.Exception(
            () => ss1 = s1?.AddSubcommand(name1, string.Empty));

        Assert.Null(ex4);
        Assert.NotNull(ss1);

        Exception? ex5 = Record.Exception(
            () => s1?.AddSubcommand(name1, string.Empty));

        Assert.NotNull(ex5);

        Exception? ex6 = Record.Exception(
            () => ss2 = s1?.AddSubcommand(name2, string.Empty));

        Assert.Null(ex6);
        Assert.NotNull(ss2);
    }

    [Fact]
    public void Parse_MutuallyExclusiveOptionGroupWithOneOption_NotThrowsException()
    {
        const string subcommandName = "subcommand1";

        var arguments = new string[]
        {
            "-a", "5",
            "-b", "1920",
            subcommandName,
            "-c", "10",
            "-d", "15"
        };

        var options = new ICommonOption[]
        {
            new ValueOption<int>(string.Empty, "a"),
            new ValueOption<int>(string.Empty, "b")
        };

        var subcommandOptions = new ICommonOption[]
        {
            new ValueOption<int>(string.Empty, "c"),
            new ValueOption<int>(string.Empty, "d")
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false
        };

        parser.AddOptions(options);

        Subcommand subcommand = parser.AddSubcommand(subcommandName, string.Empty);
        subcommand.AddOptions(subcommandOptions);

        MutuallyExclusiveOptionGroup<ICommonOption> group =
            parser.AddMutuallyExclusiveOptionGroup("group", null, [options[0]]);

        MutuallyExclusiveOptionGroup<ICommonOption> subcommandGroup =
            parser.AddMutuallyExclusiveOptionGroup("subcommandGroup", null, [subcommandOptions[1]]);

        Exception? ex = Record.Exception(() => parser.Parse(arguments));
        Assert.Null(ex);
    }

    [Fact]
    public void Parse_MutuallyExclusiveOptions_ThrowsException()
    {
        const string subcommandName = "subcommand1";

        var arguments = new string[]
        {
            "--c0", "5",
            "-b", "1920",
            "--c2", "1080",
            subcommandName,
            "-d", "10",
            "--c1", "15"
        };

        var conflictingOptions = new List<ICommonOption>()
        {
            new ValueOption<int>("c0", string.Empty),
            new ValueOption<double>("c1", string.Empty),
            new ValueOption<int>("c2", string.Empty)
        };

        var options = new ICommonOption[]
        {
            conflictingOptions[0],
            new ValueOption<double>(string.Empty, "b"),
            new ValueOption<double>("c1", string.Empty),
            new ValueOption<int>(string.Empty, "d"),
            conflictingOptions[2]
        };

        var subcommandOptions = new ICommonOption[]
        {
            new ValueOption<double>("c0", string.Empty),
            new ValueOption<double>(string.Empty, "b"),
            conflictingOptions[1],
            new ValueOption<int>(string.Empty, "d"),
            new ValueOption<double>("c2", string.Empty),
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false
        };

        parser.AddOptions(options);

        Subcommand subcommand = parser.AddSubcommand(subcommandName, string.Empty);
        subcommand.AddOptions(subcommandOptions);

        MutuallyExclusiveOptionGroup<ICommonOption> group =
            parser.AddMutuallyExclusiveOptionGroup("group", null, conflictingOptions);

        do
        {
            var exception = Assert.Throws<MutuallyExclusiveOptionsFoundException>(() =>
            {
                _ = parser.Parse(arguments);
            });

            ICommonOption existingOption = conflictingOptions[0];
            ICommonOption newOption = conflictingOptions[^1];

            Assert.Equal(existingOption, exception.ExistingOption);
            Assert.Equal(newOption, exception.NewOption);

            _ = group.RemoveOption(newOption);
            _ = conflictingOptions.Remove(newOption);

            parser.ResetOptionsHandledState();
        }
        while (conflictingOptions.Count > 1);

        Exception? ex = Record.Exception(() => parser.Parse(arguments));
        Assert.Null(ex);
    }

    [Fact]
    public void Parse_HelpnOption_ArgumentsParseResultIsCorrect()
    {
        const string subcommand1Name = "subcommand1";
        const string subcommand2Name = "subcommand2";
        const string subcommand3Name = "subcommand3";

        bool help = default;
        double angle = default;
        StringSplitOptions splitOption = default;
        List<string>? files = default;

        var arguments = new string[]
        {
            "-v",
            subcommand1Name,
            "--angle", "100.5",
            subcommand2Name,
            "--help",
            "-s", StringSplitOptions.RemoveEmptyEntries.ToString(),
            subcommand3Name,
            "--help",
            "-f", "file1", "file2", "file3"
        };

        var expectedHelpOption = new HelpOption(
            "help", afterHandlingAction: () => help = true);

        var options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true),

            new HelpOption("help", afterHandlingAction: () => help = true),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand1Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true),

            expectedHelpOption,

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand2Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true),

            new HelpOption("help", afterHandlingAction: () => help = true),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand3Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true),

            new HelpOption("help", afterHandlingAction: () => help = true),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeSlashOptions = true
        };

        Subcommand subcommand1 = parser.AddSubcommand(subcommand1Name, string.Empty);
        subcommand1.AddOptions(subcommand1Options);

        Subcommand subcommand2 = parser.AddSubcommand(subcommand2Name, string.Empty);
        subcommand2.AddOptions(subcommand2Options);

        Subcommand subcommand3 = parser.AddSubcommand(subcommand3Name, string.Empty);
        subcommand3.AddOptions(subcommand3Options);

        parser.AddOptions(options);
        ParseArgumentsResult result = parser.ParseKnownArguments(arguments, out _);

        Assert.True(help);

        Assert.Equal(1, result.HandledOptions.Count);
        Assert.Equal(expectedHelpOption, result.HandledOptions.First());

        Assert.Equal(1, result.HandledSubcommands.Count);
        Assert.Equal(subcommand1, result.HandledSubcommands.First());
    }

    [Fact]
    public void Parse_VersionOption_ArgumentsParseResultIsCorrect()
    {
        const string subcommand1Name = "subcommand1";
        const string subcommand2Name = "subcommand2";
        const string subcommand3Name = "subcommand3";

        bool version = default;
        double angle = default;
        StringSplitOptions splitOption = default;
        List<string>? files = default;

        var arguments = new string[]
        {
            "-v",
            subcommand1Name,
            "--angle", "100.5",
            subcommand2Name,
            "--version",
            "-s", StringSplitOptions.RemoveEmptyEntries.ToString(),
            subcommand3Name,
            "--version",
            "-f", "file1", "file2", "file3"
        };

        var expectedVersionOption = new VersionOption(
            "version", afterHandlingAction: () => version = true);

        var options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true),

            new VersionOption("version", afterHandlingAction: () => version = true),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand1Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true),

            expectedVersionOption,

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand2Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true),

            new VersionOption("version", afterHandlingAction: () => version = true),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand3Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true),

            new VersionOption("version", afterHandlingAction: () => version = true),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeSlashOptions = true
        };

        Subcommand subcommand1 = parser.AddSubcommand(subcommand1Name, string.Empty);
        subcommand1.AddOptions(subcommand1Options);

        Subcommand subcommand2 = parser.AddSubcommand(subcommand2Name, string.Empty);
        subcommand2.AddOptions(subcommand2Options);

        Subcommand subcommand3 = parser.AddSubcommand(subcommand3Name, string.Empty);
        subcommand3.AddOptions(subcommand3Options);

        parser.AddOptions(options);
        ParseArgumentsResult result = parser.ParseKnownArguments(arguments, out _);

        Assert.True(version);

        Assert.Equal(1, result.HandledOptions.Count);
        Assert.Equal(expectedVersionOption, result.HandledOptions.First());

        Assert.Equal(1, result.HandledSubcommands.Count);
        Assert.Equal(subcommand1, result.HandledSubcommands.First());
    }

    [Fact]
    public void Parse_FinalOption_ArgumentsParseResultIsCorrect()
    {
        const string subcommand1Name = "subcommand1";
        const string subcommand2Name = "subcommand2";
        const string subcommand3Name = "subcommand3";

        int finalOptionValue = default;
        double angle = default;
        StringSplitOptions splitOption = default;
        List<string>? files = default;

        const int expectedFinalOptionValue = -900;

        var arguments = new string[]
        {
            "-v",
            subcommand1Name,
            "--angle", "100.5",
            subcommand2Name,
            "--final", expectedFinalOptionValue.ToString(CultureInfo.CurrentCulture),
            "-s", StringSplitOptions.RemoveEmptyEntries.ToString(),
            subcommand3Name,
            "--final", "123",
            "-f", "file1", "file2", "file3"
        };

        var expectedFinalOption = new ValueOption<int>(
            "final",
            isFinal: true,
            afterValueParsingAction: t => finalOptionValue = t);

        var options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true),

            new ValueOption<int>(
                "final",
                isFinal: true,
                afterValueParsingAction: t => finalOptionValue = t),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand1Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true),

            expectedFinalOption,

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand2Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true),

            new ValueOption<int>(
                "final",
                isFinal: true,
                afterValueParsingAction: t => finalOptionValue = t),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var subcommand3Options = new ICommonOption[]
        {
            new FlagOption(
                "verbose",
                "v",
                isRequired: true),

            new ValueOption<int>(
                "final",
                isFinal: true,
                afterValueParsingAction: t => finalOptionValue = t),

            new ValueOption<double>(
                "angle",
                "a",
                isRequired: true,
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                "f",
                isRequired: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeSlashOptions = true
        };

        Subcommand subcommand1 = parser.AddSubcommand(subcommand1Name, string.Empty);
        subcommand1.AddOptions(subcommand1Options);

        Subcommand subcommand2 = parser.AddSubcommand(subcommand2Name, string.Empty);
        subcommand2.AddOptions(subcommand2Options);

        Subcommand subcommand3 = parser.AddSubcommand(subcommand3Name, string.Empty);
        subcommand3.AddOptions(subcommand3Options);

        parser.AddOptions(options);
        ParseArgumentsResult result = parser.ParseKnownArguments(arguments, out _);

        Assert.Equal(expectedFinalOptionValue, finalOptionValue);

        Assert.Equal(1, result.HandledOptions.Count);
        Assert.Equal(expectedFinalOption, result.HandledOptions.First());

        Assert.Equal(1, result.HandledSubcommands.Count);
        Assert.Equal(subcommand1, result.HandledSubcommands.First());
    }

    [Fact]
    public void Parse_SeveralArguments_ArgumentsParseResultIsCorrect()
    {
        const string subcommand1Name = "foo";
        const string subcommand2Name = "bar";
        const string subsubcommand1Name = "bar";
        const string subsubcommand2Name = "foo";

        const int angleValue = 45;
        const int widthValue = 1920;
        const int heightValue = 1080;

        const int subcommand1Offset = 10;
        const int subcommand2Offset = 20;
        const int subsubcommand1Offset = 30;
        const int subsubcommand2Offset = 40;

        var extraArguments = new string[]
        {
            "--height",
            "900",
            "24",
            "-x",
            "-125",
            "None"
        };

        var arguments = new string[]
        {
            extraArguments[0],
            extraArguments[1],
            "-A", angleValue.ToString(CultureInfo.CurrentCulture),
            extraArguments[2],
            subcommand2Name,
            extraArguments[3],
            "-W", widthValue.ToString(CultureInfo.CurrentCulture),
            subsubcommand2Name,
            extraArguments[4],
            "-H", heightValue.ToString(CultureInfo.CurrentCulture),
            extraArguments[5]
        };

        int angle = default;
        int width = default;
        int height = default;

        var expectedOption1 = new ValueOption<int>(
            string.Empty,
            "A",
            afterValueParsingAction: t => angle = t);

        var expectedOption2 = new ValueOption<int>(
            string.Empty,
            "W",
            afterValueParsingAction: t => width = t + subcommand2Offset);

        var expectedOption3 = new ValueOption<int>(
            string.Empty,
            "H",
            afterValueParsingAction: t => height = t + subsubcommand2Offset);

        var expectedHandledOptions = new ICommonOption[]
        {
            expectedOption1, expectedOption2, expectedOption3
        };

        var options = new ICommonOption[]
        {
            expectedOption1,

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t)
        };

        var subcommand1Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subcommand1Offset)
        };

        var subcommand2Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subcommand2Offset),

            expectedOption2,

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subcommand2Offset)
        };

        var subsubcommand1Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subsubcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subsubcommand1Offset),

            new ValueOption<int>(
                string.Empty,
                "H",
                afterValueParsingAction: t => height = t + subsubcommand1Offset)
        };

        var subsubcommand2Options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                afterValueParsingAction: t => angle = t + subsubcommand2Offset),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t + subsubcommand2Offset),

            expectedOption3,
        };

        var parser = new ArgumentParser();
        parser.AddOptions(options);

        Subcommand subcommand1 = parser.AddSubcommand(subcommand1Name, string.Empty);
        subcommand1.AddOptions(subcommand1Options);

        Subcommand subcommand2 = parser.AddSubcommand(subcommand2Name, string.Empty);
        subcommand2.AddOptions(subcommand2Options);

        Subcommand subsubcommand1 = subcommand2.AddSubcommand(subsubcommand1Name, string.Empty);
        subsubcommand1.AddOptions(subsubcommand1Options);

        Subcommand subsubcommand2 = subcommand2.AddSubcommand(subsubcommand2Name, string.Empty);
        subsubcommand2.AddOptions(subsubcommand2Options);

        ParseArgumentsResult result = parser.ParseKnownArguments(arguments, out _);

        Assert.Equal(expectedHandledOptions.Length, result.HandledOptions.Count);

        foreach (ICommonOption option in expectedHandledOptions)
        {
            int foundOptions = result.HandledOptions.Count(t => t.Equals(option));
            Assert.Equal(1, foundOptions);
        }

        var expextedHandledSubcommands = new Subcommand[]
        {
            subcommand2, subsubcommand2
        };

        foreach (Subcommand subcommand in expextedHandledSubcommands)
        {
            int foundSubcommands = result.HandledSubcommands
                .Count(t => t.Equals(subcommand));

            Assert.Equal(1, foundSubcommands);
        }
    }

    private static void VerifyExtraArguments(
        IEnumerable<string> expected,
        IEnumerable<string> actual)
    {
        ExtendedArgumentNullException.ThrowIfNull(actual, nameof(expected));
        ExtendedArgumentNullException.ThrowIfNull(actual, nameof(actual));

        Assert.True(expected.ScrambledEquals(actual));
    }
}
