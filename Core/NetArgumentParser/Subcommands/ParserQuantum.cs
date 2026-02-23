using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Converters;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Collections;
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
        _optionGroups = [new OptionGroup<ICommonOption>("Options:", string.Empty, _optionSet)];
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

    public virtual void ResetOptionsHandledState(bool recursive = true)
    {
        _optionSet.ResetOptionsHandledState();

        if (recursive)
        {
            foreach (Subcommand subcommand in _subcommands)
            {
                subcommand.ResetOptionsHandledState(recursive);
            }
        }
    }

    public virtual void ResetSubcommandsHandledState(bool recursive = true)
    {
        foreach (Subcommand subcommand in _subcommands)
        {
            subcommand.ResetHandledState();

            if (recursive)
                subcommand.ResetSubcommandsHandledState(recursive);
        }
    }

    public virtual void AddOptions(params ICommonOption[] options)
    {
        ExtendedArgumentNullException.ThrowIfNull(options, nameof(options));
        DefaultGroup.AddOptions(options);
    }

    public virtual bool RemoveOption(ICommonOption commonOption)
    {
        ExtendedArgumentNullException.ThrowIfNull(commonOption, nameof(commonOption));
        return _optionSet.RemoveOption(commonOption);
    }

    public virtual void AddConverters(params IValueConverter[] converters)
    {
        ExtendedArgumentNullException.ThrowIfNull(converters, nameof(converters));
        Array.ForEach(converters, _optionSet.AddConverter);
    }

    public virtual bool RemoveConverter(IValueConverter converter)
    {
        ExtendedArgumentNullException.ThrowIfNull(converter, nameof(converter));
        return _optionSet.RemoveConverter(converter);
    }

    public virtual void AddConverters(bool includeAllSubcommands, params IValueConverter[] converters)
    {
        ExtendedArgumentNullException.ThrowIfNull(converters, nameof(converters));

        AddConverters(converters);

        if (includeAllSubcommands)
        {
            foreach (Subcommand subcommand in Subcommands)
            {
                subcommand.AddConverters(includeAllSubcommands, converters);
            }
        }
    }

    public OptionGroup<ICommonOption> AddOptionGroup(string name, string? description = null)
    {
        ExtendedArgumentNullException.ThrowIfNull(name, nameof(name));

        description ??= string.Empty;

        var group = new OptionGroup<ICommonOption>(name, description, _optionSet);
        _optionGroups.Add(group);

        return group;
    }

    public bool RemoveOptionGroup(OptionGroup<ICommonOption> group)
    {
        ExtendedArgumentNullException.ThrowIfNull(group, nameof(group));
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

    public IList<Subcommand> GetAllSubcommands()
    {
        var subcommands = new List<Subcommand>(Subcommands);

        foreach (Subcommand subcommand in _subcommands)
        {
            IList<Subcommand> currSubcommands = subcommand.GetAllSubcommands();
            subcommands.AddRange(currSubcommands);
        }

        return subcommands;
    }

    public Subcommand AddSubcommand(string name, string description)
    {
        ExtendedArgumentNullException.ThrowIfNull(name, nameof(name));
        ExtendedArgumentNullException.ThrowIfNull(description, nameof(description));

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
        ExtendedArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));
        return _subcommands.Remove(subcommand);
    }

    public IEnumerable<ICommonOption> FindOptions(
        Predicate<ICommonOption> predicate,
        bool recursive = true)
    {
        IEnumerable<ICommonOption> foundOptions = Options.Where(t => predicate(t));

        if (!recursive)
            return foundOptions;

        IEnumerable<ICommonOption> recursiveFoundOptions = Subcommands
            .SelectMany(t => t.FindOptions(predicate, recursive));

        return foundOptions.Concat(recursiveFoundOptions);
    }

    public bool FindFirstOptionByLongName(string longName, bool recursive, out ICommonOption? foundOption)
    {
        IEnumerable<ICommonOption> foundOptions = FindOptions(t => t.LongName == longName, recursive);
        foundOption = foundOptions.FirstOrDefault();

        return foundOption is not null;
    }

    public bool FindFirstValueOptionByLongName<T>(
        string longName,
        bool recursive,
        out IValueOption<T>? foundOption)
    {
        IEnumerable<ICommonOption> foundOptions = FindOptions(
            t => t.LongName == longName && t is IValueOption<T>,
            recursive);

        foundOption = foundOptions.FirstOrDefault() as IValueOption<T>;

        return foundOption is not null;
    }

    protected virtual void AddDefaultOptions()
    {
        foreach (ParserQuantum quantum in _subcommands.Concat([this]))
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
