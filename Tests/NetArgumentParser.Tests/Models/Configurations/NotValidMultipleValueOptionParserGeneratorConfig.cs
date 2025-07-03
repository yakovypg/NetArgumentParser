using System.Collections.Generic;
using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal class NotValidMultipleValueOptionParserGeneratorConfig
{
    [MultipleValueOption<double>("option", "o")]
    public IEnumerable<double> NotValidOption { get; set; } = [];
}
