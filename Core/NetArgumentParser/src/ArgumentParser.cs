using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Subcommands;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Utils;
using NetArgumentParser.Options.Utils.Verifiers;
using NetArgumentParser.Visitors;

namespace NetArgumentParser;

public class ArgumentParser : ParserQuantum
{
    private string _programName;
    private int _numberOfArgumentsToSkip;
    private readonly ArgumentsVisitor _argumentsVisitor;

    public ArgumentParser(
        IDescriptionGenerator? descriptionGenerator = null,
        ITextWriter? outputWriter = null,
        Func<Subcommand, IDescriptionGenerator>? subcommandDescriptionGeneratorCreator = null)
    {
        _programName = string.Empty;
        _numberOfArgumentsToSkip = 0;
        _argumentsVisitor = new ArgumentsVisitor(this, OptionSet);

        ProgramVersion = string.Empty;
        ProgramDescription = string.Empty;
        ProgramEpilog = string.Empty;

        RecognizeCompoundOptions = false;
        RecognizeSlashOptions = false;
        UseDefaultHelpOption = true;
        UseDefaultVersionOption = true;

        DescriptionGenerator = descriptionGenerator ?? new ApplicationDescriptionGenerator(this);
        OutputWriter = outputWriter ?? new ConsoleTextWriter();

        SubcommandDescriptionGeneratorCreator = subcommandDescriptionGeneratorCreator
            ?? (t => new SubcommandDescriptionGenerator(t));
    }

    #region Public Properties

    public string ProgramName
    {
        get => _programName;
        init
        {
            _programName = value;
            UsageStartTerm = value;
        }
    }

    public string ProgramVersion { get; init; }
    public string ProgramDescription { get; init; }
    public string ProgramEpilog { get; init; }

    public bool RecognizeCompoundOptions { get; init; }
    public bool RecognizeSlashOptions { get; init; }
    public bool UseDefaultVersionOption { get; set; }

    public int NumberOfArgumentsToSkip
    {
        get => _numberOfArgumentsToSkip;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(value));
            _numberOfArgumentsToSkip = value;
        }
    }

    #endregion

    #region String Representation Generation Methods

    public override string ToString()
    {
        return GenerateProgramDescription();
    }

    public string GenerateProgramDescription()
    {
        return DescriptionGenerator?.GenerateDescription() ?? string.Empty;
    }

    #endregion

    #region Output Stream Interaction Methods

    public void ChangeOutputWriter(ITextWriter? outputWriter)
    {
        OutputWriter = outputWriter;
    }

    #endregion

    #region Parsing Methods

    public virtual void ParseKnownArguments(IEnumerable<string> arguments, out List<string> extraArguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        AddDefaultOptions();
        IEnumerable<string> adaptedArguments = AdaptArguments(arguments);

        if (HandleFinalOptions(adaptedArguments))
        {
            extraArguments = [];
            return;
        }

        var visitorExtraArguments = new List<string>();

        _argumentsVisitor.VisitArguments(
            adaptedArguments,
            RecognizeSlashOptions,
            optionValue => optionValue.Option.Handle(optionValue.Value),
            visitorExtraArguments.Add);

        extraArguments = visitorExtraArguments;

        List<ICommonOption> allOptions = GetAllOptions();

        DynamicOptionInteractor.HandleDefaultValueBySuitableOptions(allOptions);
        ReuiredOptionVerifier.VerifyRequiredOptionsIsHandled(allOptions);

        ResetOptionsHandledState();
    }

    public void Parse(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        ParseKnownArguments(arguments, out List<string> extraArguments);

        if (extraArguments?.Count > 0)
            throw new ArgumentsAreUnknownException(null, [.. extraArguments]);
    }

    #endregion

    #region Default Option Interaction Methods

    protected override void AddDefaultOptions()
    {
        base.AddDefaultOptions();

        if (UseDefaultVersionOption && !OptionSet.HasVersionOption())
            AddDefaultVersionOption();
    }

    protected virtual void AddDefaultVersionOption()
    {
        var versionOption = new VersionOption(() =>
        {
            OutputWriter?.WriteLine(ProgramVersion);
            Environment.Exit(0);
        });

        AddOptions(versionOption);
    }

    #endregion

    #region Final Option Interaction Methods

    protected virtual bool HandleFinalOptions(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        return HandleFinalOption<HelpOption>(arguments)
            || HandleFinalOption<VersionOption>(arguments);
    }

    protected virtual bool HandleFinalOption<T>(IEnumerable<string> arguments)
        where T : ICommonOption
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        bool isFinalOptionHandled = false;

        _argumentsVisitor.VisitArguments(arguments, RecognizeSlashOptions, t =>
        {
            if (t.Option is T)
            {
                t.Option.Handle(t.Value);
                isFinalOptionHandled = true;
            }
        });

        return isFinalOptionHandled;
    }

    #endregion

    #region Argument Conversion Methods

    protected virtual IEnumerable<string> AdaptArguments(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        IEnumerable<string> consideredArguments = arguments
            .Skip(NumberOfArgumentsToSkip);

        return RecognizeCompoundOptions
            ? Argument.ExpandShortNamedOptions(consideredArguments)
            : consideredArguments;
    }

    #endregion
}
