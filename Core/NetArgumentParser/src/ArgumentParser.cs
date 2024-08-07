using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Generators;
using NetArgumentParser.Informing;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Utils;
using NetArgumentParser.Options.Utils.Verifiers;
using NetArgumentParser.Subcommands;
using NetArgumentParser.Visitors;

namespace NetArgumentParser;

public class ArgumentParser : ParserQuantum
{
    private string _programName;
    private int _numberOfArgumentsToSkip;

    public ArgumentParser(
        IDescriptionGenerator? descriptionGenerator = null,
        ITextWriter? outputWriter = null,
        Func<Subcommand, IDescriptionGenerator>? subcommandDescriptionGeneratorCreator = null)
    {
        _programName = string.Empty;
        _numberOfArgumentsToSkip = 0;

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

    public override string ToString()
    {
        return GenerateProgramDescription();
    }

    public string GenerateProgramDescription()
    {
        return DescriptionGenerator?.GenerateDescription() ?? string.Empty;
    }

    public void ChangeOutputWriter(ITextWriter? outputWriter)
    {
        OutputWriter = outputWriter;
    }

    public virtual ParseArgumentsResult ParseKnownArguments(
        IEnumerable<string> arguments,
        out IList<string> extraArguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        AddDefaultOptions();
        IEnumerable<string> adaptedArguments = AdaptArguments(arguments);

        if (HandleFinalOptions(adaptedArguments, out ParseArgumentsResult parseArgumentsResult))
        {
            extraArguments = [];
            return parseArgumentsResult;
        }

        var visitorExtraArguments = new List<string>();
        var argumentsVisitor = new ArgumentsVisitor(this, OptionSet, RecognizeSlashOptions);

        var handledOptions = new List<ICommonOption>();
        var handledSubcommands = new List<Subcommand>();

        argumentsVisitor.SubcommandExtracted += (s, e) =>
            handledSubcommands.Add(e.Subcommand);

        argumentsVisitor.OptionExtracted += (s, e) =>
        {
            e.OptionValue.Option.Handle([.. e.OptionValue.Value]);
            handledOptions.Add(e.OptionValue.Option);
        };

        argumentsVisitor.UndefinedContextItemExtracted += (s, e) =>
            visitorExtraArguments.Add(e.ContextItem);

        argumentsVisitor.VisitArguments(adaptedArguments);
        extraArguments = visitorExtraArguments;

        IList<ICommonOption> allOptions = GetAllOptions();

        DynamicOptionInteractor.HandleDefaultValueBySuitableOptions(allOptions);
        ReuiredOptionVerifier.VerifyRequiredOptionsIsHandled(allOptions);

        ResetOptionsHandledState();

        return new ParseArgumentsResult(handledOptions, handledSubcommands);
    }

    public ParseArgumentsResult Parse(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        ParseArgumentsResult result = ParseKnownArguments(
            arguments,
            out IList<string> extraArguments);

        return extraArguments.Count == 0
            ? result
            : throw new ArgumentsAreUnknownException(null, [.. extraArguments]);
    }

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

    protected virtual bool HandleFinalOptions(
        IEnumerable<string> arguments,
        out ParseArgumentsResult parseArgumentsResult)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        return HandleFinalOption<HelpOption>(arguments, out parseArgumentsResult)
            || HandleFinalOption<VersionOption>(arguments, out parseArgumentsResult);
    }

    protected virtual bool HandleFinalOption<T>(
        IEnumerable<string> arguments,
        out ParseArgumentsResult parseArgumentsResult)
        where T : ICommonOption
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        bool isFinalOptionHandled = false;
        var argumentsVisitor = new ArgumentsVisitor(this, OptionSet, RecognizeSlashOptions);

        var handledOptions = new List<ICommonOption>();
        var handledSubcommands = new List<Subcommand>();

        argumentsVisitor.SubcommandExtracted += (s, e) =>
        {
            if (!isFinalOptionHandled)
                handledSubcommands.Add(e.Subcommand);
        };

        argumentsVisitor.OptionExtracted += (s, e) =>
        {
            if (!isFinalOptionHandled && e.OptionValue.Option is T)
            {
                e.OptionValue.Option.Handle([.. e.OptionValue.Value]);
                handledOptions.Add(e.OptionValue.Option);
                isFinalOptionHandled = true;
            }
        };

        argumentsVisitor.VisitArguments(arguments);
        parseArgumentsResult = new ParseArgumentsResult(handledOptions, handledSubcommands);

        return isFinalOptionHandled;
    }

    protected virtual IEnumerable<string> AdaptArguments(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        IEnumerable<string> consideredArguments = arguments
            .Skip(NumberOfArgumentsToSkip);

        return RecognizeCompoundOptions
            ? Argument.ExpandShortNamedOptions(consideredArguments)
            : consideredArguments;
    }
}
