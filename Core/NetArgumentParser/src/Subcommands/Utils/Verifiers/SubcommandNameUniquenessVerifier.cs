using System;
using System.Collections.Generic;
using System.Linq;

namespace NetArgumentParser.Subcommands.Utils.Verifiers;

internal sealed class SubcommandNameUniquenessVerifier
{
    private readonly IEnumerable<Subcommand> _subcommands;

    internal SubcommandNameUniquenessVerifier(IEnumerable<Subcommand> subcommands)
    {
        ArgumentNullException.ThrowIfNull(subcommands, nameof(subcommands));
        _subcommands = subcommands;
    }

    public void VerifyNameIsUnique(Subcommand subcommand)
    {
        ArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));

        if (_subcommands.Any(t => t.Name == subcommand.Name))
            throw new OnlyUniqueSubcommandNameException(null, subcommand.Name);
    }
}
