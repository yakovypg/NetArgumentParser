using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Options;

public class OptionConfigurationSetter : IOptionConfigurationSetter
{
    public OptionConfigurationSetter(
        IReadOnlyDictionary<string, IOptionConfigurationProvider> optionConfigurationProviders)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionConfigurationProviders, nameof(optionConfigurationProviders));

        OptionConfigurationProviders = optionConfigurationProviders.ToDictionary(
            t => t.Key,
            t => new List<IOptionConfigurationProvider>() { t.Value });
    }

    public OptionConfigurationSetter(
        IReadOnlyDictionary<string, List<IOptionConfigurationProvider>> optionConfigurationProviders)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionConfigurationProviders, nameof(optionConfigurationProviders));
        OptionConfigurationProviders = optionConfigurationProviders;
    }

    public IReadOnlyDictionary<string, List<IOptionConfigurationProvider>> OptionConfigurationProviders { get; }

    public void SetOptionConfigurations(ArgumentParser parser)
    {
        ExtendedArgumentNullException.ThrowIfNull(parser, nameof(parser));

        IEnumerable<ParserQuantum> parserQuantums = parser.Subcommands
            .Cast<ParserQuantum>()
            .Concat([parser]);

        foreach (ParserQuantum parserQuantum in parserQuantums)
        {
            bool hasConfigurationProviders = OptionConfigurationProviders.TryGetValue(
                parserQuantum.Name,
                out List<IOptionConfigurationProvider>? configurationProviders);

            if (!hasConfigurationProviders || configurationProviders is null)
                continue;

            foreach (IOptionConfigurationProvider configurationProvider in configurationProviders)
            {
                configurationProvider.ConfigureOptions(parserQuantum);
            }
        }
    }
}
