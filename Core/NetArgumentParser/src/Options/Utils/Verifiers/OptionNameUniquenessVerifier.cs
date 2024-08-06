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

    internal static void VerifyInternalNamesIsUnique(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        if (option.LongName == option.ShortName)
            throw new OnlyUniqueOptionNameException(null, option.LongName);

        if (!string.IsNullOrEmpty(option.LongName) && option.Aliases.Contains(option.LongName))
            throw new OnlyUniqueOptionNameException(null, option.LongName);

        if (!string.IsNullOrEmpty(option.ShortName) && option.Aliases.Contains(option.ShortName))
            throw new OnlyUniqueOptionNameException(null, option.ShortName);
    }

    internal void VerifyNamesIsUnique(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        VerifyInternalNamesIsUnique(option);

        var names = new List<string>(option.Aliases)
        {
            option.LongName,
            option.ShortName
        };

        foreach (string name in names)
        {
            if (_options.Any(t => t.HasName(name)))
                throw new OnlyUniqueOptionNameException(null, name);
        }
    }
}
