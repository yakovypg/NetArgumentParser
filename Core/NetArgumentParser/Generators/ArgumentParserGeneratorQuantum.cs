using System;
using System.Collections.Generic;
using System.Linq;

namespace NetArgumentParser.Generators;

#pragma warning disable IDE0001 // Name can be simplified
using OptionInfo = System.Collections.Generic.KeyValuePair<System.Reflection.PropertyInfo, NetArgumentParser.Options.ICommonOption>;
#pragma warning restore IDE0001 // Name can be simplified

public class ArgumentParserGeneratorQuantum
{
    public ArgumentParserGeneratorQuantum(
        IEnumerable<OptionInfo> options,
        IEnumerable<ArgumentParserGeneratorQuantum> subcommands)
    {
        ExtendedArgumentNullException.ThrowIfNull(options, nameof(options));
        ExtendedArgumentNullException.ThrowIfNull(subcommands, nameof(subcommands));

        Options = options;
        Subcommands = subcommands;
    }

    public IEnumerable<OptionInfo> Options { get; }
    public IEnumerable<ArgumentParserGeneratorQuantum> Subcommands { get; }

    public IEnumerable<OptionInfo> FindOptions(
        Predicate<OptionInfo> predicate,
        bool recursive = true)
    {
        IEnumerable<OptionInfo> foundOptions = Options.Where(t => predicate(t));

        if (!recursive)
            return foundOptions;

        IEnumerable<OptionInfo> recursiveFoundOptions = Subcommands
            .SelectMany(t => t.FindOptions(predicate, recursive));

        return foundOptions.Concat(recursiveFoundOptions);
    }
}
