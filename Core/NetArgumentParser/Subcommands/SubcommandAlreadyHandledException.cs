using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Subcommands;

[Serializable]
public class SubcommandAlreadyHandledException : Exception
{
    public SubcommandAlreadyHandledException()
        : this(GetDefaultMessage()) { }

    public SubcommandAlreadyHandledException(string? message)
        : base(message ?? GetDefaultMessage()) { }

    public SubcommandAlreadyHandledException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException) { }

    public SubcommandAlreadyHandledException(string? message, ISubcommand subcommand)
        : this(message ?? GetDefaultMessage(subcommand), subcommand, null) { }

    public SubcommandAlreadyHandledException(
        string? message,
        ISubcommand subcommand,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(subcommand), innerException)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));
        Subcommand = subcommand;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected SubcommandAlreadyHandledException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        Subcommand = info.GetValue(nameof(Subcommand), typeof(ISubcommand)) as ISubcommand;
    }

    public ISubcommand? Subcommand { get; private set; }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        info.AddValue(nameof(Subcommand), Subcommand, typeof(ISubcommand));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(ISubcommand? subcommand = null)
    {
        string subcommandPresenter = subcommand is not null
            ? $" '{subcommand}'"
            : string.Empty;

        return $"Subcommand{subcommandPresenter} has already been handled.";
    }
}
