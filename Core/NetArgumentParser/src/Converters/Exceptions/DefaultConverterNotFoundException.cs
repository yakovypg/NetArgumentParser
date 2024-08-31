using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Converters;

[Serializable]
public class DefaultConverterNotFoundException : Exception
{
    public DefaultConverterNotFoundException() { }

    public DefaultConverterNotFoundException(string? message)
        : base(message) { }

    public DefaultConverterNotFoundException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public DefaultConverterNotFoundException(string? message, Type outputType)
        : this(message, outputType, null) { }

    public DefaultConverterNotFoundException(
        string? message,
        Type outputType,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(outputType), innerException)
    {
        ArgumentNullException.ThrowIfNull(outputType, nameof(outputType));
        OutputType = outputType;
    }

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    protected DefaultConverterNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        OutputType = info.GetValue(nameof(OutputType), typeof(Type)) as Type;
    }
#pragma warning restore CS0809

    public Type? OutputType { get; private set; }

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

        info.AddValue(nameof(OutputType), OutputType, typeof(Type));
        base.GetObjectData(info, context);
    }
#pragma warning restore CS0809

    private static string GetDefaultMessage(Type outputType)
    {
        ArgumentNullException.ThrowIfNull(outputType, nameof(outputType));
        return $"Default converter for type '{outputType.Name}' not found.";
    }
}
