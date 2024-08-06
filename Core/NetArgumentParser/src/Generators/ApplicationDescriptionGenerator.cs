using System;
using System.Text;
using NetArgumentParser.Extensions;

namespace NetArgumentParser.Generators;

public class ApplicationDescriptionGenerator : ParserQuantumDescriptionGenerator
{
    public ApplicationDescriptionGenerator(ArgumentParser parser)
        : base(parser, parser.ProgramName, parser.ProgramDescription)
    {
        ArgumentNullException.ThrowIfNull(parser, nameof(parser));
        Parser = parser;
    }

    protected ArgumentParser Parser { get; }

    public override string GenerateDescription()
    {
        var descriptionBuilder = new StringBuilder();

        UsageDescriptionGenerator.AddUsage(descriptionBuilder);

        if (!string.IsNullOrEmpty(Parser.ProgramDescription))
        {
            AddParserQuantumDescription(descriptionBuilder);
            descriptionBuilder.AppendLine();
        }

        OptionDescriptionGenerator.AddOptionDescriptions(descriptionBuilder);

        if (Parser.Subcommands.Count > 0)
        {
            AddSubcommandDescriptions(descriptionBuilder);
            descriptionBuilder.AppendLine();
        }

        if (!string.IsNullOrEmpty(Parser.ProgramEpilog))
            AddProgramEpilog(descriptionBuilder);

        return descriptionBuilder
            .ToString()
            .RemoveLineBreakFromEnd();
    }

    protected virtual void AddProgramEpilog(StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        if (!string.IsNullOrEmpty(Parser.ProgramEpilog))
            _ = builder.AppendLine(Parser.ProgramEpilog);
    }
}
