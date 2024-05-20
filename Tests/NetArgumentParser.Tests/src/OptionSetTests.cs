using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Converters;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Context;
using NetArgumentParser.Tests.Models;

namespace NetArgumentParser.Tests;

public class OptionSetTests
{
    [Fact]
    public void HasOption_ExistingOption_ReturnsTrue()
    {
        var options = new ICommonOption[]
        {
            new FlagOption("save_log", "l"),
            new FlagOption("auto-rotate", "r"),
            new EnumValueOption<StringSplitOptions>("split-options", "s"),
            new ValueOption<int>("width", "w"),
            new ValueOption<double>("angle", "a"),
            new MultipleValueOption<byte>("margin", "m", contextCapture: new FixedContextCapture(4)),
            new MultipleValueOption<string>("files", "f", contextCapture: new ZeroOrMoreContextCapture())
        };
        
        var optionSet = new OptionSet(options);

        foreach (ICommonOption option in options)
        {
            if (!string.IsNullOrEmpty(option.LongName))
                Assert.True(optionSet.HasOption(option.LongName));
            
            if (!string.IsNullOrEmpty(option.ShortName))
                Assert.True(optionSet.HasOption(option.ShortName));
        }
    }

    [Fact]
    public void HasOption_NotExistingOption_ReturnsFalse()
    {
        var options = new ICommonOption[]
        {
            new FlagOption("save_log", "l"),
            new FlagOption("auto-rotate", "r"),
            new EnumValueOption<StringSplitOptions>("split-options", "s"),
            new ValueOption<int>("width", "w"),
            new ValueOption<double>("angle", "a"),
            new MultipleValueOption<byte>("margin", "m", contextCapture: new FixedContextCapture(4)),
            new MultipleValueOption<string>("files", "f", contextCapture: new ZeroOrMoreContextCapture())
        };
        
        var optionSet = new OptionSet(options);

        Assert.False(optionSet.HasOption("q"));
        Assert.False(optionSet.HasOption("W"));
        Assert.False(optionSet.HasOption("Width"));
        Assert.False(optionSet.HasOption("widtH"));
        Assert.False(optionSet.HasOption("height"));
    }

    [Fact]
    public void HasConverter_ExistingConverter_ReturnsTrue()
    {
        var converters = new IValueConverter[]
        {
            new ValueConverter<Margin>(_ => new Margin(0, 0, 0, 0)),
            new ValueConverter<Point>(_ => new Point()),
            new ValueConverter<int>(_ => 0),
            new ValueConverter<double>(_ => 0),
            new ValueConverter<string>(_ => string.Empty),
            new MultipleValueConverter<int>(_ => 0),
            new EnumValueConverter<StringSplitOptions>(),
        };

        var optionSet = new OptionSet(null, converters);

        foreach (IValueConverter converter in converters)
        {
            Assert.True(optionSet.HasConverter(converter.ConversionType));
        }
    }

    [Fact]
    public void HasConverter_NotExistingConverter_ReturnsFalse()
    {
        var converters = new IValueConverter[]
        {
            new ValueConverter<Margin>(_ => new Margin(0, 0, 0, 0)),
            new ValueConverter<Point>(_ => new Point()),
            new ValueConverter<int>(_ => 0),
            new ValueConverter<double>(_ => 0),
            new ValueConverter<string>(_ => string.Empty),
            new MultipleValueConverter<int>(_ => 0),
            new EnumValueConverter<StringSplitOptions>(),
        };

        var optionSet = new OptionSet(null, converters);

        Assert.False(optionSet.HasConverter(typeof(ValueConverter<float>)));
        Assert.False(optionSet.HasConverter(typeof(ValueConverter<short>)));
        Assert.False(optionSet.HasConverter(typeof(ValueConverter<double[]>)));
        Assert.False(optionSet.HasConverter(typeof(ValueConverter<List<string>>)));
        Assert.False(optionSet.HasConverter(typeof(ValueConverter<IList<int>>)));
        Assert.False(optionSet.HasConverter(typeof(MultipleValueConverter<double>)));
        Assert.False(optionSet.HasConverter(typeof(MultipleValueConverter<int[]>)));
        Assert.False(optionSet.HasConverter(typeof(EnumValueConverter<BindMode>)));
        Assert.False(optionSet.HasConverter(typeof(EnumValueConverter<AttributeTargets>)));
    }

