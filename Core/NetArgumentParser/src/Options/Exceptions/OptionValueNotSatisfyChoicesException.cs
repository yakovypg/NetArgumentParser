using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class OptionValueNotSatisfyChoicesException : Exception
{
    private readonly string[]? _optionValue;
    private readonly string[]? _allowedValues;

    public OptionValueNotSatisfyChoicesException() { }

    public OptionValueNotSatisfyChoicesException(string? message)
        : base(message) { }

    public OptionValueNotSatisfyChoicesException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public OptionValueNotSatisfyChoicesException(
        string? message,
        string[] optionValue,
        string[] allowedValues)
        : this(message, optionValue, allowedValues, null) { }

    public OptionValueNotSatisfyChoicesException(
        string? message,
        string[] optionValue,
        string[] allowedValues,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(optionValue, allowedValues), innerException)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionValue, nameof(optionValue));
        ExtendedArgumentNullException.ThrowIfNull(allowedValues, nameof(allowedValues));

        _optionValue = optionValue;
        _allowedValues = allowedValues;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected OptionValueNotSatisfyChoicesException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        _optionValue = info.GetValue(nameof(_optionValue), typeof(string[])) as string[];
        _allowedValues = info.GetValue(nameof(_allowedValues), typeof(string[])) as string[];
    }

    public IReadOnlyCollection<string> OptionValue => _optionValue ?? [];
    public IReadOnlyCollection<string> AllowedValues => _allowedValues ?? [];

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        info.AddValue(nameof(_optionValue), _optionValue, typeof(string[]));
        info.AddValue(nameof(_allowedValues), _allowedValues, typeof(string[]));

        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(string[] optionValue, string[] allowedValues)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionValue, nameof(optionValue));
        ExtendedArgumentNullException.ThrowIfNull(allowedValues, nameof(allowedValues));

        string optionValuePresenter = string.Join(", ", optionValue);
        string allowedValuesPresenter = string.Join(", ", allowedValues);

        return $"Option value '{optionValuePresenter}' not allowed. " +
               "It must be one of {" + allowedValuesPresenter + "}.";
    }
}
