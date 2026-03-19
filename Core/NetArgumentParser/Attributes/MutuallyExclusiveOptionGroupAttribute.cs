using System;

namespace NetArgumentParser.Attributes;

[AttributeUsage(
    AttributeTargets.Property,
    AllowMultiple = true,
    Inherited = false)
]
public sealed class MutuallyExclusiveOptionGroupAttribute : OptionGroupBaseAttribute
{
    public MutuallyExclusiveOptionGroupAttribute(string id, string header, string description)
        : base(
            id ?? throw new ArgumentNullException(nameof(id)),
            header ?? throw new ArgumentNullException(nameof(header)),
            description ?? throw new ArgumentNullException(nameof(description)))
    {
    }
}
