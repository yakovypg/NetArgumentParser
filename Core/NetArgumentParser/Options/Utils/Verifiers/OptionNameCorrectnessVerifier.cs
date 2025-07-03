using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NetArgumentParser.Options.Utils.Verifiers;

internal static partial class OptionNameCorrectnessVerifier
{
    private static readonly List<string> _reservedNames = ["?"];

    internal static void VerifyAtLeastOneNameIsDefined(string longName, string shortName)
    {
        ExtendedArgumentNullException.ThrowIfNull(longName, nameof(longName));
        ExtendedArgumentNullException.ThrowIfNull(shortName, nameof(shortName));

        bool isLongNameEmpty = string.IsNullOrEmpty(longName);
        bool isShortNameEmpty = string.IsNullOrEmpty(shortName);

        if (isLongNameEmpty && isShortNameEmpty)
            throw new IncorrectOptionNameException("At least one name must be defined.", longName);
    }

    internal static void VerifyNameIsCorrect(string name)
    {
        ExtendedArgumentNullException.ThrowIfNull(name, nameof(name));

#pragma warning disable SYSLIB1045 // Use GeneratedRegexAttribute to generate the regular expression implementation at compile time
        var correctNameRegex = new Regex(@"^[a-zA-Z]+([a-zA-Z0-9_\-]*[a-zA-Z0-9]+)*$");
#pragma warning restore SYSLIB1045 // Use GeneratedRegexAttribute to generate the regular expression implementation at compile time

        if (!string.IsNullOrEmpty(name)
            && !_reservedNames.Contains(name)
            && !correctNameRegex.IsMatch(name))
        {
            throw new IncorrectOptionNameException(null, name);
        }
    }

    internal static void VerifyAliasesIsCorrect(IEnumerable<string> aliases)
    {
        ExtendedArgumentNullException.ThrowIfNull(aliases, nameof(aliases));

        foreach (string alias in aliases)
        {
            VerifyNameIsCorrect(alias);
        }
    }
}
