using System;

namespace NetArgumentParser.Converters;

public class EnumValueConverter<T> : ValueConverter<T>
    where T : struct, Enum
{
    public EnumValueConverter(bool ignoreCase = true)
        : base(t => (T)Enum.Parse(typeof(T), t, ignoreCase)) { }
}
