using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Converters;

namespace NetArgumentParser.Options.Utils.Verifiers;

internal sealed class ConversionTypeUniquenessVerifier
{
    private readonly IEnumerable<IValueConverter> _converters;

    internal ConversionTypeUniquenessVerifier(IEnumerable<IValueConverter> converters)
    {
        ArgumentNullException.ThrowIfNull(converters, nameof(converters));
        _converters = converters;
    }

    internal void VerifyConversionTypeIsUnique(IValueConverter converter)
    {
        ArgumentNullException.ThrowIfNull(converter, nameof(converter));

        if (_converters.Any(t => t.ConversionType == converter.ConversionType))
            throw new OnlyUniqueConversionTypeException(null, converter.ConversionType);
    }
}
