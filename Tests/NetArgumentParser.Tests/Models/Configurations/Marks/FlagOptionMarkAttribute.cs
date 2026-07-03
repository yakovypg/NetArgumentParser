using System;
using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations.Marks;

[AttributeUsage(
    AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = true)
]
internal sealed class FlagOptionMarkAttribute : OptionMarkAttribute
{
    internal const string DefaultName = nameof(FlagOptionMarkAttribute);

    internal FlagOptionMarkAttribute()
        : base(DefaultName)
    { }
}
