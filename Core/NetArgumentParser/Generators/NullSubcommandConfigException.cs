using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace NetArgumentParser.Generators;

[Serializable]
public class NullSubcommandConfigException : Exception
{
    public NullSubcommandConfigException() { }

    public NullSubcommandConfigException(string? message)
        : base(message) { }

    public NullSubcommandConfigException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public NullSubcommandConfigException(string? message, PropertyInfo subcommandConfig)
        : this(message, subcommandConfig, null) { }

    public NullSubcommandConfigException(
        string? message,
        PropertyInfo subcommandConfig,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(subcommandConfig), innerException)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommandConfig, nameof(subcommandConfig));
        SubcommandConfig = subcommandConfig;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected NullSubcommandConfigException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        SubcommandConfig = info.GetValue(
            nameof(SubcommandConfig),
            typeof(PropertyInfo)) as PropertyInfo;
    }

    public PropertyInfo? SubcommandConfig { get; private set; }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        info.AddValue(nameof(SubcommandConfig), SubcommandConfig, typeof(PropertyInfo));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(PropertyInfo subcommandConfig)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommandConfig, nameof(subcommandConfig));
        return $"Subcommand configuration '{subcommandConfig.Name}' is null.";
    }
}
