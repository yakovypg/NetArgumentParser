using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Converters;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Utils;
using NetArgumentParser.Options.Utils.Verifiers;

namespace NetArgumentParser;

public class ArgumentParser
{
    private int _numberOfArgumentsToSkip;
    private ITextWriter _outputWriter;
    private IDescriptionGenerator _descriptionGenerator;

    private readonly List<OptionGroup<ICommonOption>> _optionGroups;

    public ArgumentParser(
        IDescriptionGenerator? descriptionGenerator = null,
        ITextWriter? outputWriter = null)
    {
        _descriptionGenerator = descriptionGenerator ?? new DescriptionGenerator(this);
        _outputWriter = outputWriter ?? new ConsoleTextWriter();

        UsageHeader = "Usage: ";
        OptionsHeader = $"Options:";
        OptionSet = new OptionSet();

        _optionGroups = [new OptionGroup<ICommonOption>(OptionsHeader, OptionSet)];

        UseDefaultHelpOption = true;
        UseDefaultVersionOption = true;

        ProgramName = string.Empty;
        ProgramVersion = string.Empty;
        ProgramDescription = string.Empty;
        ProgramEpilog = string.Empty;
    }

    public string UsageHeader { get; init; }
    public string OptionsHeader { get; init; }

    public string ProgramName { get; init; }
    public string ProgramVersion { get; init; }
    public string ProgramDescription { get; init; }
    public string ProgramEpilog { get; init; }

    public bool RecognizeCompoundOptions { get; init; }
    public bool RecognizeSlashOptions { get; init; }

    public bool UseDefaultHelpOption { get; init; }
    public bool UseDefaultVersionOption { get; init; }

    public int NumberOfArgumentsToSkip
    {
        get => _numberOfArgumentsToSkip;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(value));
            _numberOfArgumentsToSkip = value;
        }
    }

    public ITextWriter OutputWriter
    {
        get => _outputWriter;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            _outputWriter = value;
        }
    }

    public IDescriptionGenerator DescriptionGenerator
    {
        get => _descriptionGenerator;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            _descriptionGenerator = value;
        }
    }

    public OptionGroup<ICommonOption> DefaultGroup => _optionGroups.First();
    public IReadOnlyList<ICommonOption> AllOptions => OptionSet.Options;
    public IReadOnlyList<OptionGroup<ICommonOption>> OptionGroups => _optionGroups;

    protected IOptionSet<ICommonOption> OptionSet { get; }

    public override string ToString()
    {
        return GenerateProgramDescription();
    }

    public string GenerateProgramDescription()
    {
        return DescriptionGenerator.GenerateDescription();
    }

    public OptionGroup<ICommonOption> AddOptionGroup(string name)
    {
        var group = new OptionGroup<ICommonOption>(name, OptionSet);
        _optionGroups.Add(group);

        return group;
    }

    #region Parsing Methods

    public virtual void ParseKnownArgs(IEnumerable<string> arguments, out List<string> extraArguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        AddDefaultOptions();

        IEnumerable<string> adaptedArguments = AdaptArguments(arguments);
        var context = new Queue<string>(adaptedArguments);

        extraArguments = [];

        if (HandleFinalOptions(adaptedArguments))
            return;

        while (context.Count > 0)
        {
            string contextItem = context.Dequeue();
            var argument = new Argument(contextItem, RecognizeSlashOptions);

            bool isContextItemHandled = false;
            
            if (argument.IsOption)
            {
                string optionName = argument.ExtractOptionName();

                if (OptionSet.HasOption(optionName))
                {
                    OptionValue optionValue = argument.ExtractOptionValueFromContext(context, OptionSet);
                    optionValue.Option.Handle(optionValue.Value);
                    isContextItemHandled = true;
                }
            }

            if (!isContextItemHandled)
                extraArguments.Add(contextItem);
        }

        DynamicOptionInteractor.HandleDefaultValueBySuitableOptions(OptionSet.Options);
        ReuiredOptionVerifier.VerifyRequiredOptionsIsHandled(OptionSet.Options);

        OptionSet.ResetOptionsHandledState();
    }

    public void Parse(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        ParseKnownArgs(arguments, out List<string> extraArguments);

        if (extraArguments?.Count > 0)
            throw new ArgumentsAreUnknownException(null, [..extraArguments]);
    }

    #endregion

    #region Option Set Interaction Methods

    public void AddOptions(params ICommonOption[] options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        DefaultGroup.AddOptions(options);
    }

    public void AddConverters(params IValueConverter[] converters)
    {
        ArgumentNullException.ThrowIfNull(converters, nameof(converters));
        Array.ForEach(converters, OptionSet.AddConverter);
    }

    public bool RemoveOption(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));
        return OptionSet.RemoveOption(option);
    }

    public void ResetOptionsHandledState()
    {
        OptionSet.ResetOptionsHandledState();
    }

    #endregion

    protected virtual void AddDefaultOptions()
    {
        if (UseDefaultHelpOption && !OptionSet.HasHelpOption())
            AddDefaultHelpOption();
        
        if (UseDefaultVersionOption && !OptionSet.HasVersionOption())
            AddDefaultVersionOption();
    }

    protected virtual IEnumerable<string> AdaptArguments(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        IEnumerable<string> consideredArguments = arguments
            .Skip(NumberOfArgumentsToSkip);

        return RecognizeCompoundOptions
            ? Argument.ExpandShortMinusOptions(consideredArguments)
            : consideredArguments;
    }

    protected virtual bool HandleFinalOptions(IEnumerable<string> arguments)
    {
        return HandleFinalOption<HelpOption>(arguments)
            || HandleFinalOption<VersionOption>(arguments);
    }

    protected virtual bool HandleFinalOption<T>(IEnumerable<string> arguments)
        where T : ICommonOption
    {
        if (AllOptions.FirstOrDefault(t => t is T) is not T finalOption)
            return false;

        string? finalOptionArgument = arguments
            .Where(t => new Argument(t, RecognizeSlashOptions).IsOption)
            .Select(t => new Argument(t, RecognizeSlashOptions).ExtractOptionName())
            .FirstOrDefault(t => t == finalOption.LongName || t == finalOption.ShortName);
        
        if (finalOptionArgument is not null)
        {
            finalOption.Handle();
            return true;
        }

        return false;
    }

    private void AddDefaultHelpOption()
    {
        var helpOption = new HelpOption(() =>
        {
            OutputWriter.WriteLine(GenerateProgramDescription());
            Environment.Exit(0);
        });

        AddOptions(helpOption);
    }

    private void AddDefaultVersionOption()
    {
        var versionOption = new VersionOption(() =>
        {
            OutputWriter.WriteLine(ProgramVersion);
            Environment.Exit(0);
        });

        AddOptions(versionOption);
    }
}
