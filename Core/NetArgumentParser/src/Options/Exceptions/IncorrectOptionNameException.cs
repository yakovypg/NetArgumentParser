using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class IncorrectOptionNameException : Exception
{
    public IncorrectOptionNameException() { }

    public IncorrectOptionNameException(string? message)
        : base(message) { }

    public IncorrectOptionNameException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public IncorrectOptionNameException(string? message, string optionName)
        : this(message, optionName, null) { }

    public IncorrectOptionNameException(
        string? message,
        string optionName,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(optionName), innerException)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionName, nameof(optionName));
        OptionName = optionName;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected IncorrectOptionNameException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        OptionName = info.GetString(nameof(OptionName));
    }

    public string? OptionName { get; private set; }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        ExtendedArgumentNullException.ThrowIfNull(context, nameof(context));

        info.AddValue(nameof(OptionName), OptionName, typeof(string));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(string optionName)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionName, nameof(optionName));
        return $"Option name '{optionName}' is not correct.";
    }
}
