using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NetArgumentParser;
using NetArgumentParser.Converters;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Context;

bool verbose = false;
bool quick = false;
int? angle = default;
int verbosityLevel = default;
TimeSpan? time = default;
FileMode? fileMode = default;
List<string> inputFiles = [];
DateTime? date = default;
string? name = default;

var parser = new ArgumentParser()
{
    NumberOfArgumentsToSkip = 0,
    RecognizeCompoundOptions = true,
    RecognizeSlashOptions = true,
    ProgramName = "ProgramName",
    ProgramVersion = "ProgramName v1.0.0",
    ProgramDescription = "What the program does",
    ProgramEpilog = "Text at the bottom"
};

var nameOption = new ValueOption<string>("name", "n", afterValueParsingAction: t => name = t)
{
    Converter = new ValueConverter<string>(t => t.ToUpper())
};

var options = new ICommonOption[]
{
    nameOption,
    
    new HelpOption("help", "h",
        description: "show command-line help",
        afterHandlingAction: () =>
        {
            Console.WriteLine(parser.GenerateProgramDescription());
            Environment.Exit(0);
        }),

    new FlagOption("verbose", "v",
        description: "be verbose",
        afterHandlingAction: () => verbose = true),
    
    new FlagOption("legacy-verbose", string.Empty,
        description: "be verbose",
        isHidden: true,
        afterHandlingAction: () => verbose = true),

    new FlagOption(string.Empty, "q",
        description: "use fast algorithm",
        aliases: ["quick", "fast"],
        afterHandlingAction: () => quick = true),

    new MultipleValueOption<string>("input", "i",
        description: "images that need to be processed",
        isRequired: true,
        valueRestriction: new OptionValueRestriction<IList<string>>(t => t.All(p => File.Exists(p))),
        contextCapture: new OneOrMoreContextCapture(),
        afterValueParsingAction: t => inputFiles = new List<string>(t)),

    new ValueOption<int>("angle", "a",
        description: "angle by which you want to rotate the image",
        isRequired: true,
        choices: [0, 45, 90, 180],
        afterValueParsingAction: t => angle = t),
    
    new CounterOption(string.Empty, "V",
        description: "increase verbosity level",
        increaseCounter: () => verbosityLevel++)
};

var additionalOptions = new ICommonOption[]
{
    new ValueOption<TimeSpan>("time", "t",
        description: "maximum algorithm execution time",
        afterValueParsingAction: t => time = t),

    new EnumValueOption<FileMode>("file-mode", string.Empty,
        description: "specifies how the operatng system should open a file",
        defaultValue: new DefaultOptionValue<FileMode>(FileMode.OpenOrCreate),
        afterValueParsingAction: t => fileMode = t),
    
    new MultipleValueOption<int>("date",
        description: "next date the program update notification will be displayed",
        metaVariable: "D",
        contextCapture: new FixedContextCapture(3),
        afterValueParsingAction: t => date = new DateTime(t[0], t[1], t[2]))
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

var descriptionGenerator = new DescriptionGenerator(parser)
{
    UsageHeader = "Usage: ",
    OptionExamplePrefix = new string(' ', 2),
    DelimiterAfterOptionExample = new string(' ', 2),
    OptionExampleCharsLimit = 30,
    WindowWidth = Console.WindowWidth
};

parser.DescriptionGenerator = descriptionGenerator;

parser.AddOptions(options);
parser.AddConverters(converters);

OptionGroup<ICommonOption> group = parser.AddOptionGroup("Additional options:");
group.AddOptions(additionalOptions);

List<string> extraArguments = [];

try
{
    parser.ParseKnownArguments(args, out extraArguments);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    return;
}

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
