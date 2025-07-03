using System.Collections.Generic;
using NetArgumentParser.Options;

namespace NetArgumentParser.Subcommands;

public interface IOptionSetOrganizer : IOptionSetContainer
{
    IReadOnlyList<OptionGroup<ICommonOption>> OptionGroups { get; }
    OptionGroup<ICommonOption> DefaultGroup { get; }

    OptionGroup<ICommonOption> AddOptionGroup(string name, string? description = null);
}
