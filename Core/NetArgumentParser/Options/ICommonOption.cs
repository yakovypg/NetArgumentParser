using System.Collections.Generic;
using NetArgumentParser.Options.Context;

namespace NetArgumentParser.Options;

public interface ICommonOption : IOption
{
    IReadOnlyCollection<string> Aliases { get; }
    IContextCapture ContextCapture { get; }
    bool IsHandled { get; }

    string GetPrefferedName();
    string GetShortExample();
    string GetLongExample();

    bool HasName(string name);

    void ResetHandledState();
    void Handle(params string[] value);
}
