using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Collections;
using NetArgumentParser.Subcommands;

namespace NetArgumentParser.Visitors;

public class ArgumentsVisitor
{
    public ArgumentsVisitor(
        ParserQuantum rootParserQuantum,
        IReadOnlyOptionSet<ICommonOption> rootOptions,
        bool recognizeSlashOptions = false)
    {
        ExtendedArgumentNullException.ThrowIfNull(rootParserQuantum, nameof(rootParserQuantum));
        ExtendedArgumentNullException.ThrowIfNull(rootOptions, nameof(rootOptions));

        RootParserQuantum = rootParserQuantum;
        RootOptions = rootOptions;
        RecognizeSlashOptions = recognizeSlashOptions;
    }

    public event EventHandler<SubcommandExtractedEventArgs>? SubcommandExtracted;
    public event EventHandler<OptionExtractedEventArgs>? OptionExtracted;
    public event EventHandler<UndefinedContextItemExtractedEventArgs>? UndefinedContextItemExtracted;

    public ParserQuantum RootParserQuantum { get; }
    public IReadOnlyOptionSet<ICommonOption> RootOptions { get; }

    public bool RecognizeSlashOptions { get; }

    public void VisitArguments(IEnumerable<string> arguments)
    {
        ExtendedArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        Queue<string> context = new(arguments);

        IReadOnlyOptionSet<ICommonOption> localOptions = RootOptions;
        ParserQuantum localParserQuantum = RootParserQuantum;

        while (context.Count > 0)
        {
            string contextItem = context.Dequeue();
            var argument = new Argument(contextItem, RecognizeSlashOptions);

            Subcommand? subcommand = localParserQuantum.Subcommands
                .FirstOrDefault(t => t.Name == contextItem);

            if (subcommand is not null)
            {
                localOptions = subcommand.OptionSet;
                localParserQuantum = subcommand;
                SubcommandExtracted?.Invoke(this, new SubcommandExtractedEventArgs(subcommand));
                continue;
            }

            bool isContextItemHandled = false;

            if (argument.IsOption)
            {
                string optionName = argument.ExtractOptionName();

                if (localOptions.HasOption(optionName))
                {
                    OptionValue optionValue = argument.ExtractOptionValueFromContext(context, localOptions);
                    OptionExtracted?.Invoke(this, new OptionExtractedEventArgs(optionValue));
                    isContextItemHandled = true;
                }
            }

            if (!isContextItemHandled)
            {
                UndefinedContextItemExtracted?.Invoke(
                    null,
                    new UndefinedContextItemExtractedEventArgs(contextItem));
            }
        }
    }
}
