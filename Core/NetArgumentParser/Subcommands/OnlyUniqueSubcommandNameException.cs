using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Subcommands;

[Serializable]
public class OnlyUniqueSubcommandNameException : Exception
{
    public OnlyUniqueSubcommandNameException()
        : this(GetDefaultMessage()) { }

    public OnlyUniqueSubcommandNameException(string? message)
        : base(message ?? GetDefaultMessage()) { }

    public OnlyUniqueSubcommandNameException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException) { }

    public OnlyUniqueSubcommandNameException(string? message, string subcommandName)
        : this(message ?? GetDefaultMessage(subcommandName), subcommandName, null) { }

    public OnlyUniqueSubcommandNameException(
        string? message,
        string subcommandName,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(subcommandName), innerException)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommandName, nameof(subcommandName));
        SubcommandName = subcommandName;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected OnlyUniqueSubcommandNameException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        SubcommandName = info.GetString(nameof(SubcommandName));
    }

    public string? SubcommandName { get; private set; }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        info.AddValue(nameof(SubcommandName), SubcommandName, typeof(string));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(string? subcommandName = null)
    {
        if (!string.IsNullOrEmpty(subcommandName))
            subcommandName = $" '{subcommandName}'";

        return $"Subcommand name{subcommandName} is already in use.";
    }
}
