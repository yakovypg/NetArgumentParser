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
using NetArgumentParser.Options.Collections;
using NetArgumentParser.Options.Configuration;
using NetArgumentParser.Options.Context;
using NetArgumentParser.Subcommands;

var resultValues = new ResultValues();

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

var nameOption = new ValueOption<string>("name", "n", afterValueParsingAction: t => resultValues.Name = t)
{
    Converter = new ValueConverter<string>(t => t.ToUpper(CultureInfo.CurrentCulture))
};

var nickOption = new ValueOption<string>("nick", afterValueParsingAction: t => resultValues.Name = t);

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
        afterValueParsingAction: t => resultValues.InputFiles = [.. t]),

    new MultipleValueOption<string>(
        longName: "persons",
        shortName: string.Empty,
        description: "persons that should be added to organization",
        ignoreCaseInChoices: true,
        ignoreOrderInChoices: true,
        contextCapture: new FixedContextCapture(3),
        choices: [["Max", "Robert", "Tom"], ["David", "John", "Richard"]],
        afterValueParsingAction: t => resultValues.Persons = [.. t]),

    new MultipleValueOption<PageRange>(
        longName: "ranges",
        shortName: string.Empty,
        description: "ranges of pages that should be processed",
        afterValueParsingAction: t => resultValues.PageRanges.AddRange(t)),

    new MultipleValueOption<PageFontSize>(
        longName: "fonts",
        shortName: string.Empty,
        description: "font sizes for pages",
        afterValueParsingAction: t => resultValues.PageFontSizes = [.. t]),

    new ValueOption<int>(
        longName: "angle",
        shortName: "a",
        description: "angle by which you want to rotate the image",
        isRequired: true,
        choices: [0, 45, 90, 180],
        beforeParseChoices: ["0", "45", "90", "180"],
        afterValueParsingAction: t => resultValues.Angle = t),

    new CounterOption(
        longName: string.Empty,
        shortName: "V",
        description: "increase verbosity level",
        increaseCounter: () => resultValues.VerbosityLevel++)
};

var additionalOptions = new ICommonOption[]
{
    new ValueOption<TimeSpan>(
        longName: "time",
        shortName: "t",
        description: "maximum algorithm execution time",
        afterValueParsingAction: t => resultValues.Time = t),

    new EnumValueOption<FileMode>(
        longName: "file-mode",
        shortName: string.Empty,
        description: "specifies how the operatng system should open a file",
        defaultValue: new DefaultOptionValue<FileMode>(FileMode.OpenOrCreate),
        afterValueParsingAction: t => resultValues.FileMode = t),

    new MultipleValueOption<int>(
        longName: "date",
        description: "next date the program update notification will be displayed",
        metaVariable: "D",
        contextCapture: new FixedContextCapture(3),
        afterValueParsingAction: t => resultValues.Date = new DateTime(t[0], t[1], t[2]))
};

var resizeSubcommandOptions = new ICommonOption[]
{
    new ValueOption<int>(
        longName: "width",
        shortName: "w",
        description: "new width of the image",
        afterValueParsingAction: t => resultValues.Width = t),

    new ValueOption<int>(
        longName: "height",
        shortName: "H",
        description: "new height of the image",
        afterValueParsingAction: t => resultValues.Height = t)
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

            _ => throw new FormatException()
        };
    }),

    new MultipleValueConverter<PageRange>(PageRange.Parse),
    new MultipleValueConverter<PageFontSize>(PageFontSize.ParseMany)
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

OptionGroup<ICommonOption> group = parser.AddOptionGroup("Additional options:", "Description\n");
group.AddOptions(additionalOptions);

MutuallyExclusiveOptionGroup<ICommonOption> mutuallyExclusiveOptionGroup =
    parser.AddMutuallyExclusiveOptionGroup("group", null, [nameOption, nickOption]);

Subcommand resizeSubcommand = parser.AddSubcommand("resize", "resize the image");
resizeSubcommand.UseDefaultHelpOption = true;
resizeSubcommand.AddOptions(resizeSubcommandOptions);

