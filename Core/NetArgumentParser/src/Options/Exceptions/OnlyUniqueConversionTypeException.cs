using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class OnlyUniqueConversionTypeException : Exception
{
    public OnlyUniqueConversionTypeException() { }

    public OnlyUniqueConversionTypeException(string? message)
        : base(message) { }

    public OnlyUniqueConversionTypeException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public OnlyUniqueConversionTypeException(string? message, Type outputType)
        : this(message, outputType, null) { }

    public OnlyUniqueConversionTypeException(
        string? message,
        Type conversionType,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(conversionType), innerException)
    {
        ExtendedArgumentNullException.ThrowIfNull(conversionType, nameof(conversionType));
        ConversionType = conversionType;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected OnlyUniqueConversionTypeException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        ConversionType = info.GetValue(nameof(ConversionType), typeof(Type)) as Type;
    }

    public Type? ConversionType { get; private set; }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        ExtendedArgumentNullException.ThrowIfNull(context, nameof(context));

        info.AddValue(nameof(ConversionType), ConversionType, typeof(Type));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(Type conversionType)
    {
        ExtendedArgumentNullException.ThrowIfNull(conversionType, nameof(conversionType));
        return $"Conversion type '{conversionType.Name}' is already in use.";
    }
}
