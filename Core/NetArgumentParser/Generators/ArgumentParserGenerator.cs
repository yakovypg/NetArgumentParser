using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetArgumentParser.Attributes;
using NetArgumentParser.Attributes.Extensions;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Collections;
using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Generators;

public class ArgumentParserGenerator
{
    public void ConfigureParser(ArgumentParser parser, object config)
    {
        ExtendedArgumentNullException.ThrowIfNull(parser, nameof(parser));
        ExtendedArgumentNullException.ThrowIfNull(config, nameof(config));

        Type configType = config.GetType();

        bool isSuitableConfig = configType.CustomAttributes
            .Any(t => t.AttributeType == typeof(ParserConfigAttribute));

        if (!isSuitableConfig)
            throw new UnsupportedParserConfigException(null, config);

        var rootQuantum = new ArgumentParserGeneratorQuantum(
            options: ConfigureOptions(parser, config),
            subcommands: ConfigureSubcommands(parser, config));

        ConfigureMutuallyExclusiveOptionGroups(parser, rootQuantum);
    }

    protected virtual Dictionary<PropertyInfo, ICommonOption> ConfigureOptions(
        ParserQuantum parserQuantum,
        object config)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(config, nameof(config));

        Type configType = config.GetType();

        IEnumerable<PropertyInfo> optionProperties = configType
            .GetProperties()
            .Where(t => t.HasOptionAttribute() && t.CanRead && t.CanWrite);

        Dictionary<PropertyInfo, ICommonOption> optionMap = optionProperties
            .CreateOptions(config);

        ConfigureOptionGroups(parserQuantum, optionMap);

        return optionMap;
    }

    protected virtual void ConfigureOptionGroups(
        ParserQuantum parserQuantum,
        IReadOnlyDictionary<PropertyInfo, ICommonOption> optionMap)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(optionMap, nameof(optionMap));

        IEnumerable<IGrouping<OptionGroupAttribute?, PropertyInfo>> attributeGroups =
            optionMap.Keys.GroupOptionAttributes();

        foreach (var attributeGroup in attributeGroups)
        {
            OptionGroupAttribute? attribute = attributeGroup.Key;

            if (attribute is not null)
            {
                attribute = optionMap.Keys
                    .FindOptionGroupAttributeWithBestParameters<OptionGroupAttribute>(attribute.Id);
            }

            OptionGroup<ICommonOption>? optionGroup = attribute is not null
                ? parserQuantum.AddOptionGroup(attribute.Header, attribute.Description)
                : null;

            foreach (PropertyInfo property in attributeGroup)
            {
                ICommonOption option = optionMap[property];

                if (optionGroup is null)
                    parserQuantum.AddOptions(option);
                else
                    optionGroup.AddOptions(option);
            }
        }
    }

    protected virtual void ConfigureMutuallyExclusiveOptionGroups(
        ArgumentParser argumentParser,
        ArgumentParserGeneratorQuantum rootQuantum)
    {
        ExtendedArgumentNullException.ThrowIfNull(argumentParser, nameof(argumentParser));
        ExtendedArgumentNullException.ThrowIfNull(rootQuantum, nameof(rootQuantum));

        IEnumerable<KeyValuePair<PropertyInfo, ICommonOption>> optionsWithGroup =
            rootQuantum.FindOptions(t => t.Key.HasMutuallyExclusiveOptionGroupAttribute(), true);

        var mutuallyExclusiveOptionGroups = optionsWithGroup.GroupBy(t =>
        {
            return t.Key.GetCustomAttribute<MutuallyExclusiveOptionGroupAttribute>();
        });

        foreach (var group in mutuallyExclusiveOptionGroups)
        {
            MutuallyExclusiveOptionGroupAttribute? attribute = group.Key;

            if (attribute is null)
                continue;

            MutuallyExclusiveOptionGroupAttribute? attributeWithBestParameters = rootQuantum.Options
                .Select(t => t.Key)
                .FindOptionGroupAttributeWithBestParameters<MutuallyExclusiveOptionGroupAttribute>(
                    attribute.Id);

            if (attributeWithBestParameters is not null)
                attribute = attributeWithBestParameters;

            IEnumerable<ICommonOption> groupOptions = group.Select(t => t.Value);

            _ = argumentParser.AddMutuallyExclusiveOptionGroup(
                attribute.Header,
                attribute.Description,
                groupOptions);
        }
    }

    protected virtual IList<ArgumentParserGeneratorQuantum> ConfigureSubcommands(
        ParserQuantum parserQuantum,
        object config)
    {
        ExtendedArgumentNullException.ThrowIfNull(parserQuantum, nameof(parserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(config, nameof(config));

        Type configType = config.GetType();

        IEnumerable<PropertyInfo> subcommandProperties = configType
            .GetProperties()
            .Where(t => t.HasSubcommandAttribute() && t.CanRead);

        List<ArgumentParserGeneratorQuantum> quantums = [];

        foreach (PropertyInfo propertyInfo in subcommandProperties)
        {
            SubcommandAttribute? subcommandAttribute = propertyInfo
                .GetCustomAttribute<SubcommandAttribute>();

            if (subcommandAttribute is null)
                continue;

            Subcommand subcommand = parserQuantum.AddSubcommand(
                subcommandAttribute.Name,
                subcommandAttribute.Description);

            object subcommandConfig = propertyInfo.GetValue(config)
                ?? throw new NullSubcommandConfigException(null, propertyInfo);

            Dictionary<PropertyInfo, ICommonOption> optionMap =
                ConfigureOptions(subcommand, subcommandConfig);

            IList<ArgumentParserGeneratorQuantum> optionMapFromSubcommands =
                ConfigureSubcommands(subcommand, subcommandConfig);

            var quantum = new ArgumentParserGeneratorQuantum(
                optionMap,
                optionMapFromSubcommands);

            quantums.Add(quantum);
        }

        return quantums;
    }
}
