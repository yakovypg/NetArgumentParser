using System;

namespace NetArgumentParser.Subcommands;

public class Subcommand : ParserQuantum
{
    internal Subcommand(string name, string description)
    {
        ExtendedArgumentNullException.ThrowIfNull(name, nameof(name));
        ExtendedArgumentNullException.ThrowIfNull(description, nameof(description));

        Name = name;
        Description = description;
    }

    public string Name { get; }
    public string Description { get; }
}
