using System;
using System.Collections.Generic;

namespace NetArgumentParser.Options;

public sealed class OptionGroup<T>
    where T : IOption
{
    private readonly List<T> _options;
    
    internal OptionGroup(string header, IOptionSet<T> optionSet)
    {
        ArgumentNullException.ThrowIfNull(header, nameof(header));
        ArgumentNullException.ThrowIfNull(optionSet, nameof(optionSet));

        _options = [];

        Header = header;
        OptionSet = optionSet;
    }

    public string Header { get; }
    internal IOptionSet<T> OptionSet { get; }

    public IReadOnlyList<T> Options => _options;

    public void AddOptions(params T[] options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        Array.ForEach(options, OptionSet.AddOption);
        Array.ForEach(options, _options.Add);
    }

    public void RemoveOptions(params T[] options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        Array.ForEach(options, t => OptionSet.RemoveOption(t));
        Array.ForEach(options, t => _options.Remove(t));
    }
}
