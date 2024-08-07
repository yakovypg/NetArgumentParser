using System.Collections.Generic;

namespace NetArgumentParser.Options;

public record OptionValue(ICommonOption Option, IReadOnlyCollection<string> Value);
