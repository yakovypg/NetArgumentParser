using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal class MutuallyExclusiveOptionGroupWithoutDuplicateInfoParserGeneratorConfig
{
    public const string GroupId = "group1";
    public const string GroupHeader = "group1 header";
    public const string GroupDescription = "group1 description";

    [FlagOption("flag1")]
    [MutuallyExclusiveOptionGroup(GroupId, "", "")]
    public bool Flag1 { get; set; }

    [FlagOption("flag2")]
    [MutuallyExclusiveOptionGroup(GroupId, GroupHeader, GroupDescription)]
    public bool Flag2 { get; set; }

    [FlagOption("flag3")]
    [MutuallyExclusiveOptionGroup(GroupId, "", "")]
    public bool Flag3 { get; set; }
}
