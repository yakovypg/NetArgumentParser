using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal class NotValidFlagOptionParserGeneratorConfig
{
    [FlagOption("option", "o")]
    public int NotValidOption { get; set; }
}
