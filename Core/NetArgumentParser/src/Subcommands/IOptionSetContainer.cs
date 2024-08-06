using System.Collections.Generic;
using NetArgumentParser.Converters;
using NetArgumentParser.Options;

namespace NetArgumentParser.Subcommands;

public interface IOptionSetContainer
{
    IReadOnlyList<ICommonOption> Options { get; }
    IEnumerable<ICommonOption> HiddenOptions { get; }
    IEnumerable<ICommonOption> VisibleOptions { get; }

    void ResetOptionsHandledState();
    void AddOptions(params ICommonOption[] options);
    void AddConverters(params IValueConverter[] converters);
    bool RemoveOption(ICommonOption option);
    bool RemoveConverter(IValueConverter converter);

    List<ICommonOption> GetAllOptions();
}
