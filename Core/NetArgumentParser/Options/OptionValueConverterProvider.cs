using System;
using NetArgumentParser.Converters;
using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Options;

public abstract class OptionValueConverterProvider : OptionConfigurationProvider
{
    protected static void AddValueConverter<T>(
        Subcommand subcommand,
        string optionLongName,
        Func<string, T> converter)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));
        ExtendedArgumentNullException.ThrowIfNull(converter, nameof(converter));

        AddValueConverter(subcommand, optionLongName, new ValueConverter<T>(converter));
    }

    protected static void AddValueConverter<T>(
        Subcommand subcommand,
        string optionLongName,
        ValueConverter<T>? converter)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));

        IValueOption<T> foundOption = FindOption<T>(subcommand, optionLongName);
        foundOption.Converter = converter;
    }
}
