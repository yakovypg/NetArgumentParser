using System.Collections.Generic;
using NetArgumentParser.Converters;
using NetArgumentParser.Options;

namespace NetArgumentParser.Subcommands;

public interface IOptionSetContainer
{
    IReadOnlyList<ICommonOption> Options { get; }
    IEnumerable<ICommonOption> HiddenOptions { get; }
    IEnumerable<ICommonOption> VisibleOptions { get; }

    void ResetOptionsHandledState(bool recursive = true);
    void AddOptions(params ICommonOption[] options);
    void AddConverters(params IValueConverter[] converters);
    bool RemoveOption(ICommonOption commonOption);
    bool RemoveConverter(IValueConverter converter);

    IList<ICommonOption> GetAllOptions();
}
