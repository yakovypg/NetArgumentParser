using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Configuration;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Context;

namespace NetArgumentParser;

public class Argument
{
    public Argument(string argument, bool recognizeSlashAsOption = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(argument, nameof(argument));

        Data = argument;
        RecognizeSlashAsOption = recognizeSlashAsOption;
    }

    public string Data { get; }
    public bool RecognizeSlashAsOption { get; init; }

    public bool IsLongNamedOption =>
        Data.Length >= SpecialCharacters.LongNamedOptionPrefix.Length + 1
        && Data.StartsWith(SpecialCharacters.LongNamedOptionPrefix);

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

    public bool HasValueAfterAssignmentCharacter()
    {
        int equationIndex = Data.IndexOf(SpecialCharacters.AssignmentCharacter);   
        return equationIndex >= 0 && equationIndex < Data.Length - 1;
    }

    public string ExtractOptionName()
    {       
        if (!IsOption)
            throw new InvalidOperationException("Argument is not option.");
        
        string argumentWithoutPrefix = Data.StartsWith(SpecialCharacters.LongNamedOptionPrefix)
            ? Data.Remove(0, SpecialCharacters.LongNamedOptionPrefix.Length)
            : Data.StartsWith(SpecialCharacters.ShortNamedOptionPrefix)
                || Data.StartsWith(SpecialCharacters.SlashOptionPrefix)
            ? Data.Remove(0, 1)
            : Data;
        
        if (argumentWithoutPrefix.Contains(SpecialCharacters.AssignmentCharacter))
        {
            int equationIndex = argumentWithoutPrefix.IndexOf(SpecialCharacters.AssignmentCharacter);
            return argumentWithoutPrefix.Remove(equationIndex);
        }

        return argumentWithoutPrefix;
    }

    public string ExtractValueAfterAssignmentCharacter()
    {
        if (!HasValueAfterAssignmentCharacter())
            throw new ArgumentValueNotSpecifiedException(null, Data);

        int equationIndex = Data.IndexOf(SpecialCharacters.AssignmentCharacter);
        return Data[(equationIndex + 1)..];
    }

    public string[] ExtractOptionValueFromContext(Queue<string> context, ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(option, nameof(option));

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

            List<string> capturedContext = ContextInteractor.CaptureContext(
                context,
                numberOfItemsToCapture);

            if (capturedContext.Any(t => new Argument(t, RecognizeSlashAsOption).IsOption))
                throw new ArgumentValueNotRecognizedException(null, Data);

            return [..capturedContext];
        }
    }

    public OptionValue ExtractOptionValueFromContext(
        Queue<string> context,
        IReadOnlyOptionSet<ICommonOption> options)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(options, nameof(options));

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

    public static List<string> ExpandShortNamedOptions(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

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
}
