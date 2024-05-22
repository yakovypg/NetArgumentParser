using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class ArgumentsAreUnknownException : Exception
{   
    public ArgumentsAreUnknownException() {}

    public ArgumentsAreUnknownException(string? message)
        : base(message) {}

    public ArgumentsAreUnknownException(string? message, Exception? innerException)
        : base(message, innerException) {}

    public ArgumentsAreUnknownException(string? message, string[] arguments)  
        : this(message, arguments, null) {}
    
    public ArgumentsAreUnknownException(
        string? message,
        string[] arguments,
        Exception? innerException)  
        : base(message ?? GetDefaultMessage(arguments), innerException)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));
        Arguments = arguments;
    }

    public string[]? Arguments { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    protected ArgumentsAreUnknownException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        Arguments = info.GetValue(nameof(Arguments), typeof(string[])) as string[];
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        info.AddValue(nameof(Arguments), Arguments, typeof(string[]));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(string[] arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));
        return $"Arguments {string.Join(' ', arguments)} are unknown.";
    }
}
