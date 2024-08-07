namespace NetArgumentParser.Visitors;

using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Options;
using NetArgumentParser.Subcommands;

public class ArgumentsVisitor
{
    public ArgumentsVisitor(
        ParserQuantum rootParserQuantum,
        IReadOnlyOptionSet<ICommonOption> rootOptions)
    {
        ArgumentNullException.ThrowIfNull(rootParserQuantum, nameof(rootParserQuantum));
        ArgumentNullException.ThrowIfNull(rootOptions, nameof(rootOptions));

        RootParserQuantum = rootParserQuantum;
        RootOptions = rootOptions;
    }

    public ParserQuantum RootParserQuantum { get; }
    public IReadOnlyOptionSet<ICommonOption> RootOptions { get; }

    public void VisitArguments(
        IEnumerable<string> arguments,
        bool recognizeSlashOptions = false,
        Action<OptionValue>? handleOptionValueAction = null,
        Action<string>? handleUndefinedContextItemAction = null)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        Queue<string> context = new(arguments);

        IReadOnlyOptionSet<ICommonOption> localOptions = RootOptions;
        ParserQuantum localParserQuantum = RootParserQuantum;

        while (context.Count > 0)
        {
            string contextItem = context.Dequeue();
            var argument = new Argument(contextItem, recognizeSlashOptions);

            Subcommand? subcommand = localParserQuantum.Subcommands
                .FirstOrDefault(t => t.Name == contextItem);

            if (subcommand is not null)
            {
                localOptions = subcommand.OptionSet;
                localParserQuantum = subcommand;
                continue;
            }

            bool isContextItemHandled = false;

            if (argument.IsOption)
            {
                string optionName = argument.ExtractOptionName();

                if (localOptions.HasOption(optionName))
                {
                    OptionValue optionValue = argument.ExtractOptionValueFromContext(context, localOptions);
                    handleOptionValueAction?.Invoke(optionValue);
                    isContextItemHandled = true;
                }
            }

            if (!isContextItemHandled)
                handleUndefinedContextItemAction?.Invoke(contextItem);
        }
    }
}
