using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class OptionSetNotBuiltException : Exception
{
    public OptionSetNotBuiltException() {}

    public OptionSetNotBuiltException(string? message)
        : base(message) {}

    public OptionSetNotBuiltException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException) {}
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    protected OptionSetNotBuiltException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        base.GetObjectData(info, context);
    }
    
    private static string GetDefaultMessage()
    {
        return "Option set was not built.";
    }
}
