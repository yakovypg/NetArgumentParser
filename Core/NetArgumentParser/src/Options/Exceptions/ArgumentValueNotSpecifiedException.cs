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
        ArgumentNullException.ThrowIfNull(argument, nameof(argument));
        Argument = argument;
    }

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    protected ArgumentValueNotSpecifiedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        Argument = info.GetString(nameof(Argument));
    }
#pragma warning restore CS0809

    public string? Argument { get; private set; }

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

        info.AddValue(nameof(Argument), Argument, typeof(string));
        base.GetObjectData(info, context);
    }
#pragma warning restore CS0809

    private static string GetDefaultMessage(string argument)
    {
        ArgumentNullException.ThrowIfNull(argument, nameof(argument));
        return $"Argument {argument} does not have value.";
    }
}
