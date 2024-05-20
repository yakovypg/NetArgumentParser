using System;
using System.Collections.Generic;
using System.Linq;
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

    public bool IsLongMinusOption => Data.Length >= 3 && Data.StartsWith("--");

    public bool IsSlashOption => RecognizeSlashAsOption
        && Data.Length >= 2
        && Data.StartsWith('/');

    public bool IsShortMinusOption => !IsLongMinusOption
        && Data.Length >= 2
        && Data.StartsWith('-')
        && !char.IsDigit(Data[1]);
    
    public bool IsOption => IsShortMinusOption
        || IsLongMinusOption
        || (RecognizeSlashAsOption && IsSlashOption);

    public bool HasValueAfterEqualSign()
    {
        int equationIndex = Data.IndexOf('=');   
        return equationIndex >= 0 && equationIndex < Data.Length - 1;
    }

    public string ExtractOptionName()
    {       
        if (!IsOption)
            throw new InvalidOperationException("Argument is not option.");
        
        string argumentWithoutPrefix = Data.StartsWith("--")
            ? Data.Remove(0, 2)
            : Data.StartsWith('-') || Data.StartsWith('/')
            ? Data.Remove(0, 1)
            : Data;
        
        if (argumentWithoutPrefix.Contains('='))
        {
            int equationIndex = argumentWithoutPrefix.IndexOf('=');
            return argumentWithoutPrefix.Remove(equationIndex);
        }

        return argumentWithoutPrefix;
    }

    public string ExtractValueAfterEqualSign()
    {
        if (!HasValueAfterEqualSign())
            throw new ArgumentValueNotSpecifiedException(null, Data);

        int equationIndex = Data.IndexOf('=');
        return Data[(equationIndex + 1)..];
    }

    public string[] ExtractOptionValueFromContext(Queue<string> context, ICommonOption option)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        if (!IsOption)
            throw new InvalidOperationException("Argument is not option.");

        if (HasValueAfterEqualSign())
        {
            if (option.ContextCapture.MinNumberOfItemsToCapture > 1)
                throw new ArgumentValueNotRecognizedException(null, Data);
            
            return [ExtractValueAfterEqualSign()];
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
        IOptionSet<ICommonOption> options)
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

    public IEnumerable<string> ExpandShortMinusOptionName()
    {
        if (!IsShortMinusOption)
            throw new InvalidOperationException("Argument is not short minus option.");

        string name = ExtractOptionName();
        return name.ToCharArray().Select(t => $"-{t}");
    }

    public static List<string> ExpandShortMinusOptions(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

        var newOptions = new List<string>();

        foreach (string argument in arguments)
        {
            var argumentInfo = new Argument(argument, false);

            if (argumentInfo.IsShortMinusOption && !argumentInfo.HasValueAfterEqualSign())
            {
                IEnumerable<string> expandedOptions = argumentInfo.ExpandShortMinusOptionName();          
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
