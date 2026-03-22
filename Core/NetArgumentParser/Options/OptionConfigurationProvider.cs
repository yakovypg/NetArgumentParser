using System;
using System.Collections.Generic;
using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Options;

public abstract class OptionConfigurationProvider : IOptionConfigurationProvider
{
    protected abstract IReadOnlyCollection<Action<Subcommand>> ConfigurationProviders { get; }

    public virtual void ConfigureOptions(Subcommand subcommand)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));

        foreach (Action<Subcommand> configureAction in ConfigurationProviders)
        {
            configureAction.Invoke(subcommand);
        }
    }

    protected static IValueOption<T> FindOption<T>(Subcommand subcommand, string longName)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));
        ExtendedArgumentNullException.ThrowIfNull(longName, nameof(longName));

        bool found = subcommand.FindFirstValueOptionByLongName(longName, false, out IValueOption<T>? foundOption);

        return found && foundOption is not null
            ? foundOption
            : throw new SubcommandConfiguredIncorrectlyException(null, subcommand);
    }
}
