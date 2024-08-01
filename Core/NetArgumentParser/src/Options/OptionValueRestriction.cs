using System;

namespace NetArgumentParser.Options;

public record OptionValueRestriction<T>(Predicate<T> IsValueAllowed);