using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class ArgumentValueNotRecognizedException : Exception
{
    public ArgumentValueNotRecognizedException() {}

    public ArgumentValueNotRecognizedException(string? message)
        : base(message) {}

    public ArgumentValueNotRecognizedException(string? message, Exception? innerException)
        : base(message, innerException) {}

    public ArgumentValueNotRecognizedException(string? message, string argument)  
        : this(message, argument, null) {}
    
    public ArgumentValueNotRecognizedException(
        string? message,
        string argument,
        Exception? innerException)  
        : base(message ?? GetDefaultMessage(argument), innerException)
    {
        ArgumentNullException.ThrowIfNull(argument, nameof(argument));
        Argument = argument;
    }

    public string? Argument { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    protected ArgumentValueNotRecognizedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        Argument = info.GetString(nameof(Argument));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        info.AddValue(nameof(Argument), Argument);
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(string argument)
    {
        ArgumentNullException.ThrowIfNull(argument, nameof(argument));
        return $"Value of the argument {argument} not recognized.";
    }
}
