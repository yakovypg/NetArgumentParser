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
        ExtendedArgumentNullException.ThrowIfNull(arguments, nameof(arguments));
        _arguments = arguments;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected ArgumentsAreUnknownException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        _arguments = info.GetValue(nameof(_arguments), typeof(string[])) as string[];
    }

    public IReadOnlyCollection<string> Arguments => _arguments ?? [];

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        info.AddValue(nameof(_arguments), _arguments, typeof(string[]));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(string[] arguments)
    {
        ExtendedArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        string argumentsPresenter = string.Join(" ", arguments);
        return $"Arguments {argumentsPresenter} are unknown.";
    }
}
