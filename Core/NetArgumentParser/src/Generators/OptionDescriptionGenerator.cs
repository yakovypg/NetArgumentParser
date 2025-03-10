using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetArgumentParser.Extensions;
using NetArgumentParser.Options;

namespace NetArgumentParser.Generators;

public class OptionDescriptionGenerator
{
    private int _windowWidth;
    private int _optionExampleCharsLimit;

    public OptionDescriptionGenerator(
        IEnumerable<ICommonOption> options,
        IEnumerable<OptionGroup<ICommonOption>> optionGroups)
    {
        ExtendedArgumentNullException.ThrowIfNull(options, nameof(options));
        ExtendedArgumentNullException.ThrowIfNull(optionGroups, nameof(optionGroups));

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
            DefaultExceptions.ThrowIfNegativeOrZero(value, nameof(value));
            _windowWidth = value;
        }
    }

    public int OptionExampleCharsLimit
    {
        get => _optionExampleCharsLimit;
        set
        {
            DefaultExceptions.ThrowIfNegativeOrZero(value, nameof(value));
            _optionExampleCharsLimit = value;
        }
    }

    protected IEnumerable<ICommonOption> Options { get; }
    protected IEnumerable<OptionGroup<ICommonOption>> OptionGroups { get; }

    public virtual void AddOptionDescriptions(StringBuilder builder)
    {
        ExtendedArgumentNullException.ThrowIfNull(builder, nameof(builder));

        foreach (OptionGroup<ICommonOption> group in OptionGroups)
        {
            string optionDescriptions = GenerateOptionDescriptions(group);

            _ = builder.AppendLine(group.Header);

            if (!string.IsNullOrEmpty(group.Description))
            {
                string description = OptionExamplePrefix + group.Description;
                _ = builder.AppendLine(description);
            }

            _ = builder.AppendLine(optionDescriptions);
        }
    }

    public virtual string GenerateOptionDescriptions(OptionGroup<ICommonOption> optionGroup)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionGroup, nameof(optionGroup));

        var descriptionBuilder = new StringBuilder();

        foreach (ICommonOption option in optionGroup.VisibleOptions)
        {
            AppendOptionExample(descriptionBuilder, option);
            AppendOptionDescription(descriptionBuilder, option);
        }

        return descriptionBuilder.ToString();
    }

    protected virtual void AppendOptionExample(StringBuilder builder, ICommonOption commonOption)
    {
        ExtendedArgumentNullException.ThrowIfNull(builder, nameof(builder));
        ExtendedArgumentNullException.ThrowIfNull(commonOption, nameof(commonOption));

        int maxExampleLength = GetMaxSuitableOptionExampleLength();

        string example = GetOptionExample(commonOption);
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

    protected virtual void AppendOptionDescription(StringBuilder builder, ICommonOption commonOption)
    {
        ExtendedArgumentNullException.ThrowIfNull(builder, nameof(builder));
        ExtendedArgumentNullException.ThrowIfNull(commonOption, nameof(commonOption));

        int maxExampleLength = GetMaxSuitableOptionExampleLength();
        int emptySpaceLength = GetEmptySpaceForOptionExample(maxExampleLength).Length;

        int charsForDescriptionLine = WindowWidth - emptySpaceLength;
        int leftOffset = emptySpaceLength;

        List<string> descriptionParts = [.. commonOption.Description.Split()];

        var longTextWriter = new LongTextWriter(
            builder,
            leftOffset,
            charsForDescriptionLine);

        longTextWriter.AppendParts(descriptionParts);
    }

    protected string GetEmptySpaceForOptionExample(int maxOptionExampleLength)
    {
        DefaultExceptions.ThrowIfNegative(
            maxOptionExampleLength,
            nameof(maxOptionExampleLength));

        int postfixLength = DelimiterAfterOptionExample?.Length ?? 0;
        return LongTextWriter.GetEmptySpace(maxOptionExampleLength + postfixLength);
    }

    protected string GetOptionExample(ICommonOption option)
    {
        ExtendedArgumentNullException.ThrowIfNull(option, nameof(option));

        int maxOptionExampleLength = GetMaxSuitableOptionExampleLength();
        string example = OptionExamplePrefix + option.GetLongExample();

        return example.AddEmptyPostfix(maxOptionExampleLength);
    }

    protected int GetMaxSuitableOptionExampleLength()
    {
        if (!Options.Any())
            return 0;

        List<ICommonOption> suitableOptions = Options
            .Where(t => t.GetLongExample().Length <= OptionExampleCharsLimit)
            .ToList();

        int maxOptionExampleLength = suitableOptions.Count > 0
            ? suitableOptions.Max(t => t.GetLongExample().Length)
            : Options.Min(t => t.GetLongExample().Length);

        maxOptionExampleLength += OptionExamplePrefix?.Length ?? 0;

        return maxOptionExampleLength;
    }
}
