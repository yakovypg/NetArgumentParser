using System.Collections.Generic;
using NetArgumentParser.Options.Configuration;
using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Options;

public abstract class OptionDefaultValueProvider : OptionConfigurationProvider
{
    protected static void AddDefaultValue<T>(
        ParserQuantum parserQuantum,
        string optionLongName,
        T defaultValue)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));

        AddDefaultValue(parserQuantum, optionLongName, new DefaultOptionValue<T>(defaultValue));
    }

    protected static void AddDefaultValue<T>(
        ParserQuantum parserQuantum,
        string optionLongName,
        DefaultOptionValue<T> defaultValue)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));
        ExtendedArgumentNullException.ThrowIfNull(defaultValue, nameof(defaultValue));

        IValueOption<T> foundOption = FindValueOption<T>(parserQuantum, optionLongName);
        foundOption.DefaultValue = defaultValue;
    }

    protected static void AddDefaultValueToMultipleValueOption<T>(
        ParserQuantum parserQuantum,
        string optionLongName,
        IList<T> defaultValue)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));

        AddDefaultValue(parserQuantum, optionLongName, defaultValue);
    }

    protected static void AddDefaultValueToMultipleValueOption<T>(
        ParserQuantum parserQuantum,
        string optionLongName,
        DefaultOptionValue<IList<T>> defaultValue)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));
        ExtendedArgumentNullException.ThrowIfNull(defaultValue, nameof(defaultValue));

        AddDefaultValue(parserQuantum, optionLongName, defaultValue);
    }
}
