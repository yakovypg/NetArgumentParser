using System;

namespace NetArgumentParser.Attributes;

[AttributeUsage(
    AttributeTargets.Property,
    AllowMultiple = true,
    Inherited = false)
]
public class OptionMarkAttribute : Attribute
{
    public OptionMarkAttribute(string name = nameof(OptionMarkAttribute))
    {
        ExtendedArgumentNullException.ThrowIfNull(name, nameof(name));
        Name = name;
    }

    public string Name { get; }
}
