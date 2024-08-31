using System;

namespace NetArgumentParser.Options;

public class OptionValueEventArgs<T> : EventArgs
{
    public OptionValueEventArgs(T value)
    {
        ExtendedArgumentNullException.ThrowIfNull(value, nameof(value));
        Value = value;
    }

    public T Value { get; }
}
