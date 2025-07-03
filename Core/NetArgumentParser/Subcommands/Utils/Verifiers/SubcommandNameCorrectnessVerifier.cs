using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NetArgumentParser.Subcommands.Utils.Verifiers;

internal static partial class CommandNameCorrectnessVerifier
{
    private static readonly List<string> _reservedNames = [];

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
            throw new IncorrectSubcommandNameException(null, name);
        }
    }
}