    [Fact]
    public void ResetOptionsHandledState_OptionSet_AllOptionsAreResetted()
    {
        var saveLogOption = new FlagOption("save_log", "l");
        var autoRotateOption = new FlagOption("auto-rotate", "r");
        var splitOptionsOption = new EnumValueOption<StringSplitOptions>("split-options", "s");
        var widthOption = new ValueOption<int>("width", "w");
        var angleOption = new ValueOption<double>("angle", "a");
        var marginOption = new MultipleValueOption<int>("margin", "m");
        var filesOption = new MultipleValueOption<string>("files", "f");
        
        var options = new ICommonOption[]
        {
            saveLogOption,
            autoRotateOption,
            splitOptionsOption,
            widthOption,
            angleOption,
            marginOption,
            filesOption
        };

        saveLogOption.Handle();
        autoRotateOption.Handle();
        splitOptionsOption.Handle(StringSplitOptions.TrimEntries.ToString());
        widthOption.Handle("100");
        angleOption.Handle("-15.4");
        marginOption.Handle("10", "20", "30", "-40");
        filesOption.Handle("/file1.txt", "C://path//file2.png", "./file3");
        
        var optionSet = new OptionSet(options);

        foreach (ICommonOption option in options)
        {
            Assert.True(option.IsHandled);
        }

        optionSet.ResetOptionsHandledState();

        foreach (ICommonOption option in options)
        {
            Assert.False(option.IsHandled);
        }
    }

    [Fact]
    public void AddOption_OptionsWithSameName_ThrowsException()
    {
        var options = new ICommonOption[]
        {
            new FlagOption("save_log", "l"),
            new FlagOption("auto-rotate", "r"),
            new EnumValueOption<StringSplitOptions>("split-options", "s"),
            new ValueOption<int>("width", "w"),
            new ValueOption<double>("angle", "a"),
            new MultipleValueOption<byte>("margin", "m", contextCapture: new FixedContextCapture(4)),
            new MultipleValueOption<string>("files", "f", contextCapture: new ZeroOrMoreContextCapture())
        };
        
        var optionSet = new OptionSet(options);

        Assert.Throws<OnlyUniqueOptionNameException>(() =>
        {
            optionSet.AddOption(new ValueOption<int>("save_log", "l"));
        });

        Assert.Throws<OnlyUniqueOptionNameException>(() =>
        {
            optionSet.AddOption(new ValueOption<int>("quick-mode", "s"));
        });

        Assert.Throws<OnlyUniqueOptionNameException>(() =>
        {
            optionSet.AddOption(new MultipleValueOption<byte>("margin", "Q"));
        });

        Assert.Throws<OnlyUniqueOptionNameException>(() =>
        {
            optionSet.AddOption(new FlagOption("angle"));
        });

        Assert.Throws<OnlyUniqueOptionNameException>(() =>
        {
            optionSet.AddOption(new FlagOption(string.Empty, "f"));
        });
    }

    [Fact]
    public void AddOption_UniqueOption_OptionAdded()
    {
        var uniqueOptions = new ICommonOption[]
        {
            new FlagOption("save_log", "l"),
            new FlagOption("auto-rotate", "r"),
            new EnumValueOption<StringSplitOptions>("split-options", "s"),
            new ValueOption<int>("width", "w"),
            new ValueOption<double>("angle", "a"),
            new MultipleValueOption<byte>("margin", "m", contextCapture: new FixedContextCapture(4)),
            new MultipleValueOption<string>("files", "f", contextCapture: new ZeroOrMoreContextCapture())
        };
        
        var optionSet = new OptionSet();

        foreach (ICommonOption option in uniqueOptions)
        {
            Exception? ex = Record.Exception(() => optionSet.AddOption(option));
            Assert.Null(ex);

            string optionName = option.GetPrefferedName();
            optionSet.HasOption(optionName);
        }
    }

