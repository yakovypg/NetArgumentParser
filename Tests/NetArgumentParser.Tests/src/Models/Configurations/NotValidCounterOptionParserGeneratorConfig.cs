using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal class NotValidCounterOptionParserGeneratorConfig
{
    [CounterOption("option", "o")]
    public string? NotValidOption { get; set; }
}
