using System;
using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations.Marks;

[ParserConfig]
internal class ComplexOptionMarksParserGeneratorConfig
{
    public const string FlagOptionLongName = "flag";
    public const string EnumOptionLongName = "enum";
    public const string ValueOptionLongName = "value";
    public const string MultipleValueOptionLongName = "multiple";

    public const string MarksSubcommandName = "marks";
    public const string MarksSubcommandDescription = "marks description";

    public ComplexOptionMarksParserGeneratorConfig()
    {
        Marks = new OptionMarksParserGeneratorConfig();
    }

    [FlagOption(FlagOptionLongName)]
    public bool Flag { get; set; }

    [EnumValueOption<StringSplitOptions>(EnumOptionLongName)]
    [OptionMark(OptionMarkNames.Enum)]
    public StringSplitOptions Enum { get; set; }

    [ValueOption<int>(ValueOptionLongName)]
    [OptionMark(OptionMarkNames.Value)]
    public int Value { get; set; }

    [Subcommand(MarksSubcommandName, MarksSubcommandDescription)]
    public OptionMarksParserGeneratorConfig Marks { get; }

    [Subcommand("ignored", "i")]
    protected int IgnoredByProtectedModifierSubcommand { get; }
}
