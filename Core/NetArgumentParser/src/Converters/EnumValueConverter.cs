using System;

namespace NetArgumentParser.Converters;

public class EnumValueConverter<T> : ValueConverter<T>
    where T : struct, Enum
{
#pragma warning disable CA2263 // Prefer generic overload when type is known
    public EnumValueConverter(bool ignoreCase = true)
        : base(t => (T)Enum.Parse(typeof(T), t, ignoreCase)) { }
#pragma warning restore CA2263 // Prefer generic overload when type is known
}
