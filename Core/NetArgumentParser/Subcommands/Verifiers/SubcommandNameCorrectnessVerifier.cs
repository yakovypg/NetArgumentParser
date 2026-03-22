using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NetArgumentParser.Subcommands.Verifiers;

internal static partial class CommandNameCorrectnessVerifier
{
    private static readonly List<string> _reservedNames = [];

    internal static void VerifyNameIsCorrect(string name)
    {
        ExtendedArgumentNullException.ThrowIfNull(name, nameof(name));

        var correctNameRegex = new Regex(@"^[a-zA-Z]+([a-zA-Z0-9_\-]*[a-zA-Z0-9]+)*$");

        if (!string.IsNullOrEmpty(name)
            && !_reservedNames.Contains(name)
            && !correctNameRegex.IsMatch(name))
        {
            throw new IncorrectSubcommandNameException(null, name);
        }
    }
}
