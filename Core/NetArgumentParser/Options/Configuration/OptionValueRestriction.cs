using System;

namespace NetArgumentParser.Options.Configuration;

public record OptionValueRestriction<T>(Predicate<T> IsValueAllowed);
