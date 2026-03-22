using System;
using NetArgumentParser.Options.Configuration;
using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Options;

public abstract class OptionRestrictionProvider : OptionConfigurationProvider
{
    protected static string CreateValueNotSatisfuRestrictionMessage(string optionName, string? reason = null)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionName, nameof(optionName));

        if (!string.IsNullOrEmpty(reason))
            reason = $": {reason}";

        return $"Value of '{optionName}' is incorrect{reason}.";
    }

    protected static void AddRestriction<T>(
        Subcommand subcommand,
        string optionLongName,
        Predicate<T> isValueAllowed,
        string? valueNotSatisfuRestrictionMessage = null)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));
        ExtendedArgumentNullException.ThrowIfNull(isValueAllowed, nameof(isValueAllowed));

        AddRestriction(
            subcommand,
            optionLongName,
            new OptionValueRestriction<T>(isValueAllowed, valueNotSatisfuRestrictionMessage));
    }

    protected static void AddRestriction<T>(
        Subcommand subcommand,
        string optionLongName,
        OptionValueRestriction<T>? valueRestriction)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));

        IValueOption<T> foundOption = FindOption<T>(subcommand, optionLongName);
        foundOption.ValueRestriction = valueRestriction;
    }
}
