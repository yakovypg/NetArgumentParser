namespace NetArgumentParser.Options.Design;

public interface IValueOptionDescriptionDesigner
{
    void AddChoicesToDescription(
        string separator = ", ",
        string prefix = " (",
        string postfix = ")",
        string arraySeparator = "; ",
        string arrayPrefix = "[",
        string arrayPostfix = "]");
}
