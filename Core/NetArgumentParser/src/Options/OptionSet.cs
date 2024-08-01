using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Converters;
using NetArgumentParser.Options.Utils;
using NetArgumentParser.Options.Utils.Verifiers;

namespace NetArgumentParser.Options;

public class OptionSet : IBuildableOptionSet<ICommonOption>
{
    private readonly List<ICommonOption> _options;
    private readonly List<IValueConverter> _converters;
    private readonly OptionNameUniquenessVerifier _optionNameUniquenessVerifier;
    private readonly ConversionTypeUniquenessVerifier _conversionTypeUniquenessVerifier;

    public OptionSet(
        IEnumerable<ICommonOption>? options = null,
        IEnumerable<IValueConverter>? converters = null,
        bool automaticBuild = true)
    {   
        _options = [];
        _converters = [];
        _optionNameUniquenessVerifier = new(_options);
        _conversionTypeUniquenessVerifier = new(_converters);

        AutomaticBuild = automaticBuild;
        CollectionChanged += (sender, e) => IsBuilt = false;

        if (options is not null) AddOptions(options);
        if (converters is not null) AddConverters(converters);
    }

    public event EventHandler<NotifyOptionSetChangedEventArgs>? CollectionChanged;

    public bool IsBuilt { get; protected set; }
    public bool AutomaticBuild { get; init; }

    public IReadOnlyList<ICommonOption> Options => _options;
    public IReadOnlyList<IValueConverter> Converters => _converters;

    public virtual void Build()
    {       
        foreach (IValueConverter converter in _converters)
        {
            DynamicOptionInteractor.SetConverterToSuitableOptions(_options, converter);
        }

        IsBuilt = true;
    }

    public virtual ICommonOption GetOption(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        if (!IsBuilt)
        {
            if (AutomaticBuild) Build();
            else throw new OptionSetNotBuiltException();
        }
        
        ICommonOption? foundOption = GetOptionByName(name);

        return foundOption is not null
            ? foundOption
            : throw new OptionNotFoundException(null, name);
    }

    public bool HasHelpOption()
    {
        return _options.Any(t => t is HelpOption);
    }

    public bool HasVersionOption()
    {
        return _options.Any(t => t is VersionOption);
    }

    public bool HasOption(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        return GetOptionByName(name) is not null;
    }

    public bool HasConverter(Type conversionType)
    {
        ArgumentNullException.ThrowIfNull(conversionType, nameof(conversionType));
        return GetConverterByConversionType(conversionType) is not null;
    }

    public void ResetOptionsHandledState()
    {
        foreach (ICommonOption option in _options)
        {
            option.ResetHandledState();
        }

        var e = new NotifyOptionSetChangedEventArgs(
            NotifyOptionSetChangedAction.Reset,
            null,
            null);

        OnCollectionChanged(e);
    }

    public void AddOptions(IEnumerable<ICommonOption> options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        foreach (ICommonOption option in options)
        {
            AddOption(option);
        }
    }
    
    public void AddConverters(IEnumerable<IValueConverter> converters)
    {
        ArgumentNullException.ThrowIfNull(converters, nameof(converters));

        foreach (IValueConverter converter in converters)
        {
            AddConverter(converter);
        }
    }

    public void AddOption(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        _optionNameUniquenessVerifier.VerifyNamesIsUnique(option);
        _options.Add(option);

        var e = new NotifyOptionSetChangedEventArgs(
            NotifyOptionSetChangedAction.AddOption,
            new List<ICommonOption>() { option },
            null);

        OnCollectionChanged(e);
    }

    public void AddConverter(IValueConverter converter)
    {
        ArgumentNullException.ThrowIfNull(converter, nameof(converter));

        _conversionTypeUniquenessVerifier.VerifyConversionTypeIsUnique(converter);
        _converters.Add(converter);

        var e = new NotifyOptionSetChangedEventArgs(
            NotifyOptionSetChangedAction.AddConverter,
            new List<IValueConverter>() { converter },
            null);

        OnCollectionChanged(e);
    }

    public bool RemoveOption(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        bool isRemoved = _options.Remove(option);
        
        if (isRemoved)
        {
            var e = new NotifyOptionSetChangedEventArgs(
                NotifyOptionSetChangedAction.RemoveOption,
                null,
                new List<ICommonOption>() { option });

            OnCollectionChanged(e);
        }

        return isRemoved;
    }

    public bool RemoveConverter(IValueConverter converter)
    {
        ArgumentNullException.ThrowIfNull(converter, nameof(converter));

        bool isRemoved = _converters.Remove(converter);
        
        if (isRemoved)
        {
            var e = new NotifyOptionSetChangedEventArgs(
                NotifyOptionSetChangedAction.RemoveConverter,
                null,
                new List<IValueConverter>() { converter });

            OnCollectionChanged(e);
        }

        return isRemoved;
    }

    protected ICommonOption? GetOptionByName(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        return _options.FirstOrDefault(t => t.HasName(name));
    }

    protected IValueConverter? GetConverterByConversionType(Type conversionType)
    {
        ArgumentNullException.ThrowIfNull(conversionType, nameof(conversionType));
        return _converters.FirstOrDefault(t => t.ConversionType == conversionType);
    }

    protected virtual void OnCollectionChanged(NotifyOptionSetChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));
        CollectionChanged?.Invoke(this, e);
    }
}
