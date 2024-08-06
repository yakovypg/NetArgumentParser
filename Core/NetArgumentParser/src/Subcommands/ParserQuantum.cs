using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Converters;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;
using NetArgumentParser.Subcommands.Utils.Verifiers;

namespace NetArgumentParser.Subcommands;

public class ParserQuantum : IOptionSetOrganizer, ISubcommandContainer
{   
    private readonly OptionSet _optionSet;
    private readonly List<OptionGroup<ICommonOption>> _optionGroups;
    private readonly List<Subcommand> _subcommands;

    private ITextWriter? _outputWriter;
    private readonly SubcommandNameUniquenessVerifier _nameUniquenessVerifier;

    public ParserQuantum()
    {        
        _optionSet = new OptionSet();
        _optionGroups = [new OptionGroup<ICommonOption>("Options:", _optionSet)];
        
        _subcommands = [];
        _nameUniquenessVerifier = new SubcommandNameUniquenessVerifier(_subcommands);

        UseDefaultHelpOption = true;
        UsageStartTerm = string.Empty;
    }

    #region Public Properties

    public string UsageStartTerm { get; set; }
    public bool UseDefaultHelpOption { get; set; }

    public IDescriptionGenerator? DescriptionGenerator { get; set; }
    public Func<Subcommand, IDescriptionGenerator>? SubcommandDescriptionGeneratorCreator { get; set; }

    #endregion

    #region Public Read-only Properties

    public IReadOnlyList<ICommonOption> Options => _optionSet.Options;
    public IEnumerable<ICommonOption> HiddenOptions => Options.Where(t => t.IsHidden);
    public IEnumerable<ICommonOption> VisibleOptions => Options.Where(t => !t.IsHidden);

    public IReadOnlyList<OptionGroup<ICommonOption>> OptionGroups => _optionGroups;
    public OptionGroup<ICommonOption> DefaultGroup => _optionGroups[0];

    public IReadOnlyList<Subcommand> Subcommands => _subcommands;
    public IReadOnlyOptionSet<ICommonOption> OptionSet => _optionSet;

    #endregion

    #region Protected Properties

    protected virtual ITextWriter? OutputWriter
    {
        get => _outputWriter;
        set
        {
            _outputWriter = value;

            foreach (Subcommand subcommand in _subcommands)
            {
                subcommand.OutputWriter = value;
            }
        }
    }

    #endregion

    #region Option Set Organizer Methods

    public virtual void ResetOptionsHandledState()
    {
        _optionSet.ResetOptionsHandledState();

        foreach (Subcommand subcommand in _subcommands)
        {
            subcommand.ResetOptionsHandledState();
        }
    }

    public void AddOptions(params ICommonOption[] options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        DefaultGroup.AddOptions(options);
    }

    public void AddConverters(params IValueConverter[] converters)
    {
        ArgumentNullException.ThrowIfNull(converters, nameof(converters));
        Array.ForEach(converters, _optionSet.AddConverter);
    }

    public virtual bool RemoveOption(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));
        return _optionSet.RemoveOption(option);
    }

    public virtual bool RemoveConverter(IValueConverter converter)
    {
        ArgumentNullException.ThrowIfNull(converter, nameof(converter));
        return _optionSet.RemoveConverter(converter);
    }

    public OptionGroup<ICommonOption> AddOptionGroup(string name)
    {
        var group = new OptionGroup<ICommonOption>(name, _optionSet);
        _optionGroups.Add(group);

        return group;
    }

    public List<ICommonOption> GetAllOptions()
    {
        var options = new List<ICommonOption>(Options);
        
        foreach (Subcommand subcommand in _subcommands)
        {
            List<ICommonOption> currOptions = subcommand.GetAllOptions();
            options.AddRange(currOptions);
        }

        return options;
    }

    #endregion

    #region Subcommand Container Methods

    public Subcommand AddSubcommand(string name, string description)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        ArgumentNullException.ThrowIfNull(description, nameof(description));

        var subcommand = new Subcommand(name, description)
        {
            UseDefaultHelpOption = UseDefaultHelpOption,
            OutputWriter = OutputWriter,
            SubcommandDescriptionGeneratorCreator = SubcommandDescriptionGeneratorCreator,
            UsageStartTerm = $"{UsageStartTerm} {name}"
        };

        if (SubcommandDescriptionGeneratorCreator is not null)
            subcommand.DescriptionGenerator = SubcommandDescriptionGeneratorCreator(subcommand);

        _nameUniquenessVerifier.VerifyNameIsUnique(subcommand);
        _subcommands.Add(subcommand);

        return subcommand;
    }

    public virtual bool RemoveSubcommand(Subcommand subcommand)
    {
        ArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));
        return _subcommands.Remove(subcommand);
    }

    #endregion

    #region Default Option Interaction Methods

    protected virtual void AddDefaultOptions()
    {        
        foreach (ParserQuantum quantum in _subcommands.Append(this))
        {
            if (quantum.UseDefaultHelpOption && !quantum.OptionSet.HasHelpOption())
                quantum.AddDefaultHelpOption();
        }
    }

    protected virtual void AddDefaultHelpOption()
    {
        var helpOption = new HelpOption(() =>
        {
            string? description = DescriptionGenerator?.GenerateDescription();
            OutputWriter?.WriteLine(description);
            Environment.Exit(0);
        });

        AddOptions(helpOption);
    }

    #endregion
}
