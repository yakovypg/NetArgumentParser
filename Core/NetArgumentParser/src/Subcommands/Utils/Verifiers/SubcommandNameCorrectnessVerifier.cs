using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NetArgumentParser.Subcommands.Utils.Verifiers;

internal static partial class CommandNameCorrectnessVerifier
{
    private static readonly List<string> _reservedNames = [];

    internal static void VerifyNameIsCorrect(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        if (!string.IsNullOrEmpty(name)
            && !_reservedNames.Contains(name)
            && !CorrectNameRegex().IsMatch(name))
        {
            throw new IncorrectSubcommandNameException(null, name);
        }
    }

    [GeneratedRegex(@"^[a-zA-Z]+([a-zA-Z0-9_\-]*[a-zA-Z0-9]+)*$")]
    private static partial Regex CorrectNameRegex();
}