ParseArgumentsResult result;
IList<string> extraArguments;

try
{
    result = parser.ParseKnownArguments(args, out extraArguments);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    return;
}

Console.WriteLine($"Handled options: {result.HandledOptions.Count}");
Console.WriteLine($"Handled subcommands: {result.HandledSubcommands.Count}");

if (result.TryGetLastHandledSubcommand(out Subcommand? subcommand))
    Console.WriteLine($"Last handled subcommand: {subcommand?.Name}");

Console.WriteLine();
Console.WriteLine($"Extra arguments: {string.Join(" ", extraArguments)}");
Console.WriteLine($"Verbose: {resultValues.Verbose}");
Console.WriteLine($"Quick: {resultValues.Quick}");
Console.WriteLine($"Verbosity level: {resultValues.VerbosityLevel}");
Console.WriteLine($"Angle: {resultValues.Angle}");
Console.WriteLine($"Time: {resultValues.Time}");
Console.WriteLine($"File mode: {resultValues.FileMode}");
Console.WriteLine($"Input files: {string.Join(" ", resultValues.InputFiles)}");
Console.WriteLine($"Persons: {string.Join(" ", resultValues.Persons)}");
Console.WriteLine($"Page ranges: {string.Join(" ", resultValues.PageRanges)}");
Console.WriteLine($"Page font sizes: {string.Join(" ", resultValues.PageFontSizes)}");
Console.WriteLine($"Date: {resultValues.Date?.ToLongDateString()}");
Console.WriteLine($"Name: {resultValues.Name}");
Console.WriteLine($"Width: {resultValues.Width}");
Console.WriteLine($"Height: {resultValues.Height}");

#pragma warning disable
internal sealed record PageRange(int Start, int End)
{
    public static PageRange Parse(string data)
    {
        int[] parts = data.Split('-').Select(int.Parse).ToArray();

        return parts.Length == 2
            ? new PageRange(parts[0], parts[1])
            : throw new ArgumentException("Incorrect format");
    }
}

internal sealed record PageFontSize(int PageNumber, int FontSize)
{
    public static PageFontSize Parse(string data)
    {
        int[] parts = data.Split(';').Select(int.Parse).ToArray();

        return parts.Length == 2
            ? new PageFontSize(parts[0], parts[1])
            : throw new ArgumentException("Incorrect format");
    }

    public static PageFontSize[] ParseMany(string data)
    {
        string[] parts = data.Split(':');

        if (parts.Length != 2)
            throw new ArgumentException("Incorrect format");

        PageRange pageRange = PageRange.Parse(parts[0]);
        IEnumerable<int> pages = Enumerable.Range(pageRange.Start, pageRange.End - pageRange.Start + 1);
        int fontSize = int.Parse(parts[1]);

        return pages.Select(t => $"{t};{fontSize}").Select(Parse).ToArray();
    }
}

internal sealed class ResultValues
{
    public bool Verbose { get; set; }
    public bool Quick { get; set; }
    public int? Angle { get; set; }
    public int VerbosityLevel { get; set; }
    public TimeSpan? Time { get; set; }
    public FileMode? FileMode { get; set; }
    public List<string> InputFiles { get; set; } = [];
    public List<string> Persons { get; set; } = [];
    public List<PageRange> PageRanges { get; set; } = [new PageRange(1, 1)];
    public List<PageFontSize> PageFontSizes { get; set; } = [];
    public DateTime? Date { get; set; }
    public string? Name { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
}
#pragma warning restore

/*
./NetArgumentParser.Examples.AllUseCases -n Name --verbose -q \
    --input ./NetArgumentParser.Examples.AllUseCases ./NetArgumentParser.dll \
    --persons David John Richard --ranges 2-5 6-8 --fonts 1-2:12 3-7:16 -a 45 \
    -VVV --time 1,2,3,4,5 --file-mode open --date 2026 02 15 \
    extra1 --extra2 extra3
*/
