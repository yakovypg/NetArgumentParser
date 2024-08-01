using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NetArgumentParser.Options.Utils.Verifiers;

internal static partial class OptionNameCorrectnessVerifier
{
    private static readonly List<string> _specialNames = [ "?" ];
    
    internal static void VerifyAtLeastOneNameIsDefined(string longName, string shortName)
    {
        ArgumentNullException.ThrowIfNull(longName, nameof(longName));
        ArgumentNullException.ThrowIfNull(shortName, nameof(shortName));

        bool isLongNameEmpty = string.IsNullOrEmpty(longName);
        bool isShortNameEmpty = string.IsNullOrEmpty(shortName);

        if (isLongNameEmpty && isShortNameEmpty)
            throw new IncorrectOptionNameException("At least one name must be defined.", longName);
    }

    internal static void VerifyNameIsCorrect(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        if (!string.IsNullOrEmpty(name)
            && !_specialNames.Contains(name)
            && !CorrectNameRegex().IsMatch(name))
        {
            throw new IncorrectOptionNameException(null, name);
        }
    }

    internal static void VerifyAliasesIsCorrect(IEnumerable<string> aliases)
    {
        ArgumentNullException.ThrowIfNull(aliases, nameof(aliases));
        
        foreach (string alias in aliases)
        {
            VerifyNameIsCorrect(alias);
        }
    }

    [GeneratedRegex(@"^[a-zA-Z]+([a-zA-Z0-9_\-]*[a-zA-Z0-9]+)*$")]
    private static partial Regex CorrectNameRegex();
}
