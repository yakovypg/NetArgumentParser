namespace NetArgumentParser.Options;

public interface IOption
{
    string LongName { get; }
    string ShortName { get; }
    string Description { get; }
    bool IsRequired { get; }
    bool IsHidden { get; }
    bool IsFinal { get; }
}
