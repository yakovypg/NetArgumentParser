using System;
using System.Collections.Generic;
using System.Reflection;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Context;

namespace NetArgumentParser.Attributes;

[AttributeUsage(
    AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = true)
]
public class MultipleValueOptionAttribute<T> : ValueOptionAttribute<IList<T>>
{
#pragma warning disable CA1019 // Define accessors for attribute arguments
    public MultipleValueOptionAttribute(
        T[] defaultValue,
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        bool ignoreCaseInChoices = false,
        bool ignoreOrderInChoices = false,
        string[]? aliases = null,
        ContextCaptureType contextCaptureType = ContextCaptureType.None,
        int numberOfItemsToCapture = -1)
        : base(
            defaultValue,
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            metaVariable,
            isRequired,
            isHidden,
            isFinal,
            ignoreCaseInChoices,
            aliases,
            choices: null,
            beforeParseChoices: null)
    {
        IgnoreOrderInChoices = ignoreOrderInChoices;
        ContextCapture = CreateContextCapture(contextCaptureType, numberOfItemsToCapture);
    }

    public MultipleValueOptionAttribute(
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        bool ignoreCaseInChoices = false,
        bool ignoreOrderInChoices = false,
        string[]? aliases = null,
        ContextCaptureType contextCaptureType = ContextCaptureType.None,
        int numberOfItemsToCapture = -1)
        : base(
            choices: null,
            beforeParseChoices: null,
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            metaVariable,
            isRequired,
            isHidden,
            isFinal,
            ignoreCaseInChoices,
            aliases)
    {
        IgnoreOrderInChoices = ignoreOrderInChoices;
        ContextCapture = CreateContextCapture(contextCaptureType, numberOfItemsToCapture);
    }
#pragma warning restore CA1019 // Define accessors for attribute arguments

    public bool IgnoreOrderInChoices { get; }
    public IContextCapture? ContextCapture { get; }

    protected override IReadOnlyList<Type> ValidPropertyTypes => [typeof(IList<T>)];

    public override bool CanCreateOption(object source, PropertyInfo propertyInfo)
    {
        ExtendedArgumentNullException.ThrowIfNull(source, nameof(source));
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));

        return propertyInfo.CanRead
            && propertyInfo.CanWrite
            && typeof(IList<T>).IsAssignableFrom(propertyInfo.PropertyType);
    }

    public override ICommonOption CreateOption(object source, PropertyInfo propertyInfo)
    {
        ExtendedArgumentNullException.ThrowIfNull(source, nameof(source));
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));

        if (!CanCreateOption(source, propertyInfo))
            throw new CannotCreateOptionException(null, propertyInfo);

        return new MultipleValueOption<T>(
            LongName,
            ShortName,
            Description,
            MetaVariable,
            IsRequired,
            IsHidden,
            IsFinal,
            IgnoreCaseInChoices,
            IgnoreOrderInChoices,
            Aliases,
            choices: null,
            DefaultValue,
            valueRestriction: null,
            t => propertyInfo.SetValue(source, t),
            ContextCapture);
    }

    private static IContextCapture? CreateContextCapture(
        ContextCaptureType contextCaptureType,
        int numberOfItemsToCapture)
    {
        if (contextCaptureType == ContextCaptureType.None)
            return null;

        int? itemsToCapture = numberOfItemsToCapture >= 0
            ? numberOfItemsToCapture
            : null;

        return ContextCaptureRecognizer.Recognize(
                contextCaptureType,
                itemsToCapture);
    }
}
