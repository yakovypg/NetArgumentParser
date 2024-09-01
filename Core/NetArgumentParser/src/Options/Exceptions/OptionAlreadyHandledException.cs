using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class OptionAlreadyHandledException : Exception
{
    public OptionAlreadyHandledException() { }

    public OptionAlreadyHandledException(string? message)
        : base(message) { }

    public OptionAlreadyHandledException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public OptionAlreadyHandledException(string? message, ICommonOption option)
        : this(message, option, null) { }

    public OptionAlreadyHandledException(
        string? message,
        ICommonOption option,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(option), innerException)
    {
        ExtendedArgumentNullException.ThrowIfNull(option, nameof(option));
        Option = option;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected OptionAlreadyHandledException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        Option = info.GetValue(nameof(Option), typeof(ICommonOption)) as ICommonOption;
    }

    public ICommonOption? Option { get; private set; }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));
        ExtendedArgumentNullException.ThrowIfNull(context, nameof(context));

        info.AddValue(nameof(Option), Option, typeof(ICommonOption));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(ICommonOption option)
    {
        ExtendedArgumentNullException.ThrowIfNull(option, nameof(option));
        return $"Option '{option}' has already been handled.";
    }
}
