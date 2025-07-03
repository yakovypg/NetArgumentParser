using System;
using System.Collections.Generic;

namespace NetArgumentParser.Options.Utils.Verifiers;

internal static class ReuiredOptionVerifier
{
    internal static void VerifyRequiredOptionIsHandled(ICommonOption option)
    {
        ExtendedArgumentNullException.ThrowIfNull(option, nameof(option));

        if (option.IsRequired && !option.IsHandled)
            throw new RequiredOptionNotSpecifiedException(null, option);
    }

    internal static void VerifyRequiredOptionsIsHandled(IEnumerable<ICommonOption> options)
    {
        ExtendedArgumentNullException.ThrowIfNull(options, nameof(options));

        foreach (ICommonOption option in options)
        {
            if (option.IsRequired)
                VerifyRequiredOptionIsHandled(option);
        }
    }
}
