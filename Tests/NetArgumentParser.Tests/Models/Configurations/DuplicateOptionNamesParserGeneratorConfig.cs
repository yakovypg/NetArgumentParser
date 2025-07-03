using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal class DuplicateOptionNamesParserGeneratorConfig
{
    [FlagOption("guide", "g")]
    public bool ShowGuide { get; set; }

    [FlagOption("short-guide", "g")]
    public bool ShowShortGuide { get; set; }
}
