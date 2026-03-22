using System;
using System.Collections.Generic;
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
        ParserQuantum parserQuantum,
        string optionLongName,
        Predicate<T> isValueAllowed,
        string? valueNotSatisfuRestrictionMessage = null)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));
        ExtendedArgumentNullException.ThrowIfNull(isValueAllowed, nameof(isValueAllowed));

        AddRestriction(
            parserQuantum,
            optionLongName,
            new OptionValueRestriction<T>(isValueAllowed, valueNotSatisfuRestrictionMessage));
    }

    protected static void AddRestriction<T>(
        ParserQuantum parserQuantum,
        string optionLongName,
        OptionValueRestriction<T>? valueRestriction)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));

        IValueOption<T> foundOption = FindValueOption<T>(parserQuantum, optionLongName);
        foundOption.ValueRestriction = valueRestriction;
    }

    protected static void AddRestrictionToMultipleValueOption<T>(
        ParserQuantum parserQuantum,
        string optionLongName,
        Predicate<IList<T>> isValueAllowed,
        string? valueNotSatisfuRestrictionMessage = null)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));
        ExtendedArgumentNullException.ThrowIfNull(isValueAllowed, nameof(isValueAllowed));

        AddRestriction(
            parserQuantum,
            optionLongName,
            isValueAllowed,
            valueNotSatisfuRestrictionMessage);
    }

    protected static void AddRestrictionToMultipleValueOption<T>(
        ParserQuantum parserQuantum,
        string optionLongName,
        OptionValueRestriction<IList<T>>? valueRestriction)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));

        AddRestriction(parserQuantum, optionLongName, valueRestriction);
    }
}
