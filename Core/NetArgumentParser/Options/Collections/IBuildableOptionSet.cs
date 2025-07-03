namespace NetArgumentParser.Options.Collections;

public interface IBuildableOptionSet<T> : IOptionSet<T>
    where T : IOption
{
    bool IsBuilt { get; }
    void Build();
}
