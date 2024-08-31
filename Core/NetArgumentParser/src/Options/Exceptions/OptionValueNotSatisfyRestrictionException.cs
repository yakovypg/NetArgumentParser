using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class OptionValueNotSatisfyRestrictionException : Exception
{
    private readonly string[]? _optionValue;

    public OptionValueNotSatisfyRestrictionException() { }

    public OptionValueNotSatisfyRestrictionException(string? message)
        : base(message) { }

    public OptionValueNotSatisfyRestrictionException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public OptionValueNotSatisfyRestrictionException(string? message, string[] optionValue)
        : this(message, optionValue, null) { }

    public OptionValueNotSatisfyRestrictionException(
        string? message,
        string[] optionValue,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(optionValue), innerException)
    {
        ArgumentNullException.ThrowIfNull(optionValue, nameof(optionValue));
        _optionValue = optionValue;
    }

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    protected OptionValueNotSatisfyRestrictionException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        _optionValue = info.GetValue(nameof(_optionValue), typeof(string[])) as string[];
    }
#pragma warning restore CS0809

    public IReadOnlyCollection<string> OptionValue => _optionValue ?? [];

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        info.AddValue(nameof(_optionValue), _optionValue, typeof(string[]));
        base.GetObjectData(info, context);
    }
#pragma warning restore CS0809

    private static string GetDefaultMessage(string[] optionValue)
    {
        ArgumentNullException.ThrowIfNull(optionValue, nameof(optionValue));

        string value = string.Join(' ', optionValue);
        return $"Option value '{value}' doesn't satisfy the restriction.";
    }
}
