using System.Linq;

namespace NetArgumentParser.Configuration;

public static class SpecialCharacters
{
    public static char AssignmentCharacter { get; set; } = '=';
    public static char SlashOptionPrefix { get; set; } = '/';
    public static char ShortNamedOptionPrefix { get; set; } = '-';

    public static string LongNamedOptionPrefix => new(ShortNamedOptionPrefix, 2);

    public static bool IsStateValid()
    {
        char[] characters = [AssignmentCharacter, SlashOptionPrefix, ShortNamedOptionPrefix];

        int charactersCount = characters.Length;
        int distinctCharactersCount = characters.Distinct().Count();

        return charactersCount == distinctCharactersCount;
    }
}
