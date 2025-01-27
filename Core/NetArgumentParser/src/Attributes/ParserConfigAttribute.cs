using System;

namespace NetArgumentParser.Attributes;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct,
    AllowMultiple = false,
    Inherited = false)
]
public sealed class ParserConfigAttribute : Attribute { }
