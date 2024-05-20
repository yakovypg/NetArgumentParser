using System;
using System.Collections.Generic;

namespace NetArgumentParser.Converters;

public class MultipleValueConverter<T> : ValueConverter<IList<T>>
{
    public MultipleValueConverter(Func<string, T> singleValueConverter)
        : base(t => [singleValueConverter(t)]) {}
}