    [Fact]
    public void AddConverter_ConvertersWithSameConversionType_ThrowsException()
    {
        var converters = new IValueConverter[]
        {
            new ValueConverter<Margin>(_ => new Margin(0, 0, 0, 0)),
            new ValueConverter<Point>(_ => new Point()),
            new ValueConverter<int>(_ => 0),
            new ValueConverter<double>(_ => 0),
            new ValueConverter<string>(_ => string.Empty),
            new MultipleValueConverter<int>(_ => 0),
            new EnumValueConverter<StringSplitOptions>(),
        };

        var optionSet = new OptionSet(null, converters);

        Assert.Throws<OnlyUniqueConversionTypeException>(() =>
        {
            optionSet.AddConverter(new ValueConverter<Margin>(_ => new Margin(0, 0, 0, 0)));
        });

        Assert.Throws<OnlyUniqueConversionTypeException>(() =>
        {
            optionSet.AddConverter(new ValueConverter<Point>(_ => new Point(1, 1)));
        });

        Assert.Throws<OnlyUniqueConversionTypeException>(() =>
        {
            optionSet.AddConverter(new ValueConverter<int>(_ => 0));
        });

        Assert.Throws<OnlyUniqueConversionTypeException>(() =>
        {
            optionSet.AddConverter(new ValueConverter<double>(_ => 1));
        });

        Assert.Throws<OnlyUniqueConversionTypeException>(() =>
        {
            optionSet.AddConverter(new ValueConverter<string>(_ => string.Empty));
        });

        Assert.Throws<OnlyUniqueConversionTypeException>(() =>
        {
            optionSet.AddConverter(new MultipleValueConverter<int>(_ => 0));
        });

        Assert.Throws<OnlyUniqueConversionTypeException>(() =>
        {
            optionSet.AddConverter(new EnumValueConverter<StringSplitOptions>());
        });
    }

    [Fact]
    public void AddConverter_UniqueConverter_ConverterAdded()
    {
        var uniqueConverters = new IValueConverter[]
        {
            new ValueConverter<Margin>(_ => new Margin(0, 0, 0, 0)),
            new ValueConverter<Point>(_ => new Point()),
            new ValueConverter<int>(_ => 0),
            new ValueConverter<double>(_ => 0),
            new ValueConverter<string>(_ => string.Empty),
            new MultipleValueConverter<int>(_ => 0),
            new EnumValueConverter<StringSplitOptions>(),
        };

        var optionSet = new OptionSet();

        foreach (IValueConverter converter in uniqueConverters)
        {
            Exception? ex = Record.Exception(() => optionSet.AddConverter(converter));
            Assert.Null(ex);

            optionSet.HasConverter(converter.GetType());
        }
    }

    [Fact]
    public void RemoveOption_ExistingOption_OptionRemoved()
    {
        var options = new ICommonOption[]
        {
            new FlagOption("save_log", "l"),
            new FlagOption("auto-rotate", "r"),
            new EnumValueOption<StringSplitOptions>("split-options", "s"),
            new ValueOption<int>("width", "w"),
            new ValueOption<double>("angle", "a"),
            new MultipleValueOption<byte>("margin", "m", contextCapture: new FixedContextCapture(4)),
            new MultipleValueOption<string>("files", "f", contextCapture: new ZeroOrMoreContextCapture())
        };
        
        var optionSet = new OptionSet(options);

        foreach (ICommonOption option in options)
        {
            Assert.True(optionSet.RemoveOption(option));
            Assert.False(optionSet.HasOption(option.LongName));
            Assert.False(optionSet.HasOption(option.ShortName));
        }
    }

