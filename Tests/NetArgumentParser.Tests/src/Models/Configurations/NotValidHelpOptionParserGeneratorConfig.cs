using System.Collections.Generic;
using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal class NotValidHelpOptionParserGeneratorConfig
{
    [HelpOption]
    public List<int>? NotValidOption { get; set; }
}
