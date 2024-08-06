using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetArgumentParser.Options;

namespace NetArgumentParser.Generators;

public class UsageDescriptionGenerator
{
    private int _windowWidth;

    public UsageDescriptionGenerator(IEnumerable<ICommonOption> options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        
        Options = options;
        
        WindowWidth = int.MaxValue;
        UsageHeader = "Usage: ";
        ProgramName = string.Empty;
    }

    public string? UsageHeader { get; set; }
    public string? ProgramName { get; set; }

    public int WindowWidth
    {
        get => _windowWidth;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
            _windowWidth = value;
        }
    }

    protected IEnumerable<ICommonOption> Options { get; }

    public virtual void AddUsage(StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        string usage = GenerateUsage();
        _ = builder.AppendLine(usage);
    }

    public virtual string GenerateUsage()
    {
        var usageBuilder = new StringBuilder(UsageHeader);
        int emptySpaceLength = UsageHeader?.Length ?? 0;

        if (!string.IsNullOrEmpty(ProgramName))
        {
            string nameWithDelimiter = ProgramName + ' ';
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
        return Options.Select(t =>
        {
            string example = t.GetShortExample();
            return t.IsRequired ? example : $"[{example}]";
        });
    }
}
