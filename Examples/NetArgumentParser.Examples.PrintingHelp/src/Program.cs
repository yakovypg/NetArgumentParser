using NetArgumentParser;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Context;
using NetArgumentParser.Subcommands;

static SubcommandDescriptionGenerator CreateSubcommandDescriptionGenerator(Subcommand subcommand)
{
    return new SubcommandDescriptionGenerator(subcommand);
}

var parser = new ArgumentParser()
{
    ProgramName = "ProgramName",
    ProgramVersion = "ProgramName v1.0.0",
    ProgramDescription = "What the program does",
    ProgramEpilog = "Text at the bottom",
    SubcommandDescriptionGeneratorCreator = CreateSubcommandDescriptionGenerator
};

var generator = new ApplicationDescriptionGenerator(parser)
{
    UsageHeader = "My Usage: ",
    SubcommandsHeader = "My Subcommands:",
    OptionExamplePrefix = "@@",
    DelimiterAfterOptionExample = " -> ",
    SubcommandNamePrefix = "##",
    DelimiterAfterSubcommandName = " >> ",
    WindowWidth = 40,
    OptionExampleCharsLimit = 11
};

parser.DescriptionGenerator = generator;
parser.DefaultGroup.Header = "Default group:";

parser.ChangeOutputWriter(new ConsoleTextWriter());

parser.AddOptions([
    new FlagOption(
        longName: "verbose",
        description: "be verbose"),

    new ValueOption<string>(
        longName: "name",
        description: "very long description that does not fit on one line",
        isRequired: true)
]);

Subcommand statusSubcommand = parser.AddSubcommand("status", "current status");

statusSubcommand.AddOptions([
    new MultipleValueOption<string>(
        longName: "branches",
        isRequired: true,
        contextCapture: new OneOrMoreContextCapture())
]);

_ = parser.Parse(["--help"]);
