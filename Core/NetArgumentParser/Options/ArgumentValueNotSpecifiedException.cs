using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class ArgumentValueNotSpecifiedException : Exception
{
    public ArgumentValueNotSpecifiedException() { }

    public ArgumentValueNotSpecifiedException(string? message)
        : base(message) { }

    public ArgumentValueNotSpecifiedException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public ArgumentValueNotSpecifiedException(string? message, string argument)
        : this(message, argument, null) { }

    public ArgumentValueNotSpecifiedException(
        string? message,
        string argument,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(argument), innerException)
    {
        ExtendedArgumentNullException.ThrowIfNull(argument, nameof(argument));
        Argument = argument;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected ArgumentValueNotSpecifiedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        Argument = info.GetString(nameof(Argument));
    }

    public string? Argument { get; private set; }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        info.AddValue(nameof(Argument), Argument, typeof(string));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(string argument)
    {
        ExtendedArgumentNullException.ThrowIfNull(argument, nameof(argument));
        return $"Argument {argument} does not have value.";
    }
}
