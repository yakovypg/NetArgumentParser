using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Options;
using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Informing;

public class ParseArgumentsResult
{
    public ParseArgumentsResult(
        IReadOnlyCollection<ICommonOption> handledOptions,
        IReadOnlyCollection<Subcommand> handledSubcommands)
    {
        ArgumentNullException.ThrowIfNull(handledOptions, nameof(handledOptions));
        ArgumentNullException.ThrowIfNull(handledSubcommands, nameof(handledSubcommands));

        HandledOptions = handledOptions;
        HandledSubcommands = handledSubcommands;
    }

    public IReadOnlyCollection<ICommonOption> HandledOptions { get; }
    public IReadOnlyCollection<Subcommand> HandledSubcommands { get; }

    public bool TryGetLastHandledSubcommand(out Subcommand? subcommand)
    {
        subcommand = HandledSubcommands.LastOrDefault();
        return subcommand is not null;
    }
}
