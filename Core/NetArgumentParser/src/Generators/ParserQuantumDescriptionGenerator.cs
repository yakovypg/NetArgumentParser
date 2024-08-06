using System;
using System.Text;
using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Generators;

using OutOfRange = ArgumentOutOfRangeException;

public abstract class ParserQuantumDescriptionGenerator : IDescriptionGenerator
{
    private string? _usageHeader;
    private string? _optionExamplePrefix;
    private string? _delimiterAfterOptionExample;
    
    private int _windowWidth;
    private int _optionExampleCharsLimit;

    public ParserQuantumDescriptionGenerator(
        ParserQuantum parserQuantum,
        string parserQuantumName,
        string parserQuantumDescription)
    {
        ArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ArgumentNullException.ThrowIfNull(parserQuantumName, nameof(parserQuantumName));
        ArgumentNullException.ThrowIfNull(parserQuantumDescription, nameof(parserQuantumDescription));

        _usageHeader = "Usage: ";
        _optionExamplePrefix = new string(' ', 2);
        _delimiterAfterOptionExample = new string(' ', 2);
        _windowWidth = int.MaxValue;
        _optionExampleCharsLimit = 30;
        
        ParserQuantum = parserQuantum;
        ParserQuantumName = parserQuantumName;
        ParserQuantumDescription = parserQuantumDescription;

        SubcommandsHeader = "Subcommands: ";
        SubcommandNamePrefix = new string(' ', 2);
        DelimiterAfterSubcommandName = new string(' ', 2);

        UsageDescriptionGenerator = new UsageDescriptionGenerator(parserQuantum.VisibleOptions)
        {
            WindowWidth = WindowWidth,
            UsageHeader = UsageHeader,
            ProgramName = parserQuantumName
        };

        OptionDescriptionGenerator = new OptionDescriptionGenerator(
            parserQuantum.VisibleOptions,
            parserQuantum.OptionGroups)
        {
            WindowWidth = WindowWidth,
            OptionExampleCharsLimit = OptionExampleCharsLimit,
            OptionExamplePrefix = OptionExamplePrefix,
            DelimiterAfterOptionExample = DelimiterAfterOptionExample
        };
    }

    public string? SubcommandsHeader { get; set; }
    public string? SubcommandNamePrefix { get; set; }
    public string? DelimiterAfterSubcommandName { get; set; }

    public string? UsageHeader
    {
        get => _usageHeader;
        set
        {
            _usageHeader = value;
            UsageDescriptionGenerator.UsageHeader = value;
        }
    }

    public string? OptionExamplePrefix
    {
        get => _optionExamplePrefix;
        set
        {
            _optionExamplePrefix = value;
            OptionDescriptionGenerator.OptionExamplePrefix = value;
        }
    }

    public string? DelimiterAfterOptionExample
    {
        get => _delimiterAfterOptionExample;
        set
        {
            _delimiterAfterOptionExample = value;
            OptionDescriptionGenerator.DelimiterAfterOptionExample = value;
        }
    }

    public int WindowWidth
    {
        get => _windowWidth;
        set
        {
            OutOfRange.ThrowIfNegativeOrZero(value, nameof(value));

            _windowWidth = value;
            UsageDescriptionGenerator.WindowWidth = value;
            OptionDescriptionGenerator.WindowWidth = value;
        }
    }

    public int OptionExampleCharsLimit
    {
        get => _optionExampleCharsLimit;
        set
        {
            OutOfRange.ThrowIfNegativeOrZero(value, nameof(value));

            _optionExampleCharsLimit = value;
            OptionDescriptionGenerator.OptionExampleCharsLimit = value;
        }
    }

    protected ParserQuantum ParserQuantum { get; }
    protected string ParserQuantumName { get; }
    protected string ParserQuantumDescription { get; }

    protected UsageDescriptionGenerator UsageDescriptionGenerator { get; }
    protected OptionDescriptionGenerator OptionDescriptionGenerator { get; }

    public abstract string GenerateDescription();

    protected virtual void AddParserQuantumDescription(StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        if (!string.IsNullOrEmpty(ParserQuantumDescription))
            _ = builder.AppendLine(ParserQuantumDescription);
    }

    protected virtual void AddSubcommandDescriptions(StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        if (ParserQuantum.Subcommands.Count == 0)
            return;

        string descriptions = SubcommandDescriptionGenerator.GenerateSubcommandDescriptions(
            ParserQuantum.Subcommands,
            SubcommandNamePrefix,
            DelimiterAfterSubcommandName);

        _ = builder.AppendLine(SubcommandsHeader);
        _ = builder.AppendLine(descriptions);
    }
}
