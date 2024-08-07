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

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    protected DefaultConverterNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        OutputType = info.GetValue(nameof(OutputType), typeof(Type)) as Type;
    }

    public Type? OutputType { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        info.AddValue(nameof(OutputType), OutputType, typeof(Type));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(Type outputType)
    {
        ArgumentNullException.ThrowIfNull(outputType, nameof(outputType));
        return $"Default converter for type '{outputType.Name}' not found.";
    }
}
