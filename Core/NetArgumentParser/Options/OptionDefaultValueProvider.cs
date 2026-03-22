using NetArgumentParser.Options.Configuration;
using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Options;

public abstract class OptionDefaultValueProvider : OptionConfigurationProvider
{
    protected static void AddDefaultValue<T>(
        Subcommand subcommand,
        string optionLongName,
        T defaultValue)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));

        AddDefaultValue(subcommand, optionLongName, new DefaultOptionValue<T>(defaultValue));
    }

    protected static void AddDefaultValue<T>(
        Subcommand subcommand,
        string optionLongName,
        DefaultOptionValue<T> defaultValue)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));
        ExtendedArgumentNullException.ThrowIfNull(defaultValue, nameof(defaultValue));

        IValueOption<T> foundOption = FindOption<T>(subcommand, optionLongName);
        foundOption.DefaultValue = defaultValue;
    }
}
