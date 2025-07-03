using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal class NotValidValueOptionParserGeneratorConfig
{
    [ValueOption<int>("option", "o")]
    public double NotValidOption { get; set; }
}
