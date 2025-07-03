using System.Collections.Generic;

namespace NetArgumentParser.Options.Configuration;

public record OptionValue(ICommonOption Option, IReadOnlyCollection<string> Value);
