using System;
using System.Linq;
using NetArgumentParser;
using NetArgumentParser.Attributes;
using NetArgumentParser.Converters;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;

var generator = new ArgumentParserGenerator();
var parser = new ArgumentParser();
var config = new CustomParserConfig();

generator.ConfigureParser(parser, config);

ICommonOption? foundOption = parser
    .FindOptions(t => t.LongName == "date", true)
    .FirstOrDefault();

if (foundOption is ValueOption<DateTime> birthDateOption)
{
    birthDateOption.ValueRestriction = new OptionValueRestriction<DateTime>(t => true);
    birthDateOption.Converter = new ValueConverter<DateTime>(t => default);
    birthDateOption.DefaultValue = new DefaultOptionValue<DateTime>(default);
    birthDateOption.ValueParsed += (_, _) => Console.WriteLine("Parsed");

    birthDateOption.ChangeChoices(new DateTime[] { default });
}

parser.Parse(["--date", "01.01.2025"]);
Console.WriteLine(config.BirthDate); // 1/1/0001 12:00:00AM

#pragma warning disable
[ParserConfig]
internal sealed class CustomParserConfig
{
    [ValueOption<DateTime>("date", "d")]
    public DateTime BirthDate { get; set; }
}
#pragma warning restore
