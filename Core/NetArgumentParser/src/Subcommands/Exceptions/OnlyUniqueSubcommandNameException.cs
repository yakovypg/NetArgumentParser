using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Subcommands;

[Serializable]
public class OnlyUniqueSubcommandNameException : Exception
{
    public OnlyUniqueSubcommandNameException() { }

    public OnlyUniqueSubcommandNameException(string? message)
        : base(message) { }

    public OnlyUniqueSubcommandNameException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public OnlyUniqueSubcommandNameException(string? message, string subcommandName)
        : this(message, subcommandName, null) { }

    public OnlyUniqueSubcommandNameException(
        string? message,
        string subcommandName,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(subcommandName), innerException)
    {
        ArgumentNullException.ThrowIfNull(subcommandName, nameof(subcommandName));
        SubcommandName = subcommandName;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    protected OnlyUniqueSubcommandNameException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        SubcommandName = info.GetString(nameof(SubcommandName));
    }

    public string? SubcommandName { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        info.AddValue(nameof(SubcommandName), SubcommandName);
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(string subcommandName)
    {
        ArgumentNullException.ThrowIfNull(subcommandName, nameof(subcommandName));
        return $"Subcommand name '{subcommandName}' is already in use.";
    }
}
