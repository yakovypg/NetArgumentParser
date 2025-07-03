using System.Collections.Generic;
using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal class FlagOptionsOnlyParserGeneratorConfig
{
    public const string ShowGuideLongName = "guide";
    public const string ShowGuideShortName = "g";
    public const string ShowGuideDescription = "guide description";
    public const bool ShowGuideIsRequired = true;
    public const bool ShowGuideIsHidden = false;
    public const bool ShowGuideIsFinal = false;

    public const string OverwriteFileLongName = "file";
    public const string OverwriteFileShortName = "f";
    public const string OverwriteFileDescription = "file description";
    public const bool OverwriteFileIsRequired = true;
    public const bool OverwriteFileIsHidden = true;
    public const bool OverwriteFileIsFinal = false;

    public const string OverwriteDirectoryLongName = "directory";
    public const string OverwriteDirectoryShortName = "d";
    public const string OverwriteDirectoryDescription = "directory description";
    public const bool OverwriteDirectoryIsRequired = false;
    public const bool OverwriteDirectoryIsHidden = false;
    public const bool OverwriteDirectoryIsFinal = true;

    public FlagOptionsOnlyParserGeneratorConfig() { }

    public static IReadOnlyList<string> ShowGuideAliases { get; } = ["g1", "g2", "g3"];
    public static IReadOnlyList<string> OverwriteFileAliases { get; } = [];
    public static IReadOnlyList<string> OverwriteDirectoryAliases { get; } = ["o1", "o2"];

    [FlagOption(
        ShowGuideLongName,
        ShowGuideShortName,
        ShowGuideDescription,
        ShowGuideIsRequired,
        ShowGuideIsHidden,
        ShowGuideIsFinal,
        ["g1", "g2", "g3"])]
    public bool ShowGuide { get; set; }

    [FlagOption(
        OverwriteFileLongName,
        OverwriteFileShortName,
        OverwriteFileDescription,
        OverwriteFileIsRequired,
        OverwriteFileIsHidden,
        OverwriteFileIsFinal,
        [])]
    public bool OverwriteFile { get; set; }

    [FlagOption(
        OverwriteDirectoryLongName,
        OverwriteDirectoryShortName,
        OverwriteDirectoryDescription,
        OverwriteDirectoryIsRequired,
        OverwriteDirectoryIsHidden,
        OverwriteDirectoryIsFinal,
        ["o1", "o2"])]
    public bool OverwriteDirectory { get; set; }

    [FlagOption("ignored", "i")]
    public bool IgnoredBySetAccessorOption { get; }

    [FlagOption("ignored", "i")]
    internal bool IgnoredByInternalModifierOption { get; set; }

    [FlagOption("ignored", "i")]
    protected bool IgnoredByProtectedModifierOption { get; set; }

#pragma warning disable IDE0051 // Remove unused private member
    [FlagOption("ignored", "i")]
    private bool IgnoredByPrivateModifierOption { get; set; }
#pragma warning restore IDE0051 // Remove unused private member
}
