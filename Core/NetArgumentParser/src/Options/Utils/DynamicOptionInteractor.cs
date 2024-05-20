using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetArgumentParser.Converters;

namespace NetArgumentParser.Options.Utils;

internal static class DynamicOptionInteractor
{
    private const string _converterPropertyName = "Converter";
    private const string _setConverterMethodName = "TrySetConverter";
    private const string _hasDefaultValuePropertyName = "HasDefaultValue";
    private const string _handleDefaultValueMethodName = "HandleDefaultValue";
    
    internal static void HandleDefaultValueBySuitableOptions(IEnumerable<ICommonOption> options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        IEnumerable<ICommonOption> unhandledOptions = options
            .Where(t => !t.IsHandled); 

        IEnumerable<dynamic> optionsWithAbilityToHandleDefaultValue =
            GetOptionsWithAbilityToHandleDefaultValue(unhandledOptions);
        
        IEnumerable<dynamic> suitableOptions = optionsWithAbilityToHandleDefaultValue
            .Where(t => t.HasDefaultValue);

        foreach (dynamic option in suitableOptions)
        {
            option.HandleDefaultValue();
        }
    }
    
    internal static void SetConverterToSuitableOptions(
        IEnumerable<ICommonOption> options,
        IValueConverter converter)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(converter, nameof(converter));

        Type converterType = converter.GetType();

        IEnumerable<dynamic> optionsWithAbilityToSetConverter =
            GetOptionsWithAbilityToSetConverter(options, converterType);

        IEnumerable<dynamic> suitableOptions = optionsWithAbilityToSetConverter
            .Where(t => t.Converter is null);

        foreach (dynamic option in suitableOptions)
        {
            option.TrySetConverter(converter);
        }
    }

    private static IEnumerable<dynamic> GetOptionsWithAbilityToSetConverter(
        IEnumerable<ICommonOption> options,
        Type converterType)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(converterType, nameof(converterType));

        return options
            .Where(t => CheckOptionHasAbilityToSetConverter(t, converterType))
            .Cast<dynamic>();
    }

    private static bool CheckOptionHasAbilityToSetConverter(ICommonOption option, Type converterType)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));
        ArgumentNullException.ThrowIfNull(converterType, nameof(converterType));
        
        return CheckOptionHasConverterSetMethod(option, converterType)
            && CheckOptionHasConverterProperty(option, converterType);
    }

    private static bool CheckOptionHasConverterSetMethod(ICommonOption option, Type converterType)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));
        ArgumentNullException.ThrowIfNull(converterType, nameof(converterType));

        Type optionType = option.GetType();
        MethodInfo? setterMethod = optionType.GetMethod(_setConverterMethodName);

        if (setterMethod is null)
            return false;

        ParameterInfo[] parameters = setterMethod.GetParameters();

        if (parameters.Length != 1)
            return false;

        Type firstParameterType = parameters[0].ParameterType;

        return firstParameterType.IsAssignableFrom(converterType);
    }

    private static bool CheckOptionHasConverterProperty(ICommonOption option, Type converterType)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));
        ArgumentNullException.ThrowIfNull(converterType, nameof(converterType));

        Type optionType = option.GetType();
        PropertyInfo? property = optionType.GetProperty(_converterPropertyName);

        if (property is null)
            return false;
        
        MethodInfo? propertyGetter = property.GetMethod;

        return propertyGetter is not null
            && propertyGetter.IsPublic
            && property.PropertyType.IsAssignableFrom(converterType);
    }

    private static IEnumerable<dynamic> GetOptionsWithAbilityToHandleDefaultValue(
        IEnumerable<ICommonOption> options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        return options
            .Where(CheckOptionHasAbilityToHandleDefaultValue)
            .Cast<dynamic>();
    }

    private static bool CheckOptionHasAbilityToHandleDefaultValue(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        return CheckOptionHasDefaultValueHandler(option)
            && CheckOptionHasDefaultValueCheckerProperty(option);
    }

    private static bool CheckOptionHasDefaultValueHandler(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        Type optionType = option.GetType();
        MethodInfo? handleMethod = optionType.GetMethod(_handleDefaultValueMethodName);

        return handleMethod is not null
            && handleMethod.GetParameters().Length == 0;
    }

    private static bool CheckOptionHasDefaultValueCheckerProperty(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        Type optionType = option.GetType();
        PropertyInfo? property = optionType.GetProperty(_hasDefaultValuePropertyName);

        if (property is null)
            return false;
        
        MethodInfo? propertyGetter = property.GetMethod;

        return propertyGetter is not null
            && propertyGetter.IsPublic
            && propertyGetter.ReturnParameter.ParameterType == typeof(bool);
    }
}
