using System;
using System.Globalization;
using NetArgumentParser;
using NetArgumentParser.Converters;
using NetArgumentParser.Options;

var toUpperStringConverter = new ValueConverter<string>(
    t => t.ToUpper(CultureInfo.CurrentCulture));

var toLowerStringConverter = new ValueConverter<string>(
    t => t.ToLower(CultureInfo.CurrentCulture));

string firstName = string.Empty;
string secondName = string.Empty;

var firstNameOption = new ValueOption<string>(
    "first-name",
    afterValueParsingAction: t => firstName = t);

var secondNameOption = new ValueOption<string>(
    "second-name",
    afterValueParsingAction: t => secondName = t)
{
    Converter = toLowerStringConverter
};

var parser = new ArgumentParser();

parser.AddOptions(firstNameOption, secondNameOption);
parser.AddConverters(toUpperStringConverter);

parser.Parse(["--first-name", "Name", "--second-name", "Name"]);

Console.WriteLine(firstName);   // NAME
Console.WriteLine(secondName);  // name
