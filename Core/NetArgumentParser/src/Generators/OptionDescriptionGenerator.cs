using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetArgumentParser.Extensions;
using NetArgumentParser.Options;

namespace NetArgumentParser.Generators;

using OutOfRange = ArgumentOutOfRangeException;

public class OptionDescriptionGenerator
{
    private int _windowWidth;
    private int _optionExampleCharsLimit;

    public OptionDescriptionGenerator(
        IEnumerable<ICommonOption> options,
        IEnumerable<OptionGroup<ICommonOption>> optionGroups)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(optionGroups, nameof(optionGroups));

        Options = options;
        OptionGroups = optionGroups;

        WindowWidth = int.MaxValue;
        OptionExampleCharsLimit = 30;

        OptionExamplePrefix = new string(' ', 2);
        DelimiterAfterOptionExample = new string(' ', 2);
    }

    public string? OptionExamplePrefix { get; set; }
    public string? DelimiterAfterOptionExample { get; set; }

    public int WindowWidth
    {
        get => _windowWidth;
        set
        {
            OutOfRange.ThrowIfNegativeOrZero(value, nameof(value));
            _windowWidth = value;
        }
    }

    public int OptionExampleCharsLimit
    {
        get => _optionExampleCharsLimit;
        set
        {
            OutOfRange.ThrowIfNegativeOrZero(value, nameof(value));
            _optionExampleCharsLimit = value;
        }
    }

    protected IEnumerable<ICommonOption> Options { get; }
    protected IEnumerable<OptionGroup<ICommonOption>> OptionGroups { get; }

    public virtual void AddOptionDescriptions(StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        foreach (OptionGroup<ICommonOption> group in OptionGroups)
        {
            string optionDescriptions = GenerateOptionDescriptions(group);

            _ = builder.AppendLine(group.Header);
            _ = builder.AppendLine(optionDescriptions);
        }
    }

    public virtual string GenerateOptionDescriptions(OptionGroup<ICommonOption> optionGroup)
    {
        ArgumentNullException.ThrowIfNull(optionGroup, nameof(optionGroup));

        var descriptionBuilder = new StringBuilder();

        foreach (ICommonOption option in optionGroup.VisibleOptions)
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

        List<string> descriptionParts = [.. option.Description.Split()];

        var longTextWriter = new LongTextWriter(
            builder,
            leftOffset,
            charsForDescriptionLine);

        longTextWriter.AppendParts(descriptionParts);
    }

    protected string GetEmptySpaceForOptionExample(int maxOptionExampleLength)
    {
        OutOfRange.ThrowIfNegative(
            maxOptionExampleLength,
            nameof(maxOptionExampleLength));

        int postfixLength = DelimiterAfterOptionExample?.Length ?? 0;
        return LongTextWriter.GetEmptySpace(maxOptionExampleLength + postfixLength);
    }

    protected string GetOptionExample(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        int maxOptionExampleLength = GetMaxOptionExampleLength();
        string example = OptionExamplePrefix + option.GetLongExample();

        return example.AddEmptyPostfix(maxOptionExampleLength);
    }

    protected int GetMaxOptionExampleLength()
    {
        int maxOptionExampleLength = Options
            .Where(t => t.GetLongExample().Length <= OptionExampleCharsLimit)
            .Max(t => t.GetLongExample().Length);

        maxOptionExampleLength += OptionExamplePrefix?.Length ?? 0;

        return maxOptionExampleLength;
    }
}
