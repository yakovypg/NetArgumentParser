using System;
using System.Collections.Generic;
using System.Linq;

namespace NetArgumentParser.Options.Utils.Verifiers;

internal sealed class OptionNameUniquenessVerifier
{
    private readonly IEnumerable<ICommonOption> _options;

    internal OptionNameUniquenessVerifier(IEnumerable<ICommonOption> options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        _options = options;
    }

    internal void VerifyLongNameIsUnique(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        if (!string.IsNullOrEmpty(option.LongName)
            && _options.Any(t => t.LongName == option.LongName))
        {
            throw new OnlyUniqueOptionNameException(null, option.LongName);
        }
    }

    internal void VerifyShortNameIsUnique(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        if (!string.IsNullOrEmpty(option.ShortName)
            &&_options.Any(t => t.ShortName == option.ShortName))
        {
            throw new OnlyUniqueOptionNameException(null, option.ShortName);
        }
    }
}
