using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NetArgumentParser;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Configuration;
using NetArgumentParser.Options.Context;

var resultValues = new ResultValues();
var parser = new ArgumentParser();

var nameOption = new ValueOption<string>(
    "name",
    "n",
    afterValueParsingAction: t => resultValues.Name = t);

var nickOption = new ValueOption<string>(
    "nick",
    afterValueParsingAction: t => resultValues.Name = t);

var options = new ICommonOption[]
{
    nameOption,
    nickOption,

    new HelpOption(
        longName: "help",
        shortName: "h",
        description: "show command-line help",
        aliases: ["?"],
        afterHandlingAction: () =>
        {
            Console.WriteLine(parser.GenerateProgramDescription());
            Environment.Exit(0);
        }),

    new FlagOption(
        longName: "info",
        description: "get application information",
        isFinal: true,
        afterHandlingAction: () =>
        {
            Console.WriteLine("Information");
            Environment.Exit(0);
        }),

    new FlagOption(
        longName: "verbose",
        shortName: "v",
        description: "be verbose",
        afterHandlingAction: () => resultValues.Verbose = true),

    new FlagOption(
        longName: "legacy-verbose",
        shortName: string.Empty,
        description: "be verbose",
        isHidden: true,
        afterHandlingAction: () => resultValues.Verbose = true),

    new FlagOption(
        longName: string.Empty,
        shortName: "q",
        description: "use fast algorithm",
        aliases: ["quick", "fast"],
        afterHandlingAction: () => resultValues.Quick = true),

    new MultipleValueOption<string>(
        longName: "input",
        shortName: "i",
        description: "images that need to be processed",
        isRequired: true,
        valueRestriction: new OptionValueRestriction<IList<string>>(t => t.All(p => File.Exists(p))),
        contextCapture: new OneOrMoreContextCapture(),
        afterValueParsingAction: t => resultValues.InputFiles = new List<string>(t)),

    new MultipleValueOption<int>(
        longName: "date",
        description: "next date the program update notification will be displayed",
        metaVariable: "D",
        contextCapture: new FixedContextCapture(3),
        afterValueParsingAction: t => resultValues.Date = new DateTime(t[0], t[1], t[2])),

    new ValueOption<int>(
        longName: "angle",
        shortName: "a",
        description: "angle by which you want to rotate the image",
        isRequired: true,
        beforeParseChoices: ["0", "45", "90", "180"],
        choices: [0, 45, 90, 180],
        afterValueParsingAction: t => resultValues.Angle = t),

    new EnumValueOption<FileMode>(
        longName: "file-mode",
        shortName: string.Empty,
        description: "specifies how the operatng system should open a file",
        defaultValue: new DefaultOptionValue<FileMode>(FileMode.OpenOrCreate),
        afterValueParsingAction: t => resultValues.FileMode = t),

    new CounterOption(
        longName: string.Empty,
        shortName: "V",
        description: "increase verbosity level",
        increaseCounter: () => resultValues.VerbosityLevel++)
};

parser.AddOptions(options);

try
{
    _ = parser.Parse(args);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    return;
}

Console.WriteLine();
Console.WriteLine($"Verbose: {resultValues.Verbose}");
Console.WriteLine($"Quick: {resultValues.Quick}");
Console.WriteLine($"Verbosity level: {resultValues.VerbosityLevel}");
Console.WriteLine($"Angle: {resultValues.Angle}");
Console.WriteLine($"File mode: {resultValues.FileMode}");
Console.WriteLine($"Input files: {string.Join(" ", resultValues.InputFiles)}");
Console.WriteLine($"Date: {resultValues.Date?.ToLongDateString()}");
Console.WriteLine($"Name: {resultValues.Name}");

#pragma warning disable
internal sealed class ResultValues
{
    public bool Verbose { get; set; }
    public bool Quick { get; set; }
    public int? Angle { get; set; }
    public int VerbosityLevel { get; set; }
    public FileMode? FileMode { get; set; }
    public List<string> InputFiles { get; set; } = [];
    public DateTime? Date { get; set; }
    public string? Name { get; set; }
}
#pragma warning restore
