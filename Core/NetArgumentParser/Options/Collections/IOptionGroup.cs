using System.Collections.Generic;

namespace NetArgumentParser.Options.Collections;

public interface IOptionGroup<T>
    where T : IOption
{
    string Header { get; set; }
    string Description { get; set; }
    IReadOnlyList<T> Options { get; }

    void AddOptions(params T[] options);
    bool RemoveOption(T item);
}
