using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Subcommands;

[Serializable]
public class IncorrectSubcommandNameException : Exception
{
    public IncorrectSubcommandNameException() { }

    public IncorrectSubcommandNameException(string? message)
        : base(message) { }

    public IncorrectSubcommandNameException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public IncorrectSubcommandNameException(string? message, string subcommandName)
        : this(message, subcommandName, null) { }

    public IncorrectSubcommandNameException(
        string? message,
        string subcommandName,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(subcommandName), innerException)
    {
        ArgumentNullException.ThrowIfNull(subcommandName, nameof(subcommandName));
        SubcommandName = subcommandName;
    }

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    protected IncorrectSubcommandNameException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        SubcommandName = info.GetString(nameof(SubcommandName));
    }
#pragma warning restore CS0809

    public string? SubcommandName { get; private set; }

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

        info.AddValue(nameof(SubcommandName), SubcommandName, typeof(string));
        base.GetObjectData(info, context);
    }
#pragma warning restore CS0809

    private static string GetDefaultMessage(string subcommandName)
    {
        ArgumentNullException.ThrowIfNull(subcommandName, nameof(subcommandName));
        return $"Subcommand name '{subcommandName}' is not correct.";
    }
}
