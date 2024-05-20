using System;

namespace NetArgumentParser.Converters;

public class EnumValueConverter<T> : ValueConverter<T>
    where T : struct, Enum
{
    public EnumValueConverter(bool ignoreCase = true)
        : base(t => Enum.Parse<T>(t, ignoreCase)) {}
}
