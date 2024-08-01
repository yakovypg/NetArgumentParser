using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class OptionValueNotSatisfyRestrictionException : Exception
{   
    public OptionValueNotSatisfyRestrictionException() {}

    public OptionValueNotSatisfyRestrictionException(string? message)
        : base(message) {}

    public OptionValueNotSatisfyRestrictionException(string? message, Exception? innerException)
        : base(message, innerException) {}

    public OptionValueNotSatisfyRestrictionException(string? message, string[] optionValue)  
        : this(message, optionValue, null) {}
    
    public OptionValueNotSatisfyRestrictionException(
        string? message,
        string[] optionValue,
        Exception? innerException)  
        : base(message ?? GetDefaultMessage(optionValue), innerException)
    {
        ArgumentNullException.ThrowIfNull(optionValue, nameof(optionValue));
        OptionValue = optionValue;
    }

    public string[]? OptionValue { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    protected OptionValueNotSatisfyRestrictionException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        OptionValue = info.GetValue(nameof(OptionValue), typeof(string[])) as string[];
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        info.AddValue(nameof(OptionValue), OptionValue, typeof(string[]));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(string[] optionValue)
    {
        ArgumentNullException.ThrowIfNull(optionValue, nameof(optionValue));

        string value = string.Join(' ', optionValue);
        return $"Option value '{value}' doesn't satisfy the restriction.";
    }
}
