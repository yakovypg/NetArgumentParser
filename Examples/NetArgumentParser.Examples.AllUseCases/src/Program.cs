using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NetArgumentParser;
using NetArgumentParser.Converters;
using NetArgumentParser.Generators;
using NetArgumentParser.Informing;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Context;
using NetArgumentParser.Subcommands;

bool verbose = false;
bool quick = false;
int? angle = default;
int verbosityLevel = default;
TimeSpan? time = default;
FileMode? fileMode = default;
List<string> inputFiles = [];
DateTime? date = default;
string? name = default;

int? width = default;
int? height = default;

var parser = new ArgumentParser()
{
    NumberOfArgumentsToSkip = 0,
    RecognizeCompoundOptions = true,
    RecognizeSlashOptions = false,
    ProgramName = "ProgramName",
    ProgramVersion = "ProgramName v1.0.0",
    ProgramDescription = "What the program does",
    ProgramEpilog = "Text at the bottom",
    SubcommandDescriptionGeneratorCreator = t => new SubcommandDescriptionGenerator(t)
};

var nameOption = new ValueOption<string>("name", "n", afterValueParsingAction: t => name = t)
{
    Converter = new ValueConverter<string>(t => t.ToUpper(CultureInfo.CurrentCulture))
};

var options = new ICommonOption[]
{
    nameOption,

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
        longName: "verbose",
        shortName: "v",
        description: "be verbose",
        afterHandlingAction: () => verbose = true),

    new FlagOption(
        longName: "legacy-verbose",
        shortName: string.Empty,
        description: "be verbose",
        isHidden: true,
        afterHandlingAction: () => verbose = true),

    new FlagOption(
        longName: string.Empty,
        shortName: "q",
        description: "use fast algorithm",
        aliases: ["quick", "fast"],
        afterHandlingAction: () => quick = true),

    new MultipleValueOption<string>(
        longName: "input",
        shortName: "i",
        description: "images that need to be processed",
        isRequired: true,
        valueRestriction: new OptionValueRestriction<IList<string>>(t => t.All(p => File.Exists(p))),
        contextCapture: new OneOrMoreContextCapture(),
        afterValueParsingAction: t => inputFiles = new List<string>(t)),

    new ValueOption<int>(
        longName: "angle",
        shortName: "a",
        description: "angle by which you want to rotate the image",
        isRequired: true,
        choices: [0, 45, 90, 180],
        afterValueParsingAction: t => angle = t),

    new CounterOption(
        longName: string.Empty,
        shortName: "V",
        description: "increase verbosity level",
        increaseCounter: () => verbosityLevel++)
};

var additionalOptions = new ICommonOption[]
{
    new ValueOption<TimeSpan>(
        longName: "time",
        shortName: "t",
        description: "maximum algorithm execution time",
        afterValueParsingAction: t => time = t),

    new EnumValueOption<FileMode>(
        longName: "file-mode",
        shortName: string.Empty,
        description: "specifies how the operatng system should open a file",
        defaultValue: new DefaultOptionValue<FileMode>(FileMode.OpenOrCreate),
        afterValueParsingAction: t => fileMode = t),

    new MultipleValueOption<int>(
        longName: "date",
        description: "next date the program update notification will be displayed",
        metaVariable: "D",
        contextCapture: new FixedContextCapture(3),
        afterValueParsingAction: t => date = new DateTime(t[0], t[1], t[2]))
};

var resizeSubcommandOptions = new ICommonOption[]
{
    new ValueOption<int>(
        longName: "width",
        shortName: "w",
        description: "new width of the image",
        afterValueParsingAction: t => width = t),

    new ValueOption<int>(
        longName: "height",
        shortName: "H",
        description: "new height of the image",
        afterValueParsingAction: t => height = t)
};

var converters = new IValueConverter[]
{
    new ValueConverter<TimeSpan>(t =>
    {
        int[] data = t.Split(',').Select(int.Parse).ToArray();

        return data.Length switch
        {
            1 => new TimeSpan(data[0]),
            3 => new TimeSpan(data[0], data[1], data[2]),
            4 => new TimeSpan(data[0], data[1], data[2], data[3]),
            5 => new TimeSpan(data[0], data[1], data[2], data[3], data[4]),
            6 => new TimeSpan(data[0], data[1], data[2], data[3], data[4], data[5]),

            _ => throw new FormatException()
        };
    })
};

var descriptionGenerator = new ApplicationDescriptionGenerator(parser)
{
    UsageHeader = "Usage: ",
    OptionExamplePrefix = new string(' ', 2),
    DelimiterAfterOptionExample = new string(' ', 2),
    SubcommandsHeader = "Subcommands:",
    SubcommandNamePrefix = new string(' ', 2),
    DelimiterAfterSubcommandName = new string(' ', 2),
    OptionExampleCharsLimit = 30,
    WindowWidth = Console.WindowWidth
};

parser.DescriptionGenerator = descriptionGenerator;
parser.ChangeOutputWriter(new ConsoleTextWriter());

parser.AddOptions(options);
parser.AddConverters(converters);

OptionGroup<ICommonOption> group = parser.AddOptionGroup("Additional options:");
group.AddOptions(additionalOptions);

Subcommand resizeSubcommand = parser.AddSubcommand("resize", "resize the image");
resizeSubcommand.UseDefaultHelpOption = true;
resizeSubcommand.AddOptions(resizeSubcommandOptions);

ParseArgumentsResult result;
IList<string> extraArguments;

try
{
    result = parser.ParseKnownArguments(args, out extraArguments);
}
#pragma warning disable CA1031
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    return;
}
#pragma warning restore CA1031

Console.WriteLine($"Handled options: {result.HandledOptions.Count}");
Console.WriteLine($"Handled subcommands: {result.HandledSubcommands.Count}");

if (result.TryGetLastHandledSubcommand(out Subcommand? subcommand))
    Console.WriteLine($"Last handled subcommand: {subcommand?.Name}");

Console.WriteLine();
Console.WriteLine($"Extra arguments: {string.Join(' ', extraArguments)}");
Console.WriteLine($"Verbose: {verbose}");
Console.WriteLine($"Quick: {quick}");
Console.WriteLine($"Verbosity level: {verbosityLevel}");
Console.WriteLine($"Angle: {angle}");
Console.WriteLine($"Time: {time}");
Console.WriteLine($"File mode: {fileMode}");
Console.WriteLine($"Input files: {string.Join(' ', inputFiles)}");
Console.WriteLine($"Date: {date?.ToLongDateString()}");
Console.WriteLine($"Name: {name}");
Console.WriteLine($"Width: {width}");
Console.WriteLine($"Height: {height}");
