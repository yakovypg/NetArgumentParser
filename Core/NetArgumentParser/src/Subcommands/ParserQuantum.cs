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
    private readonly SubcommandNameUniquenessVerifier _nameUniquenessVerifier;

    private ITextWriter? _outputWriter;

    public ParserQuantum()
    {
        _optionSet = new OptionSet();
        _optionGroups = [new OptionGroup<ICommonOption>("Options:", _optionSet)];
        _subcommands = [];
        _nameUniquenessVerifier = new SubcommandNameUniquenessVerifier(_subcommands);

        UseDefaultHelpOption = true;
        UsageStartTerm = string.Empty;
    }

    public string UsageStartTerm { get; set; }
    public bool UseDefaultHelpOption { get; set; }

    public IDescriptionGenerator? DescriptionGenerator { get; set; }
    public Func<Subcommand, IDescriptionGenerator>? SubcommandDescriptionGeneratorCreator { get; set; }

    public IReadOnlyList<ICommonOption> Options => _optionSet.Options;
    public IEnumerable<ICommonOption> HiddenOptions => Options.Where(t => t.IsHidden);
    public IEnumerable<ICommonOption> VisibleOptions => Options.Where(t => !t.IsHidden);

    public IReadOnlyList<OptionGroup<ICommonOption>> OptionGroups => _optionGroups;
    public OptionGroup<ICommonOption> DefaultGroup => _optionGroups[0];

    public IReadOnlyList<Subcommand> Subcommands => _subcommands;
    public IReadOnlyOptionSet<ICommonOption> OptionSet => _optionSet;

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

    public virtual bool RemoveOption(ICommonOption commonOption)
    {
        ArgumentNullException.ThrowIfNull(commonOption, nameof(commonOption));
        return _optionSet.RemoveOption(commonOption);
    }

    public void AddConverters(params IValueConverter[] converters)
    {
        ArgumentNullException.ThrowIfNull(converters, nameof(converters));
        Array.ForEach(converters, _optionSet.AddConverter);
    }

    public virtual bool RemoveConverter(IValueConverter converter)
    {
        ArgumentNullException.ThrowIfNull(converter, nameof(converter));
        return _optionSet.RemoveConverter(converter);
    }

    public OptionGroup<ICommonOption> AddOptionGroup(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        var group = new OptionGroup<ICommonOption>(name, _optionSet);
        _optionGroups.Add(group);

        return group;
    }

    public bool RemoveOptionGroup(OptionGroup<ICommonOption> group)
    {
        ArgumentNullException.ThrowIfNull(group, nameof(group));
        return _optionGroups.Remove(group);
    }

    public IList<ICommonOption> GetAllOptions()
    {
        var options = new List<ICommonOption>(Options);

        foreach (Subcommand subcommand in _subcommands)
        {
            IList<ICommonOption> currOptions = subcommand.GetAllOptions();
            options.AddRange(currOptions);
        }

        return options;
    }

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
}
