using NetArgumentParser.Converters;

namespace NetArgumentParser.Options.Collections;

public interface IOptionSet<T> : IReadOnlyOptionSet<T>
    where T : IOption
{
    void ResetOptionsHandledState();

    void AddOption(T item);
    void AddConverter(IValueConverter converter);
    bool RemoveOption(T item);
    bool RemoveConverter(IValueConverter converter);
}
