using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class ArgumentsAreUnknownException : Exception
{
    private readonly string[]? _arguments;

    public ArgumentsAreUnknownException() { }

    public ArgumentsAreUnknownException(string? message)
        : base(message) { }

    public ArgumentsAreUnknownException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public ArgumentsAreUnknownException(string? message, string[] arguments)
        : this(message, arguments, null) { }

    public ArgumentsAreUnknownException(
        string? message,
        string[] arguments,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(arguments), innerException)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));
        _arguments = arguments;
    }

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    protected ArgumentsAreUnknownException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        _arguments = info.GetValue(nameof(_arguments), typeof(string[])) as string[];
    }
#pragma warning restore CS0809

    public IReadOnlyCollection<string> Arguments => _arguments ?? [];

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

        info.AddValue(nameof(_arguments), _arguments, typeof(string[]));
        base.GetObjectData(info, context);
    }
#pragma warning restore CS0809

    private static string GetDefaultMessage(string[] arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        string argumentsPresenter = string.Join(' ', arguments);
        return $"Arguments {argumentsPresenter} are unknown.";
    }
}
