using System.Collections.Generic;
using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations.Marks;

[ParserConfig]
internal class OptionMarksParserGeneratorConfig
{
    public const string FlagOptionLongName = "flag";
    public const string CounterOptionLongName = "counter";
    public const string ValueOptionLongName = "value";
    public const string MultipleValueOptionLongName = "multiple";

    [FlagOption(FlagOptionLongName)]
    [FlagOptionMark]
    public bool Flag { get; set; }

    [CounterOption(CounterOptionLongName)]
    public int Counter { get; set; }

    [ValueOption<int>(ValueOptionLongName)]
    [OptionMark(OptionMarkNames.Value)]
    public int Value { get; set; }

    [MultipleValueOption<string>(MultipleValueOptionLongName)]
    [OptionMark(OptionMarkNames.Value)]
    public List<string> Values { get; set; } = [];

    [FlagOption("ignored")]
    [OptionMark("ignored mark")]
    protected int IgnoredByProtectedModifierOption { get; }
}
