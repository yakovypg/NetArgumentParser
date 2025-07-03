using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using NetArgumentParser.Converters;
using NetArgumentParser.Extensions;
using NetArgumentParser.Informing;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Collections;
using NetArgumentParser.Options.Context;
using NetArgumentParser.Tests.Models;

// Necessary for using dynamic
[assembly: InternalsVisibleTo("NetArgumentParser")]

namespace NetArgumentParser.Tests;

public class ArgumentParserTests
{
    [Fact]
    public void Parse_FlagOptions_OptionsHandledCorrectly()
    {
        var arguments = new string[]
        {
            "-w",
            "-xyz",
            "-lr",
            "--debug-mode"
        };

        bool savaLog = false;
        bool autoRotate = false;
        bool debugMode = false;
        bool autoIncreaseWidth = false;
        bool printX = false;
        bool printY = false;
        bool printZ = false;

        var options = new ICommonOption[]
        {
            new FlagOption("save-log", "l", afterHandlingAction: () => savaLog = true),
            new FlagOption("auto-rotate", "r", afterHandlingAction: () => autoRotate = true),
            new FlagOption("debug-mode", "d", afterHandlingAction: () => debugMode = true),
            new FlagOption("auto-increase-width", "w", afterHandlingAction: () => autoIncreaseWidth = true),
            new FlagOption("print-x-coord", "x", afterHandlingAction: () => printX = true),
            new FlagOption("print-y-coord", "y", afterHandlingAction: () => printY = true),
            new FlagOption("print-z-coord", "z", afterHandlingAction: () => printZ = true)
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeCompoundOptions = true
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.True(savaLog);
        Assert.True(autoRotate);
        Assert.True(debugMode);
        Assert.True(autoIncreaseWidth);
        Assert.True(printX);
        Assert.True(printY);
        Assert.True(printZ);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void Parse_EnumValueOptions_OptionsHandledCorrectly()
    {
        var arguments = new string[]
        {
            "-s", StringSplitOptions.RemoveEmptyEntries.ToString(),
            "--bind-mode", BindMode.OneWayToSource.ToString()
        };

        StringSplitOptions splitOption = default;
        BindMode bindMode = default;

        var options = new ICommonOption[]
        {
            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new EnumValueOption<BindMode>(
                "bind-mode",
                "b",
                afterValueParsingAction: t => bindMode = t)
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.Equal(StringSplitOptions.RemoveEmptyEntries, splitOption);
        Assert.Equal(BindMode.OneWayToSource, bindMode);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void Parse_ValueOptions_OptionsHandledCorrectly()
    {
        const string boolValue = "true";
        const string byteValue = "10";
        const string charValue = "c";
        const string dateTimeValue = "1 January 2024";
        const string decimalValue = "1234567890";
        const string doubleValue = "-1e-5";
        const string shortValue = "-10000";
        const string intValue = "-10000000";
        const string longValue = "-10000000000";
        const string sbyteValue = "-10";
        const string floatValue = "100.15";
        const string stringValue = "Some text    with spaces\nAnd $symbols-1234";
        const string ushortValue = "10000";
        const string uintValue = "10000000";
        const string ulongValue = "10000000000";

        bool recievedBool = default;
        byte recievedByte = default;
        char recievedChar = default;
        DateTime recievedDateTime = default;
        decimal recievedDecimal = default;
        double recievedDouble = default;
        short recievedShort = default;
        int recievedInt = default;
        long recievedLong = default;
        sbyte recievedSByte = default;
        float recievedFloat = default;
        string recievedString = string.Empty;
        ushort recievedUShort = default;
        uint recievedUInt = default;
        ulong recievedULong = default;

        var arguments = new string[]
        {
            "-b", boolValue,
            "-B", byteValue,
            "-c", charValue,
            "--date", dateTimeValue,
            "-d", decimalValue,
            "-D", doubleValue,
            "-s", shortValue,
            "-i", intValue,
            "-l", longValue,
            "--sbyte", sbyteValue,
            "-f", floatValue,
            "-S", stringValue,
            "-U", ushortValue,
            "-I", uintValue,
            "-L", ulongValue
        };

        var options = new ICommonOption[]
        {
            new ValueOption<bool>(string.Empty, "b", afterValueParsingAction: t => recievedBool = t),
            new ValueOption<byte>(string.Empty, "B", afterValueParsingAction: t => recievedByte = t),
            new ValueOption<char>(string.Empty, "c", afterValueParsingAction: t => recievedChar = t),
            new ValueOption<DateTime>("date", string.Empty, afterValueParsingAction: t => recievedDateTime = t),
            new ValueOption<decimal>(string.Empty, "d", afterValueParsingAction: t => recievedDecimal = t),
            new ValueOption<double>(string.Empty, "D", afterValueParsingAction: t => recievedDouble = t),
            new ValueOption<short>(string.Empty, "s", afterValueParsingAction: t => recievedShort = t),
            new ValueOption<int>(string.Empty, "i", afterValueParsingAction: t => recievedInt = t),
            new ValueOption<long>(string.Empty, "l", afterValueParsingAction: t => recievedLong = t),
            new ValueOption<sbyte>("sbyte", string.Empty, afterValueParsingAction: t => recievedSByte = t),
            new ValueOption<float>(string.Empty, "f", afterValueParsingAction: t => recievedFloat = t),
            new ValueOption<string>(string.Empty, "S", afterValueParsingAction: t => recievedString = t),
            new ValueOption<ushort>(string.Empty, "U", afterValueParsingAction: t => recievedUShort = t),
            new ValueOption<uint>(string.Empty, "I", afterValueParsingAction: t => recievedUInt = t),
            new ValueOption<ulong>(string.Empty, "L", afterValueParsingAction: t => recievedULong = t)
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.Equal(bool.Parse(boolValue), recievedBool);
        Assert.Equal(byte.Parse(byteValue, CultureInfo.CurrentCulture), recievedByte);
        Assert.Equal(char.Parse(charValue), recievedChar);
        Assert.Equal(DateTime.Parse(dateTimeValue, CultureInfo.CurrentCulture), recievedDateTime);
        Assert.Equal(decimal.Parse(decimalValue, CultureInfo.CurrentCulture), recievedDecimal);
        Assert.Equal(double.Parse(doubleValue, CultureInfo.CurrentCulture), recievedDouble);
        Assert.Equal(short.Parse(shortValue, CultureInfo.CurrentCulture), recievedShort);
        Assert.Equal(int.Parse(intValue, CultureInfo.CurrentCulture), recievedInt);
        Assert.Equal(long.Parse(longValue, CultureInfo.CurrentCulture), recievedLong);
        Assert.Equal(sbyte.Parse(sbyteValue, CultureInfo.CurrentCulture), recievedSByte);
        Assert.Equal(float.Parse(floatValue, CultureInfo.CurrentCulture), recievedFloat);
        Assert.Equal(stringValue, recievedString);
        Assert.Equal(ushort.Parse(ushortValue, CultureInfo.CurrentCulture), recievedUShort);
        Assert.Equal(uint.Parse(uintValue, CultureInfo.CurrentCulture), recievedUInt);
        Assert.Equal(ulong.Parse(ulongValue, CultureInfo.CurrentCulture), recievedULong);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void Parse_MultipleValueOptions_OptionsHandledCorrectly()
    {
        const string leftMargin = "10";
        const string topMargin = "20";
        const string rightMargin = "30";
        const string bottomMargin = "40";

        const string pointX = "-15.123";
        const string pointY = "100.987654321";

        const string file1 = "/home/user1/file1.txt";
        const string file2 = "file2.png";
        const string file3 = "./file3";

        Margin? margin = null;
        Point? point = null;
        List<string>? files = null;

        var arguments = new string[]
        {
            "-m", leftMargin, topMargin, rightMargin, bottomMargin,
            "-f", file1, file2, file3,
            "-p", pointX, pointY
        };

        var options = new ICommonOption[]
        {
            new MultipleValueOption<int>(
                string.Empty,
                "m",
                contextCapture: new FixedContextCapture(4),
                afterValueParsingAction: t => margin = new Margin(t[0], t[1], t[2], t[3])),

            new MultipleValueOption<string>(
                string.Empty,
                "f",
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = new List<string>(t)),

            new MultipleValueOption<double>(
                string.Empty,
                "p",
                contextCapture: new FixedContextCapture(2),
                afterValueParsingAction: t => point = new Point(t[0], t[1]))
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        var expectedMargin = new Margin(
            int.Parse(leftMargin, CultureInfo.CurrentCulture),
            int.Parse(topMargin, CultureInfo.CurrentCulture),
            int.Parse(rightMargin, CultureInfo.CurrentCulture),
            int.Parse(bottomMargin, CultureInfo.CurrentCulture));

        var expectedPoint = new Point(
            double.Parse(pointX, CultureInfo.CurrentCulture),
            double.Parse(pointY, CultureInfo.CurrentCulture));

        List<string> expectedFiles = [file1, file2, file3];

        Assert.Equal(expectedMargin, margin);
        Assert.Equal(expectedPoint, point);
        Assert.Equal(expectedFiles, files);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void Parse_ValueOptions_SpecialConverterApplied()
    {
        const int leftMargin = 10;
        const int topMargin = -20;
        const int rightMargin = 30;
        const int bottomMargin = -40;

        const double pointX = 15.14;
        const double pointY = -14.15;

        const int inputAngle = -45;

        int? angle = null;
        Margin? margin = null;
        Point? point = null;

        var arguments = new string[]
        {
            "-m", $"{leftMargin},{topMargin},{rightMargin},{bottomMargin}",
            "-a", inputAngle.ToString(CultureInfo.CurrentCulture),
            "-p", $"({pointX};{pointY})"
        };

        var options = new ICommonOption[]
        {
            new ValueOption<int>(string.Empty, "a", afterValueParsingAction: t => angle = t),
            new ValueOption<Margin>(string.Empty, "m", afterValueParsingAction: t => margin = t),
            new ValueOption<Point>(string.Empty, "p", afterValueParsingAction: t => point = t)
        };

        var converters = new IValueConverter[]
        {
            new ValueConverter<Margin>(t =>
            {
                int[] data = t.Split(',').Select(int.Parse).ToArray();
                return new Margin(data[0], data[1], data[2], data[3]);
            }),

            new ValueConverter<Point>(t =>
            {
                double[] data = t[1..(t.Length - 1)]
                    .Split(';')
                    .Select(double.Parse)
                    .ToArray();

                return new Point(data[0], data[1]);
            }),

            new ValueConverter<int>(
                t => Math.Abs(int.Parse(t, CultureInfo.CurrentCulture)))
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false
        };

        parser.AddOptions(options);
        parser.AddConverters(converters);

        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        var expectedMargin = new Margin(
            leftMargin,
            topMargin,
            rightMargin,
            bottomMargin);

        var expectedPoint = new Point(pointX, pointY);

        Assert.Equal(Math.Abs(inputAngle), angle);
        Assert.Equal(expectedMargin, margin);
        Assert.Equal(expectedPoint, point);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void Parse_ValueOptions_DefaultValueApplied()
    {
        const int defaultAngle = 45;
        const double defaultWidth = 100.5;
        const double defaultHeight = 400.25;
        const string defaultName = "name";

        const int expectedAngle = defaultAngle;
        const double expectedWidth = 1920;
        const double expectedHeight = defaultHeight;
        const string expectedName = "some_name";

        int angle = default;
        double width = default;
        double height = default;
        string name = string.Empty;

        var arguments = new string[]
        {
            "-n", expectedName,
            "-w", expectedWidth.ToString(CultureInfo.CurrentCulture)
        };

        var options = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "a",
                defaultValue: new DefaultOptionValue<int>(defaultAngle),
                afterValueParsingAction: t => angle = t),

            new ValueOption<double>(
                string.Empty,
                "w",
                defaultValue: new DefaultOptionValue<double>(defaultWidth),
                afterValueParsingAction: t => width = t),

            new ValueOption<double>(
                string.Empty,
                "h",
                defaultValue: new DefaultOptionValue<double>(defaultHeight),
                afterValueParsingAction: t => height = t),

            new ValueOption<string>(
                string.Empty,
                "n",
                defaultValue: new DefaultOptionValue<string>(defaultName),
                afterValueParsingAction: t => name = t)
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.Equal(expectedAngle, angle);
        Assert.Equal(expectedWidth, width);
        Assert.Equal(expectedHeight, height);
        Assert.Equal(expectedName, name);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void Parse_ValueOptions_ThrowsExceptionIfRequiredOptionNotHandled()
    {
        var arguments = new string[]
        {
            "-n", "name",
            "-w", "100.5"
        };

        var requiredOptions = new List<ICommonOption>()
        {
            new ValueOption<double>(
                string.Empty,
                "w",
                isRequired: true),

            new ValueOption<double>(
                string.Empty,
                "h",
                isRequired: true,
                defaultValue: new DefaultOptionValue<double>(1080)),

            new ValueOption<string>(
                string.Empty,
                "n",
                isRequired: true,
                defaultValue: new DefaultOptionValue<string>("name"))
        };

        var notSpecifiedRequiredOptionWithoutDefaultValue = new List<ICommonOption>()
        {
            new ValueOption<int>(
                string.Empty,
                "a",
                isRequired: true),

            new ValueOption<string>(
                string.Empty,
                "b",
                isRequired: true),

            new ValueOption<float>(
                string.Empty,
                "c",
                isRequired: true)
        };

        var notRequiredOptions = new List<ICommonOption>()
        {
            new ValueOption<int>(
                string.Empty,
                "A",
                isRequired: false),

            new ValueOption<double>(
                string.Empty,
                "o",
                isRequired: false)
        };

        IEnumerable<ICommonOption> allOptions = requiredOptions
            .Concat(notSpecifiedRequiredOptionWithoutDefaultValue)
            .Concat(notRequiredOptions);

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false
        };

        parser.AddOptions(allOptions.ToArray());

        foreach (ICommonOption option in notSpecifiedRequiredOptionWithoutDefaultValue)
        {
            Assert.Throws<RequiredOptionNotSpecifiedException>(() =>
            {
                _ = parser.Parse(arguments);
            });

            parser.RemoveOption(option);
            parser.ResetOptionsHandledState();
        }

        Exception? ex = Record.Exception(() => parser.Parse(arguments));
        Assert.Null(ex);
    }

    [Fact]
    public void Parse_ValueOptions_ThrowsExceptionIfValueNotSatisfyRestriction()
    {
        var arguments = new string[]
        {
            "-H", "1",
            "-w", "-1",
            "-a", "45",
            "--name", "John15",
            "--fruit", "apple",
            "-o", "0.5"
        };

        var optionsWithIncorrectChoice = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "w",
                valueRestriction: new OptionValueRestriction<int>(t => t > 0)),

            new ValueOption<double>(
                string.Empty,
                "H",
                valueRestriction: new OptionValueRestriction<double>(t => t > -1 && t < 1)),

            new ValueOption<string>(
                "name",
                string.Empty,
                valueRestriction: new OptionValueRestriction<string>(t => !t.Any(c => char.IsDigit(c))))
        };

        var optionsWithCorrectChoice = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "a",
                valueRestriction: new OptionValueRestriction<int>(t => t > 0)),

            new ValueOption<double>(
                string.Empty,
                "o",
                valueRestriction: new OptionValueRestriction<double>(t => t > -1 && t < 1)),

            new ValueOption<string>(
                "fruit",
                string.Empty,
                valueRestriction: new OptionValueRestriction<string>(t => !t.Any(c => char.IsDigit(c))))
        };

        var allOptions = optionsWithIncorrectChoice.Concat(optionsWithCorrectChoice);

        var parser = new ArgumentParser();
        parser.AddOptions(allOptions.ToArray());

        foreach (ICommonOption option in optionsWithIncorrectChoice)
        {
            Assert.Throws<OptionValueNotSatisfyRestrictionException>(() =>
            {
                _ = parser.ParseKnownArguments(arguments, out _);
            });

            parser.RemoveOption(option);
            parser.ResetOptionsHandledState();
        }

        Exception? ex = Record.Exception(
            () => parser.ParseKnownArguments(arguments, out _));

        Assert.Null(ex);
    }

    [Fact]
    public void Parse_MultipleValueOptions_ThrowsExceptionIfValueNotSatisfyRestriction()
    {
        var arguments = new string[]
        {
            "-m", "4", "3", "2", "1",
            "-n", "0",
            "-k", "3", "2", "1",
            "--files", "img.jpg", "img.png"
        };

        var optionsWithIncorrectChoice = new ICommonOption[]
        {
            new MultipleValueOption<byte>(
                "margin",
                "m",
                contextCapture: new FixedContextCapture(4),
                valueRestriction: new OptionValueRestriction<IList<byte>>(t => t.Contains(5))),

            new MultipleValueOption<string>(
                string.Empty,
                "n",
                contextCapture: new ZeroOrMoreContextCapture(),
                valueRestriction: new OptionValueRestriction<IList<string>>(t => !t.Contains("0")))
        };

        var optionsWithCorrectChoice = new ICommonOption[]
        {
            new MultipleValueOption<byte>(
                string.Empty,
                "k",
                contextCapture: new OneOrMoreContextCapture(),
                valueRestriction: new OptionValueRestriction<IList<byte>>(t => !t.Contains(4))),

            new MultipleValueOption<string>(
                "files",
                "f",
                contextCapture: new ZeroOrMoreContextCapture(),
                valueRestriction: new OptionValueRestriction<IList<string>>(t => t.Count > 1))
        };

        var allOptions = optionsWithIncorrectChoice.Concat(optionsWithCorrectChoice);

        var parser = new ArgumentParser();
        parser.AddOptions(allOptions.ToArray());

        foreach (ICommonOption option in optionsWithIncorrectChoice)
        {
            Assert.Throws<OptionValueNotSatisfyRestrictionException>(() =>
            {
                _ = parser.ParseKnownArguments(arguments, out _);
            });

            parser.RemoveOption(option);
            parser.ResetOptionsHandledState();
        }

        Exception? ex = Record.Exception(
            () => parser.ParseKnownArguments(arguments, out _));

        Assert.Null(ex);
    }

    [Fact]
    public void Parse_ValueOptions_ThrowsExceptionIfValueNotSatisfyChoices()
    {
        var arguments = new string[]
        {
            "-H", "1080",
            "-w", "1000",
            "-a", "45",
            "--name", "John",
            "--fruit", "banana",
            "-o", "0.5"
        };

        var optionsWithIncorrectChoice = new ICommonOption[]
        {
            new ValueOption<int>(string.Empty, "w", choices: [1920, 1366, 1280]),
            new ValueOption<double>(string.Empty, "H", choices: [1080, 768, 720]),
            new ValueOption<string>("name", string.Empty, choices: ["Tom"])
        };

        var optionsWithCorrectChoice = new ICommonOption[]
        {
            new ValueOption<int>(string.Empty, "a", choices: [0, 45, 90]),
            new ValueOption<double>(string.Empty, "o", choices: [0.1, 0.5, 1]),
            new ValueOption<string>("fruit", string.Empty, choices: ["apple", "banana"])
        };

        var allOptions = optionsWithIncorrectChoice.Concat(optionsWithCorrectChoice);

        var parser = new ArgumentParser();
        parser.AddOptions(allOptions.ToArray());

        foreach (ICommonOption option in optionsWithIncorrectChoice)
        {
            Assert.Throws<OptionValueNotSatisfyChoicesException>(() =>
            {
                _ = parser.ParseKnownArguments(arguments, out _);
            });

            parser.RemoveOption(option);
            parser.ResetOptionsHandledState();
        }

        Exception? ex = Record.Exception(
            () => parser.ParseKnownArguments(arguments, out _));

        Assert.Null(ex);
    }

    [Fact]
    public void Parse_EnumValueOptions_ThrowsExceptionIfValueNotSatisfyChoices()
    {
        var arguments = new string[]
        {
            "-a", BindMode.OneWay.ToString(),
            "-c", BindMode.OneWay.ToString(),
            "-b", StringSplitOptions.None.ToString(),
            "-d", StringSplitOptions.RemoveEmptyEntries.ToString()
        };

        var optionsWithIncorrectChoice = new ICommonOption[]
        {
            new EnumValueOption<BindMode>(
                string.Empty,
                "a",
                useDefaultChoices: false,
                choices: [BindMode.TwoWay, BindMode.OneWayToSource]),

            new EnumValueOption<StringSplitOptions>(
                string.Empty,
                "b",
                useDefaultChoices: false,
                choices: [StringSplitOptions.RemoveEmptyEntries])
        };

        var optionsWithCorrectChoice = new ICommonOption[]
        {
            new EnumValueOption<BindMode>(
                string.Empty,
                "c",
                useDefaultChoices: false,
                choices: [BindMode.OneWay, BindMode.OneWayToSource]),

            new EnumValueOption<StringSplitOptions>(
                string.Empty,
                "d",
                useDefaultChoices: false,
                choices: [StringSplitOptions.RemoveEmptyEntries])
        };

        var allOptions = optionsWithIncorrectChoice.Concat(optionsWithCorrectChoice);

        var parser = new ArgumentParser();
        parser.AddOptions(allOptions.ToArray());

        foreach (ICommonOption option in optionsWithIncorrectChoice)
        {
            Assert.Throws<OptionValueNotSatisfyChoicesException>(() =>
            {
                _ = parser.ParseKnownArguments(arguments, out _);
            });

            parser.RemoveOption(option);
            parser.ResetOptionsHandledState();
        }

        Exception? ex = Record.Exception(
            () => parser.ParseKnownArguments(arguments, out _));

        Assert.Null(ex);
    }

    [Fact]
    public void Parse_MultipleValueOptions_ThrowsExceptionIfValueNotSatisfyChoices()
    {
        var arguments = new string[]
        {
            "-m", "4", "3", "2", "1",
            "-n", "1000",
            "-k", "3", "2", "1",
            "--files", "img.jpg", "img.png"
        };

        var optionsWithIncorrectChoice = new ICommonOption[]
        {
            new MultipleValueOption<byte>(
                "margin",
                "m",
                contextCapture: new FixedContextCapture(4),
                choices: [[1, 2, 3, 4], [0, 0, 1]]),

            new MultipleValueOption<string>(
                string.Empty,
                "n",
                contextCapture: new ZeroOrMoreContextCapture(),
                choices: [["1", "2", "3"], ["3", "2", "1"]])
        };

        var optionsWithCorrectChoice = new ICommonOption[]
        {
            new MultipleValueOption<byte>(
                string.Empty,
                "k",
                contextCapture: new OneOrMoreContextCapture(),
                choices: [[1], [3, 2, 1]]),

            new MultipleValueOption<string>(
                "files",
                "f",
                contextCapture: new ZeroOrMoreContextCapture(),
                choices: [["img.jpg", "img.png"]])
        };

        var allOptions = optionsWithIncorrectChoice.Concat(optionsWithCorrectChoice);

        var parser = new ArgumentParser();
        parser.AddOptions(allOptions.ToArray());

        foreach (ICommonOption option in optionsWithIncorrectChoice)
        {
            Assert.Throws<OptionValueNotSatisfyChoicesException>(() =>
            {
                _ = parser.ParseKnownArguments(arguments, out _);
            });

            parser.RemoveOption(option);
            parser.ResetOptionsHandledState();
        }

        Exception? ex = Record.Exception(
            () => parser.ParseKnownArguments(arguments, out _));

        Assert.Null(ex);
    }

    [Fact]
    public void Parse_ValueOptions_DoNotThrowsExceptionIfValueNotSatisfyChoicesDueCase()
    {
        var arguments = new string[]
        {
            "--item", "aPpLe",
            "--first-name", "max",
            "--second-name", "SMITH"
        };

        var options = new ICommonOption[]
        {
            new ValueOption<string>(
                "item",
                string.Empty,
                ignoreCaseInChoices: true,
                choices: ["Banana", "Apple", "Grape"]),

            new ValueOption<string>(
                "first-name",
                string.Empty,
                ignoreCaseInChoices: true,
                choices: ["Max"]),

            new ValueOption<string>(
                "second-name",
                string.Empty,
                ignoreCaseInChoices: true,
                choices: ["Smith", "Brown"])
        };

        var parser = new ArgumentParser();
        parser.AddOptions(options);

        Exception? ex = Record.Exception(() => parser.Parse(arguments));
        Assert.Null(ex);
    }

    [Fact]
    public void Parse_MultipleValueOptions_DoNotThrowsExceptionIfValueNotSatisfyChoicesDueCase()
    {
        var arguments = new string[]
        {
            "--items", "bAnAnA", "apple", "GRAPE"
        };

        var options = new ICommonOption[]
        {
            new MultipleValueOption<string>(
                "items",
                "i",
                ignoreCaseInChoices: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                choices: [["Banana", "Apple", "Grape"], ["Cucumber", "Tomato"]])
        };

        var parser = new ArgumentParser();
        parser.AddOptions(options);

        Exception? ex = Record.Exception(() => parser.Parse(arguments));
        Assert.Null(ex);
    }

    [Fact]
    public void Parse_MultipleValueOptions_DoNotThrowsExceptionIfValueNotSatisfyChoicesDueOrder()
    {
        var arguments = new string[]
        {
            "-n", "7", "0", "1", "0",
            "--items", "Apple", "Grape", "Banana"
        };

        var options = new ICommonOption[]
        {
            new MultipleValueOption<byte>(
                "numbers",
                "n",
                ignoreOrderInChoices: true,
                contextCapture: new FixedContextCapture(4),
                choices: [[1, 2, 3, 4], [0, 0, 7, 1]]),

            new MultipleValueOption<string>(
                "items",
                "i",
                ignoreOrderInChoices: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                choices: [["Banana", "Apple", "Grape"], ["Cucumber", "Tomato"]])
        };

        var parser = new ArgumentParser();
        parser.AddOptions(options);

        Exception? ex = Record.Exception(() => parser.Parse(arguments));
        Assert.Null(ex);
    }

    [Fact]
    public void Parse_MultipleValueOptions_DoNotThrowsExceptionIfValueNotSatisfyChoicesDueCaseAndOrder()
    {
        var arguments = new string[]
        {
            "--items", "apple", "GRAPE", "bAnAnA"
        };

        var options = new ICommonOption[]
        {
            new MultipleValueOption<string>(
                "items",
                "i",
                ignoreCaseInChoices: true,
                ignoreOrderInChoices: true,
                contextCapture: new ZeroOrMoreContextCapture(),
                choices: [["Banana", "Apple", "Grape"], ["Cucumber", "Tomato"]])
        };

        var parser = new ArgumentParser();
        parser.AddOptions(options);

        Exception? ex = Record.Exception(() => parser.Parse(arguments));
        Assert.Null(ex);
    }

    [Fact]
    public void Parse_ValueOptions_ThrowsExceptionIfValueNotSatisfyBeforeParseChoices()
    {
        var arguments = new string[]
        {
            "-H", "1080",
            "-w", "1000",
            "-a", "45",
            "--name", "John",
            "--fruit", "banana",
            "-o", "0.5"
        };

        var optionsWithIncorrectChoice = new ICommonOption[]
        {
            new ValueOption<int>(string.Empty, "w", beforeParseChoices: ["1920", "1366", "1280"]),
            new ValueOption<double>(string.Empty, "H", beforeParseChoices: ["1080", "768", "720"]),
            new ValueOption<string>("name", string.Empty, beforeParseChoices: ["Tom"])
        };

        var optionsWithCorrectChoice = new ICommonOption[]
        {
            new ValueOption<int>(string.Empty, "a", beforeParseChoices: ["0", "45", "90"]),
            new ValueOption<double>(string.Empty, "o", beforeParseChoices: ["0.1", "0.5", "1"]),
            new ValueOption<string>("fruit", string.Empty, beforeParseChoices: ["apple", "banana"])
        };

        var allOptions = optionsWithIncorrectChoice.Concat(optionsWithCorrectChoice);

        var parser = new ArgumentParser();
        parser.AddOptions([.. allOptions]);

        foreach (ICommonOption option in optionsWithIncorrectChoice)
        {
            Assert.Throws<OptionValueNotSatisfyChoicesException>(() =>
            {
                _ = parser.ParseKnownArguments(arguments, out _);
            });

            parser.RemoveOption(option);
            parser.ResetOptionsHandledState();
        }

        Exception? ex = Record.Exception(
            () => parser.ParseKnownArguments(arguments, out _));

        Assert.Null(ex);
    }

    [Fact]
    public void Parse_EnumValueOptions_ThrowsExceptionIfValueNotSatisfyBeforeParseChoices()
    {
        var arguments = new string[]
        {
            "-a", BindMode.OneWay.ToString(),
            "-c", BindMode.OneWay.ToString(),
            "-b", StringSplitOptions.None.ToString(),
            "-d", StringSplitOptions.RemoveEmptyEntries.ToString()
        };

        var optionsWithIncorrectChoice = new ICommonOption[]
        {
            new EnumValueOption<BindMode>(
                string.Empty,
                "a",
                useDefaultChoices: false,
                beforeParseChoices: [BindMode.TwoWay.ToString(), BindMode.OneWayToSource.ToString()]),

            new EnumValueOption<StringSplitOptions>(
                string.Empty,
                "b",
                useDefaultChoices: false,
                beforeParseChoices: [StringSplitOptions.RemoveEmptyEntries.ToString()])
        };

        var optionsWithCorrectChoice = new ICommonOption[]
        {
            new EnumValueOption<BindMode>(
                string.Empty,
                "c",
                useDefaultChoices: false,
                beforeParseChoices: [BindMode.OneWay.ToString(), BindMode.OneWayToSource.ToString()]),

            new EnumValueOption<StringSplitOptions>(
                string.Empty,
                "d",
                useDefaultChoices: false,
                beforeParseChoices: [StringSplitOptions.RemoveEmptyEntries.ToString()])
        };

        var allOptions = optionsWithIncorrectChoice.Concat(optionsWithCorrectChoice);

        var parser = new ArgumentParser();
        parser.AddOptions(allOptions.ToArray());

        foreach (ICommonOption option in optionsWithIncorrectChoice)
        {
            Assert.Throws<OptionValueNotSatisfyChoicesException>(() =>
            {
                _ = parser.ParseKnownArguments(arguments, out _);
            });

            parser.RemoveOption(option);
            parser.ResetOptionsHandledState();
        }

        Exception? ex = Record.Exception(
            () => parser.ParseKnownArguments(arguments, out _));

        Assert.Null(ex);
    }

    [Fact]
    public void Parse_ValueOptions_ThrowsExceptionIfValueNotSatisfyChoicesButSatisfyBeforeParseChoices()
    {
        var arguments = new string[]
        {
            "-H", "1080",
            "-w", "1000",
            "-a", "45",
            "--name", "John",
            "--fruit", "banana",
            "-o", "0.5"
        };

        var optionsWithIncorrectChoice = new ICommonOption[]
        {
            new ValueOption<int>(
                string.Empty,
                "w",
                choices: [1920, 1366, 1280],
                beforeParseChoices: ["0", "45", "90"]),

            new ValueOption<double>(
                string.Empty,
                "H",
                choices: [1080, 768, 720],
                beforeParseChoices: ["0.1", "0.5", "1"]),

            new ValueOption<string>(
                "name",
                string.Empty,
                choices: ["Tom"],
                beforeParseChoices: ["apple", "banana"])
        };

        var optionsWithCorrectChoice = new ICommonOption[]
        {
            new ValueOption<int>(string.Empty, "a", beforeParseChoices: ["0", "45", "90"]),
            new ValueOption<double>(string.Empty, "o", beforeParseChoices: ["0.1", "0.5", "1"]),
            new ValueOption<string>("fruit", string.Empty, beforeParseChoices: ["apple", "banana"])
        };

        var allOptions = optionsWithIncorrectChoice.Concat(optionsWithCorrectChoice);

        var parser = new ArgumentParser();
        parser.AddOptions([.. allOptions]);

        foreach (ICommonOption option in optionsWithIncorrectChoice)
        {
            Assert.Throws<OptionValueNotSatisfyChoicesException>(() =>
            {
                _ = parser.ParseKnownArguments(arguments, out _);
            });

            parser.RemoveOption(option);
            parser.ResetOptionsHandledState();
        }

        Exception? ex = Record.Exception(
            () => parser.ParseKnownArguments(arguments, out _));

        Assert.Null(ex);
    }

    [Fact]
    public void Parse_ValueOptions_DoNotThrowsExceptionIfValueNotSatisfyBeforeParseChoicesDueCase()
    {
        var arguments = new string[]
        {
            "--item", "aPpLe",
            "--first-name", "max",
            "--second-name", "SMITH"
        };

        var options = new ICommonOption[]
        {
            new ValueOption<string>(
                "item",
                string.Empty,
                ignoreCaseInChoices: true,
                beforeParseChoices: ["Banana", "Apple", "Grape"]),

            new ValueOption<string>(
                "first-name",
                string.Empty,
                ignoreCaseInChoices: true,
                beforeParseChoices: ["Max"]),

            new ValueOption<string>(
                "second-name",
                string.Empty,
                ignoreCaseInChoices: true,
                beforeParseChoices: ["Smith", "Brown"])
        };

        var parser = new ArgumentParser();
        parser.AddOptions(options);

        Exception? ex = Record.Exception(() => parser.Parse(arguments));
        Assert.Null(ex);
    }

    [Fact]
    public void Parse_OptionsWithAliases_OptionsHandledCorrectly()
    {
        const int expectedAngle = 45;
        const BindMode expectedBindMode = BindMode.OneWayToSource;

        var arguments = new string[]
        {
            "-A", expectedAngle.ToString(CultureInfo.CurrentCulture),
            "--binding", expectedBindMode.ToString()
        };

        int angle = default;
        BindMode bindMode = default;

        var options = new ICommonOption[]
        {
            new ValueOption<int>(
                "angle",
                string.Empty,
                aliases: ["ang", "A", "rotation"],
                afterValueParsingAction: t => angle = t),

            new EnumValueOption<BindMode>(
                "bind-mode",
                "b",
                aliases: ["binding"],
                afterValueParsingAction: t => bindMode = t)
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.Equal(expectedAngle, angle);
        Assert.Equal(expectedBindMode, bindMode);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void Parse_MinusBasedOptions_ExtraArgumentsExtracted()
    {
        var expectedExtraArguments = new string[]
        {
            "--height",
            "900",
            "24",
            "-h",
            "-125",
            "None",
            "--q"
        };

        var arguments = new string[]
        {
            "myapp",
            "-lr",
            expectedExtraArguments[0],
            expectedExtraArguments[1],
            "--margin", "15", "10", "5", "15",
            expectedExtraArguments[2],
            "-f", "/home/usr/file1.txt", "/home/usr/file2.png", "./file3",
            expectedExtraArguments[3],
            "-a", "-153.123",
            "--width", "500",
            expectedExtraArguments[4],
            "--split-option", StringSplitOptions.RemoveEmptyEntries.ToString(),
            expectedExtraArguments[5],
            expectedExtraArguments[6]
        };

        var options = new ICommonOption[]
        {
            new FlagOption("save_log", "l"),
            new FlagOption("auto-rotate", "r"),
            new EnumValueOption<StringSplitOptions>("split-option", "s"),
            new ValueOption<int>("width", "w"),
            new ValueOption<double>("angle", "a"),

            new MultipleValueOption<byte>(
                "margin",
                "m",
                contextCapture: new FixedContextCapture(4)),

            new MultipleValueOption<string>(
                "files",
                "f",
                contextCapture: new ZeroOrMoreContextCapture())
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            NumberOfArgumentsToSkip = 1,
            RecognizeCompoundOptions = true
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        VerifyExtraArguments(expectedExtraArguments, extraArguments);
    }

    [Fact]
    public void Parse_SlashBasedOptions_ExtraArgumentsExtracted()
    {
        var expectedExtraArguments = new string[]
        {
            "/Height",
            "900",
            "24",
            "/h",
            "-125",
            "None",
            "--q"
        };

        var arguments = new string[]
        {
            "myapp",
            "-lr",
            expectedExtraArguments[0],
            expectedExtraArguments[1],
            "/Margin", "15", "10", "5", "15",
            expectedExtraArguments[2],
            "-f", "C://path//file1.txt", @"D:\path\file2.png", "./file3", "file4",
            expectedExtraArguments[3],
            "/a", "-153.123",
            "--width", "500",
            expectedExtraArguments[4],
            "/split-option", StringSplitOptions.RemoveEmptyEntries.ToString(),
            expectedExtraArguments[5],
            expectedExtraArguments[6]
        };

        var options = new ICommonOption[]
        {
            new FlagOption("save_log", "l"),
            new FlagOption("auto-rotate", "r"),
            new EnumValueOption<StringSplitOptions>("split-option", "s"),
            new ValueOption<int>("width", "w"),
            new ValueOption<double>("angle", "a"),

            new MultipleValueOption<byte>(
                "Margin",
                "m",
                contextCapture: new FixedContextCapture(4)),

            new MultipleValueOption<string>(
                "files",
                "f",
                contextCapture: new ZeroOrMoreContextCapture())
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            NumberOfArgumentsToSkip = 1,
            RecognizeCompoundOptions = true,
            RecognizeSlashOptions = true
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        VerifyExtraArguments(expectedExtraArguments, extraArguments);
    }

    [Fact]
    public void Parse_SlashBasedOptionsDisabled_SlashBasedOptionsNotRecognized()
    {
        const StringSplitOptions expectedSplitOption = StringSplitOptions.RemoveEmptyEntries;

        const string file1 = "./file1";
        const string file2 = "/file2";
        const string file3 = "file3";
        const string file4 = "/file4";

        List<string> expectedFiles = [file1, file2, file3, file4];

        bool saveLog = default;
        int width = default;
        StringSplitOptions splitOption = default;
        List<string> files = [];

        var arguments = new string[]
        {
            "/l",
            "--Split", expectedSplitOption.ToString(),
            "/W", "100",
            "/Split", StringSplitOptions.RemoveEmptyEntries.ToString(),
            "--files", file1, file2, file3, file4
        };

        var expectedExtraArguments = new string[]
        {
            arguments[0],
            arguments[3],
            arguments[4],
            arguments[5],
            arguments[6]
        };

        var options = new ICommonOption[]
        {
            new FlagOption(
                string.Empty,
                "l",
                afterHandlingAction: () => saveLog = true),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t),

            new EnumValueOption<StringSplitOptions>(
                "Split",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                afterValueParsingAction: t => files = [..t])
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeSlashOptions = false
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.Equal(default, saveLog);
        Assert.Equal(default, width);
        Assert.Equal(expectedSplitOption, splitOption);
        Assert.Equal(expectedFiles, files);

        VerifyExtraArguments(expectedExtraArguments, extraArguments);
    }

    [Fact]
    public void Parse_SlashBasedOptionsEnabled_SlashBasedOptionsRecognized()
    {
        const int expectedWidth = 100;
        const StringSplitOptions expectedSplitOption = StringSplitOptions.RemoveEmptyEntries;

        const string file1 = "./file1";
        const string file2 = "/file2";
        const string file3 = "file3";
        const string file4 = "/file4";

        List<string> expectedFiles = [file1];

        bool saveLog = default;
        int width = default;
        StringSplitOptions splitOption = default;
        List<string> files = [];

        var arguments = new string[]
        {
            "/l",
            "/W", expectedWidth.ToString(CultureInfo.CurrentCulture),
            "/Split", expectedSplitOption.ToString(),
            "--files", file1, file2, file3, file4
        };

        var expectedExtraArguments = new string[]
        {
            arguments[7],
            arguments[8],
            arguments[9]
        };

        var options = new ICommonOption[]
        {
            new FlagOption(
                string.Empty,
                "l",
                afterHandlingAction: () => saveLog = true),

            new ValueOption<int>(
                string.Empty,
                "W",
                afterValueParsingAction: t => width = t),

            new EnumValueOption<StringSplitOptions>(
                "Split",
                afterValueParsingAction: t => splitOption = t),

            new MultipleValueOption<string>(
                "files",
                afterValueParsingAction: t => files = [..t])
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeSlashOptions = true
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.True(saveLog);
        Assert.Equal(expectedWidth, width);
        Assert.Equal(expectedSplitOption, splitOption);
        Assert.Equal(expectedFiles, files);

        VerifyExtraArguments(expectedExtraArguments, extraArguments);
    }

    [Fact]
    public void Parse_CompoundOptionsDisabled_OptionsNotExpanded()
    {
        var arguments = new string[]
        {
            "-a",
            "-abc",
            "-bc"
        };

        var expectedExtraArguments = new string[]
        {
            arguments[^1]
        };

        bool a = false;
        bool b = false;
        bool c = false;
        bool abc = false;

        var options = new ICommonOption[]
        {
            new FlagOption(string.Empty, "a", afterHandlingAction: () => a = true),
            new FlagOption(string.Empty, "b", afterHandlingAction: () => b = true),
            new FlagOption(string.Empty, "c", afterHandlingAction: () => c = true),
            new FlagOption(string.Empty, "abc", afterHandlingAction: () => abc = true)
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeCompoundOptions = false
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.True(a);
        Assert.True(abc);
        Assert.False(b);
        Assert.False(c);

        VerifyExtraArguments(expectedExtraArguments, extraArguments);
    }

    [Fact]
    public void Parse_CompoundOptionsEnabled_OptionsExpanded()
    {
        var arguments = new string[]
        {
            "-abc",
            "-d"
        };

        bool a = false;
        bool b = false;
        bool c = false;
        bool d = false;

        var options = new ICommonOption[]
        {
            new FlagOption(string.Empty, "a", afterHandlingAction: () => a = true),
            new FlagOption(string.Empty, "b", afterHandlingAction: () => b = true),
            new FlagOption(string.Empty, "c", afterHandlingAction: () => c = true),
            new FlagOption(string.Empty, "d", afterHandlingAction: () => d = true)
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeCompoundOptions = true
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.True(a);
        Assert.True(b);
        Assert.True(c);
        Assert.True(d);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void Parse_NotSkipFirstArguments_ArgumentsNotSkipped()
    {
        var arguments = new string[]
        {
            "-a",
            "-b",
            "-c",
            "-d"
        };

        bool a = false;
        bool b = false;
        bool c = false;
        bool d = false;

        var options = new ICommonOption[]
        {
            new FlagOption(string.Empty, "a", afterHandlingAction: () => a = true),
            new FlagOption(string.Empty, "b", afterHandlingAction: () => b = true),
            new FlagOption(string.Empty, "c", afterHandlingAction: () => c = true),
            new FlagOption(string.Empty, "d", afterHandlingAction: () => d = true)
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            NumberOfArgumentsToSkip = 0
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.True(a);
        Assert.True(b);
        Assert.True(c);
        Assert.True(d);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void Parse_SkipFewFirstArguments_ArgumentsSkipped()
    {
        var arguments = new string[]
        {
            "-a",
            "-b",
            "-c",
            "-d"
        };

        bool a = false;
        bool b = false;
        bool c = false;
        bool d = false;

        var options = new ICommonOption[]
        {
            new FlagOption(string.Empty, "a", afterHandlingAction: () => a = true),
            new FlagOption(string.Empty, "b", afterHandlingAction: () => b = true),
            new FlagOption(string.Empty, "c", afterHandlingAction: () => c = true),
            new FlagOption(string.Empty, "d", afterHandlingAction: () => d = true)
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            NumberOfArgumentsToSkip = 3
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.False(a);
        Assert.False(b);
        Assert.False(c);
        Assert.True(d);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void Parse_OptionsWithAssignmentCharacter_HandledCorrectly()
    {
        const double expectedAngle = -5.5;
        const int expectedWidth = 1920;
        const int expectedHeight = 1080;
        const StringSplitOptions expectedSplitOption = StringSplitOptions.RemoveEmptyEntries;

        double angle = default;
        int width = default;
        int height = default;
        StringSplitOptions splitOption = default;

        var expectedExtraArguments = new string[]
        {
            "10",
            "400",
            "100",
            StringSplitOptions.RemoveEmptyEntries.ToString()
        };

        var arguments = new string[]
        {
            $"-a={expectedAngle}",
            expectedExtraArguments[0],
            $"--width={expectedWidth}",
            expectedExtraArguments[1],
            $"/H={expectedHeight}",
            expectedExtraArguments[2],
            $"/split-option={expectedSplitOption}",
            expectedExtraArguments[3]
        };

        var options = new ICommonOption[]
        {
            new ValueOption<double>(
                "angle",
                "a",
                afterValueParsingAction: t => angle = t),

            new ValueOption<int>(
                "width",
                "w",
                afterValueParsingAction: t => width = t),

            new ValueOption<int>(
                "height",
                "H",
                afterValueParsingAction: t => height = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t)
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeSlashOptions = true
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.Equal(expectedAngle, angle);
        Assert.Equal(expectedWidth, width);
        Assert.Equal(expectedHeight, height);
        Assert.Equal(expectedSplitOption, splitOption);

        VerifyExtraArguments(expectedExtraArguments, extraArguments);
    }

    [Fact]
    public void Parse_CounterOption_DuplicatesHandled()
    {
        int verbosityLevel = default;
        const int expectedVerbosityLevel = 5;

        var expectedExtraArguments = new string[]
        {
            "--angle",
            "45"
        };

        var arguments = new string[]
        {
            "-V",
            expectedExtraArguments[0],
            expectedExtraArguments[1],
            "-VVVV"
        };

        var options = new ICommonOption[]
        {
            new CounterOption(string.Empty, "V", increaseCounter: () => verbosityLevel++)
        };

        var parser = new ArgumentParser()
        {
            RecognizeCompoundOptions = true
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.Equal(expectedVerbosityLevel, verbosityLevel);

        VerifyExtraArguments(expectedExtraArguments, extraArguments);
    }

    [Fact]
    public void Parse_HelpOption_OtherArgumentsSkipped()
    {
        bool help = default;
        bool verbose = default;
        double angle = default;
        StringSplitOptions splitOption = default;
        List<string>? files = default;

        var arguments = new string[]
        {
            "-v",
            "--angle", "100.5",
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

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeSlashOptions = true
        };

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
        bool version = default;
        bool verbose = default;
        double angle = default;
        StringSplitOptions splitOption = default;
        List<string>? files = default;

        var arguments = new string[]
        {
            "-v",
            "--angle", "100.5",
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

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeSlashOptions = true
        };

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
        int finalOptionValue = default;
        bool verbose = default;
        double angle = default;
        StringSplitOptions splitOption = default;
        List<string>? files = default;

        const int expectedFinalOptionValue = -900;

        var arguments = new string[]
        {
            "-v",
            "--angle", "100.5",
            "--final", expectedFinalOptionValue.ToString(CultureInfo.CurrentCulture),
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

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            RecognizeSlashOptions = true
        };

        parser.AddOptions(options);
        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.Equal(expectedFinalOptionValue, finalOptionValue);

        Assert.Equal(default, verbose);
        Assert.Equal(default, angle);
        Assert.Equal(default, splitOption);
        Assert.Equal(default, files);

        Assert.Empty(extraArguments);
    }

    [Fact]
    public void Parse_DuplicateArguments_ThrowsException()
    {
        var arguments = new string[]
        {
            "-a", "5",
            "-w", "1920",
            "-a", "10"
        };

        var options = new ICommonOption[]
        {
            new ValueOption<int>(string.Empty, "a"),
            new ValueOption<double>(string.Empty, "w")
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false
        };

        parser.AddOptions(options);

        Assert.Throws<OptionAlreadyHandledException>(() =>
        {
            _ = parser.Parse(arguments);
        });
    }

    [Fact]
    public void Parse_MutuallyExclusiveOptions_ThrowsException()
    {
        var arguments = new string[]
        {
            "--c0", "5",
            "-b", "1920",
            "--c2", "1080",
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
            conflictingOptions[1],
            new ValueOption<int>(string.Empty, "d"),
            conflictingOptions[2]
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false
        };

        parser.AddOptions(options);

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
    public void Parse_SeveralArguments_ArgumentsParseResultIsCorrect()
    {
        const int marginLeft = 15;
        const int marginTop = 10;
        const int marginRight = 5;
        const int marginBottom = 15;

        const int pointX = 30;
        const int pointY = -40;

        const string file1 = "C://path//file1.txt";
        const string file2 = @"D:\path\file2.png";
        const string file3 = "./file3";
        const string file4 = "file4";

        const int width = 500;
        const double opacity = 0.5;
        const double absAngle = 180;
        const double angle = -153.123;

        const BindMode bindMode = BindMode.TwoWay;
        const StringSplitOptions splitOption = StringSplitOptions.RemoveEmptyEntries;

        const int counterOptionValuesCount = 5;
        string counterOptionValue = new('V', counterOptionValuesCount);

        var extraArguments = new string[]
        {
            "height",
            "900",
            "24",
            "/h",
            "-125",
            "None",
            "--L",
            "0.9"
        };

        var arguments = new string[]
        {
            "myapp",
            "rebase",
            extraArguments[0],
            extraArguments[1],
            "/m", $"{marginLeft}", $"{marginTop}", $"{marginRight}", $"{marginBottom}",
            extraArguments[2],
            "--point", $"({pointX};{pointY})",
            "-f", file1, file2, file3, file4,
            extraArguments[3],
            "-a", $"{angle}",
            "-lr",
            "--width", $"{width}",
            "-q",
            $"-{counterOptionValue}",
            extraArguments[4],
            "/split-option", $"{splitOption}",
            extraArguments[5],
            extraArguments[6],
            "--abs-angle", $"-{absAngle}",
            $"--opacity={opacity}",
            $"-bind={bindMode}",
            extraArguments[7]
        };

        var optionWithCustomConverter = new ValueOption<double>(
            longName: "abs-angle")
        {
            Converter = new ValueConverter<double>(
                t => Math.Abs(double.Parse(t, CultureInfo.CurrentCulture)))
        };

        var usedOptions = new ICommonOption[]
        {
            optionWithCustomConverter,

            new FlagOption("save_log",  "l"),
            new FlagOption("auto-rotate", "r"),
            new FlagOption("quick-mode", "q"),
            new CounterOption(string.Empty, "V"),
            new EnumValueOption<BindMode>("bind", "b"),
            new EnumValueOption<StringSplitOptions>("split-option", "s"),
            new ValueOption<int>("width", "w"),
            new ValueOption<double>("angle", "a"),
            new ValueOption<double>("opacity", "o"),

            new MultipleValueOption<byte>(
                "margin",
                "m",
                contextCapture: new FixedContextCapture(4)),

            new MultipleValueOption<string>(
                "files",
                "f",
                contextCapture: new ZeroOrMoreContextCapture())
        };

        var notUsedOptions = new ICommonOption[]
        {
            new ValueOption<Point>(string.Empty, "P")
        };

        ICommonOption[] allOptions = [.. usedOptions.Concat(notUsedOptions)];

        var converters = new IValueConverter[]
        {
            new ValueConverter<Point>(t =>
            {
                double[] data = t[1..(t.Length - 1)]
                    .Split(';')
                    .Select(double.Parse)
                    .ToArray();

                return new Point(data[0], data[1]);
            })
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            NumberOfArgumentsToSkip = 2,
            RecognizeCompoundOptions = true,
            RecognizeSlashOptions = true
        };

        parser.AddOptions(allOptions);
        parser.AddConverters(converters);

        ParseArgumentsResult result = parser.ParseKnownArguments(arguments, out _);

        int expectedHandledOptionsCount = usedOptions.Length + counterOptionValuesCount - 1;
        Assert.Equal(expectedHandledOptionsCount, result.HandledOptions.Count);

        foreach (ICommonOption option in usedOptions)
        {
            int foundOptions = result.HandledOptions.Count(t => t.Equals(option));

            int expectedOptions = option is CounterOption
                ? counterOptionValuesCount
                : 1;

            Assert.Equal(expectedOptions, foundOptions);
        }

        Assert.Empty(result.HandledSubcommands);
    }

    [Fact]
    public void Parse_SeveralUseCases_ArgumentsParsedCorrectly()
    {
        const int marginLeft = 15;
        const int marginTop = 10;
        const int marginRight = 5;
        const int marginBottom = 15;

        const int pointX = 30;
        const int pointY = -40;

        const string file1 = "C://path//file1.txt";
        const string file2 = @"D:\path\file2.png";
        const string file3 = "./file3";
        const string file4 = "file4";

        const int expectedWidth = 500;
        const int expectedVerbosityLevel = 5;
        const double expectedOpacity = 0.5;
        const double expectedAbsAngle = 180;
        const double expectedAngle = -153.123;

        const BindMode expectedBindMode = BindMode.TwoWay;
        const StringSplitOptions expectedSplitOption = StringSplitOptions.RemoveEmptyEntries;

        var expectedMargin = new Margin(
            marginLeft,
            marginTop,
            marginRight,
            marginBottom);

        var expectedPoint = new Point(pointX, pointY);
        string[] expectedFiles = [file1, file2, file3, file4];

        bool saveLog = default;
        bool autoRotate = default;
        bool quickMode = default;
        BindMode bindMode = default;
        StringSplitOptions splitOption = default;
        int width = default;
        int verbosityLevel = default;
        double angle = default;
        double absAngle = default;
        double opacity = default;
        Point point = default;
        Margin? margin = null;
        List<string>? files = [];

        var expectedExtraArguments = new string[]
        {
            "height",
            "900",
            "24",
            "/h",
            "-125",
            "None",
            "--L",
            "0.9"
        };

        var arguments = new string[]
        {
            "myapp",
            "rebase",
            expectedExtraArguments[0],
            expectedExtraArguments[1],
            "-V",
            "/m", $"{marginLeft}", $"{marginTop}", $"{marginRight}", $"{marginBottom}",
            expectedExtraArguments[2],
            "--point", $"({pointX};{pointY})",
            "-f", file1, file2, file3, file4,
            expectedExtraArguments[3],
            "-a", $"{expectedAngle}",
            "-lr",
            "--width", $"{expectedWidth}",
            "-q",
            "-VVVV",
            expectedExtraArguments[4],
            "/split-option", $"{expectedSplitOption}",
            expectedExtraArguments[5],
            expectedExtraArguments[6],
            "--abs-angle", $"-{expectedAbsAngle}",
            $"--opacity={expectedOpacity}",
            $"-bind={expectedBindMode}",
            expectedExtraArguments[7]
        };

        var optionWithCustomConverter = new ValueOption<double>(
            longName: "abs-angle",
            afterValueParsingAction: t => absAngle = t)
        {
            Converter = new ValueConverter<double>(
                t => Math.Abs(double.Parse(t, CultureInfo.CurrentCulture)))
        };

        var options = new ICommonOption[]
        {
            optionWithCustomConverter,

            new FlagOption(
                "save_log",
                "l",
                afterHandlingAction: () => saveLog = true),

            new FlagOption(
                "auto-rotate",
                "r",
                afterHandlingAction: () => autoRotate = true),

            new FlagOption(
                "quick-mode",
                "q",
                afterHandlingAction: () => quickMode = true),

            new CounterOption(
                string.Empty,
                "V",
                increaseCounter: () => verbosityLevel++),

            new EnumValueOption<BindMode>(
                "bind",
                "b",
                valueRestriction: new OptionValueRestriction<BindMode>(t => t == BindMode.TwoWay),
                afterValueParsingAction: t => bindMode = t),

            new EnumValueOption<StringSplitOptions>(
                "split-option",
                "s",
                afterValueParsingAction: t => splitOption = t),

            new ValueOption<int>(
                "width",
                "w",
                valueRestriction: new OptionValueRestriction<int>(t => t > 0),
                afterValueParsingAction: t => width = t),

            new ValueOption<double>(
                "angle",
                "a",
                choices: [-153.123, 90],
                afterValueParsingAction: t => angle = t),

            new ValueOption<double>(
                "opacity",
                "o",
                afterValueParsingAction: t => opacity = t),

            new ValueOption<Point>(
                string.Empty,
                "P",
                aliases: ["point"],
                afterValueParsingAction: t => point = t),

            new MultipleValueOption<byte>(
                "margin",
                "m",
                contextCapture: new FixedContextCapture(4),
                choices: [[15, 10, 5, 15]],
                afterValueParsingAction: t => margin = new Margin(t[0], t[1], t[2], t[3])),

            new MultipleValueOption<string>(
                "files",
                "f",
                contextCapture: new ZeroOrMoreContextCapture(),
                afterValueParsingAction: t => files = [..t])
        };

        var converters = new IValueConverter[]
        {
            new ValueConverter<Point>(t =>
            {
                double[] data = t[1..(t.Length - 1)]
                    .Split(';')
                    .Select(double.Parse)
                    .ToArray();

                return new Point(data[0], data[1]);
            })
        };

        var parser = new ArgumentParser()
        {
            UseDefaultHelpOption = false,
            NumberOfArgumentsToSkip = 2,
            RecognizeCompoundOptions = true,
            RecognizeSlashOptions = true
        };

        parser.AddOptions(options);
        parser.AddConverters(converters);

        _ = parser.ParseKnownArguments(arguments, out IList<string> extraArguments);

        Assert.True(saveLog);
        Assert.True(autoRotate);
        Assert.True(quickMode);

        Assert.Equal(expectedSplitOption, splitOption);
        Assert.Equal(expectedWidth, width);
        Assert.Equal(expectedVerbosityLevel, verbosityLevel);
        Assert.Equal(expectedAngle, angle);
        Assert.Equal(expectedAbsAngle, absAngle);
        Assert.Equal(expectedOpacity, opacity);
        Assert.Equal(expectedPoint, point);
        Assert.Equal(expectedMargin, margin);
        Assert.Equal(expectedFiles, files);

        VerifyExtraArguments(expectedExtraArguments, extraArguments);
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
