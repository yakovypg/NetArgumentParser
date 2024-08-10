using System;
using System.Collections.Generic;
using System.Linq;

namespace NetArgumentParser.Options;

public sealed class OptionGroup<T>
    where T : IOption
{
    private readonly List<T> _options;
    private string _header = string.Empty;

    internal OptionGroup(string header, IOptionSet<T> optionSet)
    {
        ArgumentNullException.ThrowIfNull(header, nameof(header));
        ArgumentNullException.ThrowIfNull(optionSet, nameof(optionSet));

        _options = [];

        Header = header;
        OptionSet = optionSet;
    }

    public IReadOnlyList<T> Options => _options;
    public IEnumerable<T> HiddenOptions => Options.Where(t => t.IsHidden);
    public IEnumerable<T> VisibleOptions => Options.Where(t => !t.IsHidden);

    public string Header
    {
        get => _header;
        set => _header = value ?? string.Empty;
    }

    internal IOptionSet<T> OptionSet { get; }

    public void AddOptions(params T[] options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        Array.ForEach(options, OptionSet.AddOption);
        Array.ForEach(options, _options.Add);
    }

    public bool RemoveOption(T option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        return OptionSet.RemoveOption(option)
            && _options.Remove(option);
    }
}
