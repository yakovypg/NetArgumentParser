using System;
using System.Collections.Generic;

namespace NetArgumentParser.Options;

public sealed class MutuallyExclusiveOptionGroup<T>
    where T : IOption
{
    private readonly List<T> _options;

    internal MutuallyExclusiveOptionGroup(IEnumerable<T>? options = null)
    {
        _options = options is not null
            ? [.. options]
            : [];
    }

    public IReadOnlyList<T> Options => _options;

    public void AddOptions(params T[] options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        Array.ForEach(options, _options.Add);
    }

    public bool RemoveOption(T option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));
        return _options.Remove(option);
    }
}
