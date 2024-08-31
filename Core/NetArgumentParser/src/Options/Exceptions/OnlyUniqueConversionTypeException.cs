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

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    protected OnlyUniqueConversionTypeException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        ConversionType = info.GetValue(nameof(ConversionType), typeof(Type)) as Type;
    }
#pragma warning restore CS0809

    public Type? ConversionType { get; private set; }

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        ExtendedArgumentNullException.ThrowIfNull(context, nameof(context));

        info.AddValue(nameof(ConversionType), ConversionType, typeof(Type));
        base.GetObjectData(info, context);
    }
#pragma warning restore CS0809

    private static string GetDefaultMessage(Type conversionType)
    {
        ExtendedArgumentNullException.ThrowIfNull(conversionType, nameof(conversionType));
        return $"Conversion type '{conversionType.Name}' is already in use.";
    }
}
