using System;
using System.Collections.Generic;
using NetArgumentParser.Converters;
using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Options;

public abstract class OptionValueConverterProvider : OptionConfigurationProvider
{
    protected static void AddValueConverter<T>(
        ParserQuantum parserQuantum,
        string optionLongName,
        Func<string, T> converter)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));
        ExtendedArgumentNullException.ThrowIfNull(converter, nameof(converter));

        AddValueConverter(parserQuantum, optionLongName, new ValueConverter<T>(converter));
    }

    protected static void AddValueConverter<T>(
        ParserQuantum parserQuantum,
        string optionLongName,
        ValueConverter<T>? converter)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));

        IValueOption<T> foundOption = FindValueOption<T>(parserQuantum, optionLongName);
        foundOption.Converter = converter;
    }

    protected static void AddValueConverterToMultipleValueOption<T>(
        ParserQuantum parserQuantum,
        string optionLongName,
        Func<string, IList<T>> converter)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));
        ExtendedArgumentNullException.ThrowIfNull(converter, nameof(converter));

        AddValueConverter(parserQuantum, optionLongName, converter);
    }

    protected static void AddValueConverterToMultipleValueOption<T>(
        ParserQuantum parserQuantum,
        string optionLongName,
        ValueConverter<IList<T>>? converter)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));

        AddValueConverter(parserQuantum, optionLongName, converter);
    }
}
