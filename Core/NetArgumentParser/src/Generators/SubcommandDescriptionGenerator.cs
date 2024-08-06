using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetArgumentParser.Extensions;
using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Generators;

using OutOfRange = ArgumentOutOfRangeException;

public class SubcommandDescriptionGenerator : ParserQuantumDescriptionGenerator
{
    public SubcommandDescriptionGenerator(Subcommand subcommand)
        : base(subcommand, subcommand.UsageStartTerm, subcommand.Description)
    {
        ArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));
        Subcommand = subcommand;
    }

    protected Subcommand Subcommand { get; }

    public static string GenerateSubcommandDescriptions(
        IEnumerable<Subcommand> subcommands,
        string? namePrefix = null,
        string? delimiterAfterName = null)
    {
        ArgumentNullException.ThrowIfNull(subcommands, nameof(subcommands));

        int maxNameLength = subcommands.Max(t => t.Name.Length);
        int nameAreaLength = maxNameLength + (namePrefix?.Length ?? 0);

        IEnumerable<string> descriptions = subcommands.Select(t =>
            GenerateSubcommandDescription(t, nameAreaLength, namePrefix, delimiterAfterName));

        return string.Join(Environment.NewLine, descriptions);
    }

    public static string GenerateSubcommandDescription(
        Subcommand subcommand,
        int nameAreaLength,
        string? namePrefix = null,
        string? delimiterAfterName = null)
    {
        ArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));

        namePrefix ??= string.Empty;
        delimiterAfterName ??= string.Empty;

        OutOfRange.ThrowIfLessThan(
            nameAreaLength,
            subcommand.Name.Length + namePrefix.Length,
            nameof(nameAreaLength));

        string name = namePrefix + subcommand.Name;
        name = name.AddEmptyPostfix(nameAreaLength);

        return $"{name}{delimiterAfterName}{subcommand.Description}";
    }

    public override string GenerateDescription()
    {
        var descriptionBuilder = new StringBuilder();

        UsageDescriptionGenerator.AddUsage(descriptionBuilder);

        if (!string.IsNullOrEmpty(Subcommand.Description))
        {
            AddParserQuantumDescription(descriptionBuilder);
            descriptionBuilder.AppendLine();
        }

        OptionDescriptionGenerator.AddOptionDescriptions(descriptionBuilder);

        if (Subcommand.Subcommands.Count > 0)
            AddSubcommandDescriptions(descriptionBuilder);

        return descriptionBuilder
            .ToString()
            .RemoveLineBreakFromEnd();
    }
}
