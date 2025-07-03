using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal class NotValidVersionOptionParserGeneratorConfig
{
    [VersionOption]
    public double? NotValidOption { get; set; }
}
