using System;

namespace NetArgumentParser.Generators;

public class TextWriter : ITextWriter
{
    public TextWriter(System.IO.TextWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        Writer = writer;
    }

    protected System.IO.TextWriter Writer { get; }

    public virtual void Write(string? text)
    {
        Writer.Write(text);
    }

    public virtual void WriteLine(string? text = null)
    {
        Writer.WriteLine(text);
    }
}
