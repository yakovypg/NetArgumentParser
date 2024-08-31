using System;
using System.Text;
using NetArgumentParser.Extensions;

namespace NetArgumentParser.Generators;

public class ApplicationDescriptionGenerator : ParserQuantumDescriptionGenerator
{
    public ApplicationDescriptionGenerator(ArgumentParser parser)
        : base(
            parser ?? throw new ArgumentNullException(nameof(parser)),
            parser.ProgramName,
            parser.ProgramDescription)
    {
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
        ExtendedArgumentNullException.ThrowIfNull(builder, nameof(builder));

        if (!string.IsNullOrEmpty(Parser.ProgramEpilog))
            _ = builder.AppendLine(Parser.ProgramEpilog);
    }
}
