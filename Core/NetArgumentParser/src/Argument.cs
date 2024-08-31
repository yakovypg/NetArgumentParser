using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Configuration;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Context;

namespace NetArgumentParser;

public class Argument
{
    private const StringComparison _defaultStringComparison = StringComparison.CurrentCulture;

    public Argument(string argument, bool recognizeSlashAsOption = false)
    {
        ExtendedArgumentException.ThrowIfNullOrWhiteSpace(argument, nameof(argument));

        Data = argument;
        RecognizeSlashAsOption = recognizeSlashAsOption;
    }

    public string Data { get; }
    public bool RecognizeSlashAsOption { get; init; }

    public bool IsLongNamedOption =>
        Data.Length >= SpecialCharacters.LongNamedOptionPrefix.Length + 1
            && Data.StartsWith(SpecialCharacters.LongNamedOptionPrefix, _defaultStringComparison);

    public bool IsSlashOption => RecognizeSlashAsOption
        && Data.Length >= 2
        && Data.StartsWith(SpecialCharacters.SlashOptionPrefix);

    public bool IsShortNamedOption => !IsLongNamedOption
        && Data.Length >= 2
        && Data.StartsWith(SpecialCharacters.ShortNamedOptionPrefix)
        && !char.IsDigit(Data[1]);

    public bool IsOption => IsShortNamedOption
        || IsLongNamedOption
        || (RecognizeSlashAsOption && IsSlashOption);

    public static IList<string> ExpandShortNamedOptions(IEnumerable<string> arguments)
    {
        ExtendedArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        var newOptions = new List<string>();

        foreach (string argument in arguments)
        {
            var argumentInfo = new Argument(argument, false);

            if (argumentInfo.IsShortNamedOption && !argumentInfo.HasValueAfterAssignmentCharacter())
            {
                IEnumerable<string> expandedOptions = argumentInfo.ExpandShortNamedOptionName();
                newOptions.AddRange(expandedOptions);
            }
            else
            {
                newOptions.Add(argument);
            }
        }

        return newOptions;
    }

    public bool HasValueAfterAssignmentCharacter()
    {
        int equationIndex = Data.IndexOf(
            SpecialCharacters.AssignmentCharacter,
            _defaultStringComparison);

        return equationIndex >= 0 && equationIndex < Data.Length - 1;
    }

    public string ExtractOptionName()
    {
        if (!IsOption)
            throw new InvalidOperationException("Argument is not option.");

        bool dataStartsWithongNamedOptionPrefix = Data.StartsWith(
            SpecialCharacters.LongNamedOptionPrefix,
            _defaultStringComparison);

        string argumentWithoutPrefix = dataStartsWithongNamedOptionPrefix
            ? Data.Remove(0, SpecialCharacters.LongNamedOptionPrefix.Length)
            : Data.StartsWith(SpecialCharacters.ShortNamedOptionPrefix)
                || Data.StartsWith(SpecialCharacters.SlashOptionPrefix)
            ? Data.Remove(0, 1)
            : Data;

        bool argumentWithoutPrefixContainsAssignmentCharacter = argumentWithoutPrefix.Contains(
            SpecialCharacters.AssignmentCharacter,
            _defaultStringComparison);

        if (argumentWithoutPrefixContainsAssignmentCharacter)
        {
            int equationIndex = argumentWithoutPrefix.IndexOf(
                SpecialCharacters.AssignmentCharacter,
                _defaultStringComparison);

            return argumentWithoutPrefix.Remove(equationIndex);
        }

        return argumentWithoutPrefix;
    }

    public string ExtractValueAfterAssignmentCharacter()
    {
        if (!HasValueAfterAssignmentCharacter())
            throw new ArgumentValueNotSpecifiedException(null, Data);

        int equationIndex = Data.IndexOf(
            SpecialCharacters.AssignmentCharacter,
            _defaultStringComparison);

        return Data[(equationIndex + 1)..];
    }

    public string[] ExtractOptionValueFromContext(Queue<string> context, ICommonOption option)
    {
        ExtendedArgumentNullException.ThrowIfNull(context, nameof(context));
        ExtendedArgumentNullException.ThrowIfNull(option, nameof(option));

        if (!IsOption)
            throw new InvalidOperationException("Argument is not option.");

        if (HasValueAfterAssignmentCharacter())
        {
            if (option.ContextCapture.MinNumberOfItemsToCapture > 1)
                throw new ArgumentValueNotRecognizedException(null, Data);

            return [ExtractValueAfterAssignmentCharacter()];
        }
        else
        {
            int numberOfItemsToCapture = option.ContextCapture
                .GetNumberOfItemsToCapture(context, RecognizeSlashAsOption);

            if (numberOfItemsToCapture > context.Count)
                throw new ArgumentValueNotRecognizedException(null, Data);

            IList<string> capturedContext = ContextInteractor.CaptureContext(
                context,
                numberOfItemsToCapture);

            if (capturedContext.Any(t => new Argument(t, RecognizeSlashAsOption).IsOption))
                throw new ArgumentValueNotRecognizedException(null, Data);

            return [.. capturedContext];
        }
    }

    public OptionValue ExtractOptionValueFromContext(
        Queue<string> context,
        IReadOnlyOptionSet<ICommonOption> options)
    {
        ExtendedArgumentNullException.ThrowIfNull(context, nameof(context));
        ExtendedArgumentNullException.ThrowIfNull(options, nameof(options));

        if (!IsOption)
            throw new InvalidOperationException("Argument is not option.");

        string argumentName = ExtractOptionName();
        ICommonOption option = options.GetOption(argumentName);
        string[] optionValue = ExtractOptionValueFromContext(context, option);

        return new OptionValue(option, optionValue);
    }

    public IEnumerable<string> ExpandShortNamedOptionName()
    {
        if (!IsShortNamedOption)
            throw new InvalidOperationException("Argument is not short minus option.");

        string name = ExtractOptionName();
        return name.ToCharArray().Select(t => $"-{t}");
    }
}
