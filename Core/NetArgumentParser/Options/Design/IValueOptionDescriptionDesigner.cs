namespace NetArgumentParser.Options.Design;

public interface IValueOptionDescriptionDesigner
{
    void AddDefaultValueToDescription(
        string separator = ", ",
        string prefix = " [default=",
        string postfix = "]",
        string arraySeparator = "; ",
        string arrayPrefix = "[",
        string arrayPostfix = "]",
        string nullPresenter = "null");

    void AddChoicesToDescription(
        string separator = ", ",
        string prefix = " (",
        string postfix = ")",
        string arraySeparator = "; ",
        string arrayPrefix = "[",
        string arrayPostfix = "]");

    void AddBeforeParseChoicesToDescription(
        string separator = ", ",
        string prefix = " (",
        string postfix = ")",
        string arraySeparator = "; ",
        string arrayPrefix = "[",
        string arrayPostfix = "]");
}
