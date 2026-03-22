using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Options;

public interface IOptionConfigurationProvider
{
    void ConfigureOptions(Subcommand subcommand);
}
