using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal class SubcommandsOnlyParserGeneratorConfig
{
    public const string BrunchSubcommandName = "branch";
    public const string BrunchSubcommandDescription = "branch description";
    public const string StatusSubcommandName = "status";
    public const string StatusSubcommandDescription = "status description";
    public const string FlagsSubcommandName = "flags";
    public const string FlagsSubcommandDescription = "flags description";

    public SubcommandsOnlyParserGeneratorConfig()
    {
        Branch = string.Empty;
        Status = 0;
        Flags = new FlagOptionsOnlyParserGeneratorConfig();
    }

    [Subcommand(BrunchSubcommandName, BrunchSubcommandDescription)]
    public string? Branch { get; }

    [Subcommand(StatusSubcommandName, StatusSubcommandDescription)]
    public int Status { get; }

    [Subcommand(FlagsSubcommandName, FlagsSubcommandDescription)]
    public FlagOptionsOnlyParserGeneratorConfig Flags { get; }

    [Subcommand("ignored", "i")]
    internal int IgnoredByInternalModifierSubcommand { get; }

    [Subcommand("ignored", "i")]
    protected int IgnoredByProtectedModifierSubcommand { get; }

#pragma warning disable IDE0051 // Remove unused private member
    [Subcommand("ignored", "i")]
    private int IgnoredByPrivateModifierSubcommand { get; }
#pragma warning restore IDE0051 // Remove unused private member
}
