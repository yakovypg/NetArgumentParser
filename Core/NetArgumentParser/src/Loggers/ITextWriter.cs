namespace NetArgumentParser.Generators;

public interface ITextWriter
{
    void Write(string? text);
    void WriteLine(string? text = null);
}
