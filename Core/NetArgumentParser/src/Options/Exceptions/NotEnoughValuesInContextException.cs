using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NetArgumentParser.Options;

[Serializable]
public class NotEnoughValuesInContextException : Exception
{
    private readonly string[]? _context;

    public NotEnoughValuesInContextException() { }

    public NotEnoughValuesInContextException(string? message)
        : base(message) { }

    public NotEnoughValuesInContextException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public NotEnoughValuesInContextException(
        string? message,
        string[] context,
        int numberOfNecessaryValues)
        : this(message, context, numberOfNecessaryValues, null) { }

    public NotEnoughValuesInContextException(
        string? message,
        string[] context,
        int numberOfNecessaryValues,
        Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException)
    {
        ExtendedArgumentNullException.ThrowIfNull(context, nameof(context));

        _context = context;
        NumberOfNecessaryValues = numberOfNecessaryValues;
    }

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected NotEnoughValuesInContextException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        _context = info.GetValue(nameof(_context), typeof(string[])) as string[];
        NumberOfNecessaryValues = info.GetInt32(nameof(NumberOfNecessaryValues));
    }

    public int NumberOfNecessaryValues { get; private set; }
    public IReadOnlyCollection<string> Context => _context ?? [];

#if NET8_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ExtendedArgumentNullException.ThrowIfNull(info, nameof(info));

        info.AddValue(nameof(_context), _context, typeof(string[]));
        info.AddValue(nameof(NumberOfNecessaryValues), NumberOfNecessaryValues);

        base.GetObjectData(info, context);
    }

    private static string GetDefaultMessage()
    {
        return "There are not enough values in the context.";
    }
}
