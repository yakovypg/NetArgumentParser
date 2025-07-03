using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace NetArgumentParser.Generators;

[Serializable]
public class CannotCreateOptionException : Exception
{
    public CannotCreateOptionException() { }

    public CannotCreateOptionException(string? message)
        : base(message) { }

    public CannotCreateOptionException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public CannotCreateOptionException(string? message, PropertyInfo optionConfig)
        : this(message, optionConfig, null) { }

    public CannotCreateOptionException(
        string? message,
        PropertyInfo optionConfig,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(optionConfig), innerException)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionConfig, nameof(optionConfig));
        OptionConfig = optionConfig;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected CannotCreateOptionException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        OptionConfig = info.GetValue(nameof(OptionConfig), typeof(PropertyInfo)) as PropertyInfo;
    }

    public PropertyInfo? OptionConfig { get; private set; }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        info.AddValue(nameof(OptionConfig), OptionConfig, typeof(PropertyInfo));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(PropertyInfo optionConfig)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionConfig, nameof(optionConfig));
        return $"Cannot create option using '{optionConfig.Name}' configuration.";
    }
}
