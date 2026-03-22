using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using NetArgumentParser.Attributes;

namespace NetArgumentParser.Generators;

[Serializable]
public class UnsupportedParserConfigException : Exception
{
    public UnsupportedParserConfigException()
        : this(GetDefaultMessage()) { }

    public UnsupportedParserConfigException(string? message)
        : base(message ?? GetDefaultMessage()) { }

    public UnsupportedParserConfigException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException) { }

    public UnsupportedParserConfigException(string? message, object config)
        : this(message ?? GetDefaultMessage(config), config, null) { }

    public UnsupportedParserConfigException(
        string? message,
        object config,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(config), innerException)
    {
        ExtendedArgumentNullException.ThrowIfNull(config, nameof(config));
        Config = config;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected UnsupportedParserConfigException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        Config = info.GetValue(nameof(Config), typeof(object));
    }

    public object? Config { get; private set; }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        info.AddValue(nameof(Config), Config, typeof(object));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(object? config = null)
    {
        string? configObjectName = config?.GetType().FullName;
        string? necessaryAttributeName = typeof(ParserConfigAttribute).FullName;

        string configObjectNamePresenter = !string.IsNullOrEmpty(configObjectName)
            ? $" '{configObjectName}'"
            : string.Empty;

        string message = $"Config{configObjectNamePresenter} isn't supported.";

        return !string.IsNullOrEmpty(necessaryAttributeName)
            ? $"{message} It must be marked by {necessaryAttributeName} attribute."
            : message;
    }
}
