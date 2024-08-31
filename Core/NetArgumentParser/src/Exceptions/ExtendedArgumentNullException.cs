using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser;

[Serializable]
public class ExtendedArgumentNullException : ArgumentNullException
{
    public ExtendedArgumentNullException() { }

    public ExtendedArgumentNullException(string? paramName)
        : base(paramName) { }

    public ExtendedArgumentNullException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public ExtendedArgumentNullException(string? paramName, string? message)
        : base(paramName, message) { }

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    protected ExtendedArgumentNullException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
#pragma warning restore CS0809

    public static void ThrowIfNull<T>(T? argument, string? paramName = null)
    {
        if (argument is null)
            throw new ArgumentNullException(paramName);
    }

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ThrowIfNull(info, nameof(info));
        base.GetObjectData(info, context);
    }
#pragma warning restore CS0809
}
