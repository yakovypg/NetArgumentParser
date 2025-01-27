using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options.Context;

[Serializable]
public class ContextCaptureNotRecognizedException : Exception
{
    public ContextCaptureNotRecognizedException() { }

    public ContextCaptureNotRecognizedException(string? message)
        : base(message) { }

    public ContextCaptureNotRecognizedException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public ContextCaptureNotRecognizedException(string? message, double minValue, double maxValue)
        : this(message, minValue, maxValue, null) { }

    public ContextCaptureNotRecognizedException(
        string? message,
        double minValue,
        double maxValue,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(minValue, maxValue), innerException)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected ContextCaptureNotRecognizedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        MinValue = info.GetDouble(nameof(MinValue));
        MaxValue = info.GetDouble(nameof(MaxValue));
    }

    public double MinValue { get; private set; }
    public double MaxValue { get; private set; }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        info.AddValue(nameof(MinValue), MinValue);
        info.AddValue(nameof(MaxValue), MaxValue);

        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(double minValue, double maxValue)
    {
        return $"Failed to recognize context capture from '{(minValue, maxValue)}' tuple.";
    }
}