    [Fact]
    public void RemoveOption_NotExistingOption_ReturnsFalse()
    {
        var options = new ICommonOption[]
        {
            new FlagOption("save_log", "l"),
            new FlagOption("auto-rotate", "r"),
            new EnumValueOption<StringSplitOptions>("split-options", "s"),
            new ValueOption<int>("width", "w"),
            new ValueOption<double>("angle", "a"),
            new MultipleValueOption<byte>("margin", "m", contextCapture: new FixedContextCapture(4)),
            new MultipleValueOption<string>("files", "f", contextCapture: new ZeroOrMoreContextCapture())
        };
        
        var optionSet = new OptionSet(options);

        Assert.False(optionSet.RemoveOption(new FlagOption("quick", "q")));
        Assert.False(optionSet.RemoveOption(new ValueOption<int>("Width", "W")));
        Assert.False(optionSet.RemoveOption(new ValueOption<double>("width", "w")));
        Assert.False(optionSet.RemoveOption(new ValueOption<int>(string.Empty, "W")));
        Assert.False(optionSet.RemoveOption(new EnumValueOption<StringSplitOptions>("split-options", "S")));
        Assert.False(optionSet.RemoveOption(new EnumValueOption<BindMode>("bind")));
        Assert.False(optionSet.RemoveOption(new MultipleValueOption<string>("input")));
    }

    [Fact]
    public void RemoveConverter_ExistingConverter_ConverterRemoved()
    {
        var converters = new IValueConverter[]
        {
            new ValueConverter<Margin>(_ => new Margin(0, 0, 0, 0)),
            new ValueConverter<Point>(_ => new Point()),
            new ValueConverter<int>(_ => 0),
            new ValueConverter<double>(_ => 0),
            new ValueConverter<string>(_ => string.Empty),
            new MultipleValueConverter<int>(_ => 0),
            new EnumValueConverter<StringSplitOptions>(),
        };

        var optionSet = new OptionSet(null, converters);

        foreach (IValueConverter converter in converters)
        {
            Assert.True(optionSet.RemoveConverter(converter));
            Assert.False(optionSet.HasConverter(converter.ConversionType));
        }
    }

    [Fact]
    public void RemoveConverter_NotExistingConverter_ReturnsFalse()
    {
        var converters = new IValueConverter[]
        {
            new ValueConverter<Margin>(_ => new Margin(0, 0, 0, 0)),
            new ValueConverter<Point>(_ => new Point()),
            new ValueConverter<int>(_ => 0),
            new ValueConverter<double>(_ => 0),
            new ValueConverter<string>(_ => string.Empty),
            new MultipleValueConverter<int>(_ => 0),
            new EnumValueConverter<StringSplitOptions>(),
        };

        var optionSet = new OptionSet(null, converters);

        Assert.False(optionSet.RemoveConverter(new ValueConverter<float>(_ => 0)));
        Assert.False(optionSet.RemoveConverter(new ValueConverter<short>(_ => 0)));
        Assert.False(optionSet.RemoveConverter(new ValueConverter<double[]>(_ => [])));
        Assert.False(optionSet.RemoveConverter(new ValueConverter<List<string>>(_ => [])));
        Assert.False(optionSet.RemoveConverter(new ValueConverter<IList<int>>(_ => [])));
        Assert.False(optionSet.RemoveConverter(new MultipleValueConverter<double>(_ => 0)));
        Assert.False(optionSet.RemoveConverter(new MultipleValueConverter<int[]>(_ => [])));
        Assert.False(optionSet.RemoveConverter(new EnumValueConverter<BindMode>(false)));
        Assert.False(optionSet.RemoveConverter(new EnumValueConverter<AttributeTargets>(true)));
    }

