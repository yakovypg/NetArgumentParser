using System;
using NetArgumentParser.Utils;

namespace NetArgumentParser.Attributes;

[AttributeUsage(
    AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = true)
]
public class OptionGroupBaseAttribute : Attribute, IEquatable<OptionGroupBaseAttribute?>
{
    public OptionGroupBaseAttribute(string id, string header, string description)
    {
        ExtendedArgumentNullException.ThrowIfNull(id, nameof(id));
        ExtendedArgumentNullException.ThrowIfNull(header, nameof(header));
        ExtendedArgumentNullException.ThrowIfNull(description, nameof(description));

        Id = id;
        Header = header;
        Description = description;
    }

    public string Id { get; }
    public string Header { get; }
    public string Description { get; }

    public bool Equals(OptionGroupBaseAttribute? other)
    {
        return other is not null && Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as OptionGroupBaseAttribute);
    }

    public override int GetHashCode()
    {
        return HashGenerator.Generate(Id);
    }
}
