using System;

namespace NetArgumentParser.Attributes;

[AttributeUsage(
    AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = false)
]
public sealed class SubcommandAttribute : Attribute
{
    public SubcommandAttribute(string name, string description)
    {
        ExtendedArgumentNullException.ThrowIfNull(name, nameof(name));
        ExtendedArgumentNullException.ThrowIfNull(description, nameof(description));

        Name = name;
        Description = description;
    }

    public string Name { get; }
    public string Description { get; }
}
