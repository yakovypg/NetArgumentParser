using System;
using System.Collections.Generic;
using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Options;

public abstract class OptionConfigurationProvider : IOptionConfigurationProvider
{
    protected abstract IReadOnlyCollection<Action<ParserQuantum>> ConfigurationProviders { get; }

    public virtual void ConfigureOptions(ParserQuantum subcommand)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));

        foreach (Action<ParserQuantum> configureAction in ConfigurationProviders)
        {
            configureAction.Invoke(subcommand);
        }
    }

    protected static IValueOption<T> FindValueOption<T>(ParserQuantum parserQuantum, string optionLongName)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));

        bool found = parserQuantum.FindFirstValueOptionByLongName(
            optionLongName,
            false,
            out IValueOption<T>? foundOption);

        return found && foundOption is not null
            ? foundOption
            : throw new ParserQuantumConfiguredIncorrectlyException(null, parserQuantum.Name);
    }

    protected static IValueOption<IList<T>> FindMultipleValueOption<T>(ParserQuantum parserQuantum, string optionLongName)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionLongName, nameof(optionLongName));

        return FindValueOption<IList<T>>(parserQuantum, optionLongName);
    }
}
