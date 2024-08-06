using System;
using System.Collections;

namespace NetArgumentParser.Options;

public class NotifyOptionSetChangedEventArgs : EventArgs
{
    public NotifyOptionSetChangedEventArgs(
        NotifyOptionSetChangedAction action,
        IList? newItems = null,
        IList? oldItems = null)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        Action = action;
        NewItems = newItems is null ? newItems : ArrayList.ReadOnly(newItems);
        OldItems = oldItems is null ? oldItems : ArrayList.ReadOnly(oldItems);
    }

    public NotifyOptionSetChangedAction Action { get; }
    public IList? NewItems { get; }
    public IList? OldItems { get; }
}
