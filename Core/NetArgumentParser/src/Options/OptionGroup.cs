using System;
using System.Collections.Generic;
using System.Linq;

namespace NetArgumentParser.Options;

public sealed class OptionGroup<T>
    where T : IOption
{
    private readonly List<T> _options;
    private string _header;
    private string _description;

    internal OptionGroup(string header, string description, IOptionSet<T> optionSet)
    {
        ExtendedArgumentNullException.ThrowIfNull(header, nameof(header));
        ExtendedArgumentNullException.ThrowIfNull(description, nameof(description));
        ExtendedArgumentNullException.ThrowIfNull(optionSet, nameof(optionSet));

        _options = [];
        _header = header;
        _description = description;

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

    public string Description
    {
        get => _description;
        set => _description = value ?? string.Empty;
    }

    internal IOptionSet<T> OptionSet { get; }

    public void AddOptions(params T[] options)
    {
        ExtendedArgumentNullException.ThrowIfNull(options, nameof(options));

        Array.ForEach(options, OptionSet.AddOption);
        Array.ForEach(options, _options.Add);
    }

    public bool RemoveOption(T option)
    {
        ExtendedArgumentNullException.ThrowIfNull(option, nameof(option));

        return OptionSet.RemoveOption(option)
            && _options.Remove(option);
    }
}
