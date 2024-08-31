using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class MutuallyExclusiveOptionsFoundException : Exception
{
    public MutuallyExclusiveOptionsFoundException() { }

    public MutuallyExclusiveOptionsFoundException(string? message)
        : base(message) { }

    public MutuallyExclusiveOptionsFoundException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public MutuallyExclusiveOptionsFoundException(
        string? message,
        ICommonOption newOption,
        ICommonOption existingOption)
        : this(message, newOption, existingOption, null) { }

    public MutuallyExclusiveOptionsFoundException(
        string? message,
        ICommonOption newOption,
        ICommonOption existingOption,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(newOption, existingOption), innerException)
    {
        ArgumentNullException.ThrowIfNull(newOption, nameof(newOption));
        ArgumentNullException.ThrowIfNull(existingOption, nameof(existingOption));

        NewOption = newOption;
        ExistingOption = existingOption;
    }

#pragma warning disable CS0809
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET5_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#else
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
    protected MutuallyExclusiveOptionsFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info, nameof(info));

        NewOption = info.GetValue(nameof(NewOption), typeof(ICommonOption)) as ICommonOption;
        ExistingOption = info.GetValue(nameof(ExistingOption), typeof(ICommonOption)) as ICommonOption;
    }
#pragma warning restore CS0809

    public ICommonOption? NewOption { get; private set; }
    public ICommonOption? ExistingOption { get; private set; }

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

        info.AddValue(nameof(NewOption), NewOption, typeof(ICommonOption));
        info.AddValue(nameof(ExistingOption), ExistingOption, typeof(ICommonOption));
        base.GetObjectData(info, context);
    }
#pragma warning restore CS0809

    private static string GetDefaultMessage(ICommonOption newOption, ICommonOption existingOption)
    {
        ArgumentNullException.ThrowIfNull(newOption, nameof(newOption));
        ArgumentNullException.ThrowIfNull(existingOption, nameof(existingOption));

        return $"Option '{newOption}' not allowed with option '{existingOption}'.";
    }
}
