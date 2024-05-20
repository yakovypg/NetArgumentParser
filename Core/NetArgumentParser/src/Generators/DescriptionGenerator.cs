using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetArgumentParser.Extensions;
using NetArgumentParser.Options;

namespace NetArgumentParser.Generators;

using OutOfRange = ArgumentOutOfRangeException;

public class DescriptionGenerator : IDescriptionGenerator
{
    private int _windowWidth;
    private int _optionExampleCharsLimit;
    
    public DescriptionGenerator(ArgumentParser parser)
    {
        ArgumentNullException.ThrowIfNull(parser, nameof(parser));

        Parser = parser;
        WindowWidth = int.MaxValue;
        OptionExampleCharsLimit = 30;
        OptionExamplePrefix = new string(' ', 2);
        DelimiterAfterOptionExample = new string(' ', 2);
    }

    public string? OptionExamplePrefix { get; init; }
    public string? DelimiterAfterOptionExample { get; init; }

    public int WindowWidth
    {
        get => _windowWidth;
        init
        {
            OutOfRange.ThrowIfNegativeOrZero(value, nameof(value));
            _windowWidth = value;
        }
    }

    public int OptionExampleCharsLimit
    {
        get => _optionExampleCharsLimit;
        init
        {
            OutOfRange.ThrowIfNegativeOrZero(value, nameof(value));
            _optionExampleCharsLimit = value;
        }
    }

    protected ArgumentParser Parser { get; }

    public virtual string GenerateDescription()
    {
        var descriptionBuilder = new StringBuilder();

        AddUsage(descriptionBuilder);

        if (!string.IsNullOrEmpty(Parser.ProgramDescription))
        {
            AddProgramDescription(descriptionBuilder);
            descriptionBuilder.AppendLine();
        }

        AddOptionDescriptions(descriptionBuilder);

        if (!string.IsNullOrEmpty(Parser.ProgramEpilog))
            AddProgramEpilog(descriptionBuilder);

        return descriptionBuilder.ToString().RemoveLineBreakFromEnd();
    }

    protected virtual void AddUsage(StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        string usage = GenerateUsage();
        _ = builder.AppendLine(usage);
    }

    protected virtual void AddOptionDescriptions(StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        foreach (OptionGroup<ICommonOption> group in Parser.OptionGroups)
        {
            string optionDescriptions = GenerateOptionDescriptions(group);

            _ = builder.AppendLine(group.Header);
            _ = builder.AppendLine(optionDescriptions);
        }
    }

    protected virtual void AddProgramDescription(StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        if (!string.IsNullOrEmpty(Parser.ProgramDescription))
            _ = builder.AppendLine(Parser.ProgramDescription);
    }

    protected virtual void AddProgramEpilog(StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        if (!string.IsNullOrEmpty(Parser.ProgramEpilog))
            _ = builder.AppendLine(Parser.ProgramEpilog);
    }

    protected virtual string GenerateUsage()
    {
        var usageBuilder = new StringBuilder(Parser.UsageHeader);
        int emptySpaceLength = Parser.UsageHeader.Length;

        if (!string.IsNullOrEmpty(Parser.ProgramName))
        {
            string nameWithDelimiter = Parser.ProgramName + ' ';
            _ = usageBuilder.Append(nameWithDelimiter);

            emptySpaceLength += nameWithDelimiter.Length;
        }

        List<string> descriptionParts = GenerateOptionDescriptionsForUsage().ToList();
        
        int charsForDescriptionLine = WindowWidth - emptySpaceLength;
        int leftOffset = emptySpaceLength;

        var longTextWriter = new LongTextWriter(
            usageBuilder,
            leftOffset,
            charsForDescriptionLine);

        longTextWriter.AppendParts(descriptionParts);

        return usageBuilder.ToString();
    }

    protected virtual IEnumerable<string> GenerateOptionDescriptionsForUsage()
    {
        return Parser.AllOptions.Select(t =>
        {
            string example = t.GetShortExample();
            return t.IsRequired ? example : $"[{example}]";
        });
    }

    protected virtual string GenerateOptionDescriptions(OptionGroup<ICommonOption> optionGroup)
    {
        ArgumentNullException.ThrowIfNull(optionGroup, nameof(optionGroup));

        var descriptionBuilder = new StringBuilder();

        foreach (ICommonOption option in optionGroup.Options)
        {
            AppendOptionExample(descriptionBuilder, option);
            AppendOptionDescription(descriptionBuilder, option);
        }

        return descriptionBuilder.ToString();
    }

    protected virtual void AppendOptionExample(StringBuilder builder, ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        int maxExampleLength = GetMaxOptionExampleLength();
        
        string example = GetOptionExample(option);
        string exampleWithDelimiter = example + DelimiterAfterOptionExample;

        if (example.Length > maxExampleLength)
        {
            string emptySpace = GetEmptySpaceForOptionExample(maxExampleLength);
            _ = builder.AppendLine(exampleWithDelimiter).Append(emptySpace);
        }
        else
        {
            _ = builder.Append(exampleWithDelimiter);
        }
    }

    protected virtual void AppendOptionDescription(StringBuilder builder, ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        int maxExampleLength = GetMaxOptionExampleLength();
        int emptySpaceLength = GetEmptySpaceForOptionExample(maxExampleLength).Length;
        
        int charsForDescriptionLine = WindowWidth - emptySpaceLength;
        int leftOffset = emptySpaceLength;

        List<string> descriptionParts = [..option.Description.Split()];

        var longTextWriter = new LongTextWriter(
            builder,
            leftOffset,
            charsForDescriptionLine);

        longTextWriter.AppendParts(descriptionParts);
    }

    private string GetEmptySpaceForOptionExample(int maxOptionExampleLength)
    {
        OutOfRange.ThrowIfNegative(
            maxOptionExampleLength,
            nameof(maxOptionExampleLength));

        int postfixLength = DelimiterAfterOptionExample?.Length ?? 0;
        return LongTextWriter.GetEmptySpace(maxOptionExampleLength + postfixLength);
    }

    private string GetOptionExample(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        int maxOptionExampleLength = GetMaxOptionExampleLength();
        string example = OptionExamplePrefix + option.GetLongExample();

        if (example.Length < maxOptionExampleLength)
        {
            string emptyPostfix = new(' ', maxOptionExampleLength - example.Length);
            example += emptyPostfix;
        }

        return example;
    }

    private int GetMaxOptionExampleLength()
    {
        int maxOptionExampleLength = Parser.AllOptions
            .Where(t => t.GetLongExample().Length <= OptionExampleCharsLimit)
            .Max(t => t.GetLongExample().Length);
        
        maxOptionExampleLength += OptionExamplePrefix?.Length ?? 0;

        return maxOptionExampleLength;
    }
}
