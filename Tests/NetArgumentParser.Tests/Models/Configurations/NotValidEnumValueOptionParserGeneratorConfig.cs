using System;
using System.IO;
using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal class NotValidEnumValueOptionParserGeneratorConfig
{
    [EnumValueOption<StringSplitOptions>("option", "o")]
    public FileMode NotValidOption { get; set; }
}
