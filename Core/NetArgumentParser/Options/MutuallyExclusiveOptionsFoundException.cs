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
        ExtendedArgumentNullException.ThrowIfNull(newOption, nameof(newOption));
        ExtendedArgumentNullException.ThrowIfNull(existingOption, nameof(existingOption));

        NewOption = newOption;
        ExistingOption = existingOption;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected MutuallyExclusiveOptionsFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        NewOption = info.GetValue(nameof(NewOption), typeof(ICommonOption)) as ICommonOption;
        ExistingOption = info.GetValue(nameof(ExistingOption), typeof(ICommonOption)) as ICommonOption;
    }

    public ICommonOption? NewOption { get; private set; }
    public ICommonOption? ExistingOption { get; private set; }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        info.AddValue(nameof(NewOption), NewOption, typeof(ICommonOption));
        info.AddValue(nameof(ExistingOption), ExistingOption, typeof(ICommonOption));
        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage(ICommonOption newOption, ICommonOption existingOption)
    {
        ExtendedArgumentNullException.ThrowIfNull(newOption, nameof(newOption));
        ExtendedArgumentNullException.ThrowIfNull(existingOption, nameof(existingOption));

        return $"Option '{newOption}' not allowed with option '{existingOption}'.";
    }
}
