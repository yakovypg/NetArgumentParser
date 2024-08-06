using NetArgumentParser.Converters;

namespace NetArgumentParser.Options;

public interface IOptionSet<T> : IReadOnlyOptionSet<T>
    where T : IOption
{
    void ResetOptionsHandledState();
    
    void AddOption(T option);
    void AddConverter(IValueConverter converter);
    bool RemoveOption(T option);
    bool RemoveConverter(IValueConverter converter);
}
