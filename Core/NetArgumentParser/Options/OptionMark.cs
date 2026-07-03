using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Options;

public record OptionMark(string Name, ICommonOption Option, ParserQuantum ParserQuantum);
