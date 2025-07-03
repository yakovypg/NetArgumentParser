using System;
using System.Collections.Generic;
using NetArgumentParser.Converters;

namespace NetArgumentParser.Options;

public interface IReadOnlyOptionSet<T>
    where T : IOption
{
    IReadOnlyList<T> Options { get; }
    IReadOnlyList<IValueConverter> Converters { get; }

    T GetOption(string name);

    bool HasHelpOption();
    bool HasVersionOption();
    bool HasOption(string name);
    bool HasConverter(Type conversionType);
}
