using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class OptionValueNotRecognizedException : Exception
{
    private readonly string[]? _optionValue;

    public OptionValueNotRecognizedException() { }

    public OptionValueNotRecognizedException(string? message)
        : base(message) { }

    public OptionValueNotRecognizedException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public OptionValueNotRecognizedException(string? message, string[] optionValue)
        : this(message, optionValue, null) { }

    public OptionValueNotRecognizedException(
        string? message,
        string[] optionValue,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(optionValue), innerException)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionValue, nameof(optionValue));
        _optionValue = optionValue;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected OptionValueNotRecognizedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        _optionValue = info.GetValue(nameof(_optionValue), typeof(string[])) as string[];
    }

    public IReadOnlyCollection<string> OptionValue => _optionValue ?? [];

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        info.AddValue(nameof(_optionValue), _optionValue, typeof(string[]));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(string[] optionValue)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionValue, nameof(optionValue));

        string value = string.Join(" ", optionValue);
        return $"Option value '{value}' not recognized.";
    }
}
