using System;
using System.Collections.Generic;
using NetArgumentParser.Converters;

namespace NetArgumentParser.Options;

public interface IOptionSet<T>
    where T : IOption
{
    IReadOnlyList<T> Options { get; }
    IReadOnlyList<IValueConverter> Converters { get; }

    T GetOption(string name);

    bool HasHelpOption();
    bool HasVersionOption();
    bool HasOption(string name);
    bool HasConverter(Type conversionType);

    void ResetOptionsHandledState();
    
    void AddOption(T option);
    void AddConverter(IValueConverter converter);
    bool RemoveOption(T option);
    bool RemoveConverter(IValueConverter converter);
}
