using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class OptionSetNotBuiltException : Exception
{
    public OptionSetNotBuiltException() { }

    public OptionSetNotBuiltException(string? message)
        : base(message) { }

    public OptionSetNotBuiltException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException) { }

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    protected OptionSetNotBuiltException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
#pragma warning restore CS0809

    private static string GetDefaultMessage()
    {
        return "Option set was not built.";
    }
}
