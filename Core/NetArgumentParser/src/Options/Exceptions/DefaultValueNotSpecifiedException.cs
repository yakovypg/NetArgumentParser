using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class DefaultValueNotSpecifiedException : Exception
{
    public DefaultValueNotSpecifiedException() { }

    public DefaultValueNotSpecifiedException(string? message)
        : base(message) { }

    public DefaultValueNotSpecifiedException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public DefaultValueNotSpecifiedException(string? message, ICommonOption option)
        : this(message, option, null) { }

    public DefaultValueNotSpecifiedException(
        string? message,
        ICommonOption option,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(option), innerException)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));
        Option = option;
    }

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    protected DefaultValueNotSpecifiedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        Option = info.GetValue(nameof(Option), typeof(ICommonOption)) as ICommonOption;
    }
#pragma warning restore CS0809

    public ICommonOption? Option { get; private set; }

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        info.AddValue(nameof(Option), Option, typeof(ICommonOption));
        base.GetObjectData(info, context);
    }
#pragma warning restore CS0809

    private static string GetDefaultMessage(ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(option, nameof(option));
        return $"Default value isn't specified for option '{option}'.";
    }
}
