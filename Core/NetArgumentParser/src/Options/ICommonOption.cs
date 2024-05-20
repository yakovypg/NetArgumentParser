using NetArgumentParser.Options.Context;

namespace NetArgumentParser.Options;

public interface ICommonOption : IOption
{
    IContextCapture ContextCapture { get; }
    bool IsHandled { get; }

    string GetPrefferedName();
    string GetShortExample();
    string GetLongExample();

    void ResetHandledState();
    void Handle(params string[] value);
}
