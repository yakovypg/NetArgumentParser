using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class ChoicesAlreadyAddedToDescriptionException : Exception
{
    public ChoicesAlreadyAddedToDescriptionException() { }

    public ChoicesAlreadyAddedToDescriptionException(string? message)
        : base(message) { }

    public ChoicesAlreadyAddedToDescriptionException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException) { }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected ChoicesAlreadyAddedToDescriptionException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }

    private static string GetDefaultMessage()
    {
        return "Choices have already been added to description.";
    }
}