    [Fact]
    public void GetOption_NotBuiltOptionSet_ThrowsException()
    {
        var optionSet = new OptionSet(null, null, false);
        Assert.Throws<OptionSetNotBuiltException>(() => optionSet.GetOption("w"));
    }

    [Fact]
    public void GetOption_NotBuiltOptionSetWithAutomaticBuild_GetCorrectOption()
    {
        var options = new ICommonOption[]
        {
            new FlagOption("save_log", "l"),
            new FlagOption("auto-rotate", "r"),
            new EnumValueOption<StringSplitOptions>("split-options", "s"),
            new ValueOption<int>("width", "w"),
            new ValueOption<double>("angle", "a"),
            new MultipleValueOption<byte>("margin", "m", contextCapture: new FixedContextCapture(4)),
            new MultipleValueOption<string>("files", "f", contextCapture: new ZeroOrMoreContextCapture())
        };
        
        var optionSet = new OptionSet(options, null, true);

        foreach (ICommonOption option in options)
        {
            Assert.Equal(option, optionSet.GetOption(option.LongName));
            Assert.Equal(option, optionSet.GetOption(option.ShortName));
        }
    }

    [Fact]
    public void GetOption_BuiltOptionSet_GetCorrectOption()
    {
        var options = new ICommonOption[]
        {
            new FlagOption("save_log", "l"),
            new FlagOption("auto-rotate", "r"),
            new EnumValueOption<StringSplitOptions>("split-options", "s"),
            new ValueOption<int>("width", "w"),
            new ValueOption<double>("angle", "a"),
            new MultipleValueOption<byte>("margin", "m", contextCapture: new FixedContextCapture(4)),
            new MultipleValueOption<string>("files", "f", contextCapture: new ZeroOrMoreContextCapture())
        };
        
        var optionSet = new OptionSet(options, null, false);
        optionSet.Build();

        foreach (ICommonOption option in options)
        {
            Assert.Equal(option, optionSet.GetOption(option.LongName));
            Assert.Equal(option, optionSet.GetOption(option.ShortName));
        }
    }

    [Fact]
    public void Build_BuiltOptionSet_ConvertersSetted()
    {
        var converters = new IValueConverter[]
        {
            new ValueConverter<Margin>(_ => new Margin(0, 0, 0, 0)),
            new ValueConverter<Point>(_ => new Point()),
            new EnumValueConverter<BindMode>(),
            new MultipleValueConverter<byte>(_ => 0),
        };

        var marginOption = new ValueOption<Margin>(string.Empty, "m");
        var pointOption = new ValueOption<Point>(string.Empty, "p");
        var bindOption = new EnumValueOption<BindMode>(string.Empty, "b");
        var marginMultiOption = new MultipleValueOption<byte>(string.Empty, "M");

        var optionsWithConverter = new ICommonOption[]
        {
            marginOption,    
            pointOption,
            bindOption,
            marginMultiOption
        };

        var splitOptionsOption = new EnumValueOption<StringSplitOptions>("split-options", "s");
        var widthOption = new ValueOption<int>("width", "w");
        var angleOption = new ValueOption<double>("angle", "a");
        var filesOption = new MultipleValueOption<string>("files", "f");
        
        var optionsWithoutConverter = new ICommonOption[]
        {
            splitOptionsOption,
            widthOption,
            angleOption,
            filesOption
        };

        IEnumerable<ICommonOption> allOptions = optionsWithConverter
            .Concat(optionsWithoutConverter);
        
        var optionSet = new OptionSet(allOptions, converters, false);
        optionSet.Build();

        Assert.True(marginOption.HasConverter);
        Assert.True(pointOption.HasConverter);
        Assert.True(bindOption.HasConverter);
        Assert.True(marginMultiOption.HasConverter);

        Assert.False(splitOptionsOption.HasConverter);
        Assert.False(widthOption.HasConverter);
        Assert.False(angleOption.HasConverter);
        Assert.False(filesOption.HasConverter);
    }
}
