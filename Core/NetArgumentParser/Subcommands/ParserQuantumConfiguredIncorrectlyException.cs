using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Subcommands;

[Serializable]
public class ParserQuantumConfiguredIncorrectlyException : Exception
{
    public ParserQuantumConfiguredIncorrectlyException()
        : this(GetDefaultMessage()) { }

    public ParserQuantumConfiguredIncorrectlyException(string? message)
        : base(message ?? GetDefaultMessage()) { }

    public ParserQuantumConfiguredIncorrectlyException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException) { }

    public ParserQuantumConfiguredIncorrectlyException(string? message, string parserQuantumName)
        : this(message ?? GetDefaultMessage(parserQuantumName), parserQuantumName, null) { }

    public ParserQuantumConfiguredIncorrectlyException(
        string? message,
        string parserQuantumName,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(parserQuantumName), innerException)
    {
        ParserQuantumName = parserQuantumName;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    private ParserQuantumConfiguredIncorrectlyException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        ParserQuantumName = info.GetString(nameof(ParserQuantumName));
    }

    public string? ParserQuantumName { get; private set; }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        info.AddValue(nameof(ParserQuantumName), ParserQuantumName, typeof(string));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(string? parserQuantumName = null)
    {
        if (!string.IsNullOrEmpty(parserQuantumName))
            parserQuantumName = $" {parserQuantumName}";

        return $"Parser quantum{parserQuantumName} is configured incorrectly.";
    }
}
