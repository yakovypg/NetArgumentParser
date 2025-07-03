using System;
using System.Collections.Generic;

namespace NetArgumentParser.Options;

public sealed class MutuallyExclusiveOptionGroup<T> : IOptionGroup<T>
    where T : IOption
{
    private readonly List<T> _options;
    private string _header;
    private string _description;

    internal MutuallyExclusiveOptionGroup(
        string header,
        string description,
        IEnumerable<T>? options = null)
    {
        _header = header;
        _description = description;

        _options = options is not null
            ? [.. options]
            : [];
    }

    public IReadOnlyList<T> Options => _options;

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

    public void AddOptions(params T[] options)
    {
        ExtendedArgumentNullException.ThrowIfNull(options, nameof(options));
        Array.ForEach(options, _options.Add);
    }

    public bool RemoveOption(T item)
    {
        ExtendedArgumentNullException.ThrowIfNull(item, nameof(item));
        return _options.Remove(item);
    }
}
