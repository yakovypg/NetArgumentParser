using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class OptionNotFoundException : Exception
{
    public OptionNotFoundException() {}

    public OptionNotFoundException(string? message)
        : base(message) {}

    public OptionNotFoundException(string? message, Exception? innerException)
        : base(message, innerException) {}

    public OptionNotFoundException(string? message, string optionName)  
        : this(message, optionName, null) {}
    
    public OptionNotFoundException(
        string? message,
        string optionName,
        Exception? innerException)  
        : base(message ?? GetDefaultMessage(optionName), innerException)
    {
        ArgumentNullException.ThrowIfNull(optionName, nameof(optionName));
        OptionName = optionName;
    }

    public string? OptionName { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    protected OptionNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        OptionName = info.GetString(nameof(OptionName));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        info.AddValue(nameof(OptionName), OptionName);
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(string optionName)
    {
        ArgumentNullException.ThrowIfNull(optionName, nameof(optionName));
        return $"Option with name '{optionName}' not found.";
    }
}
