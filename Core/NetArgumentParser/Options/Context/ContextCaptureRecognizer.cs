using System;

namespace NetArgumentParser.Options.Context;

public static class ContextCaptureRecognizer
{
    public static bool TryRecognize(
        double minValue,
        double maxValue,
        out IContextCapture? contextCapture)
    {
        try
        {
            contextCapture = Recognize(minValue, maxValue);
        }
        catch
        {
            contextCapture = null;
        }

        return contextCapture is not null;
    }

    public static IContextCapture Recognize(
        ContextCaptureType contextCaptureType,
        int? numberOfItemsToCapture = null)
    {
        if (contextCaptureType != ContextCaptureType.Fixed
            && numberOfItemsToCapture is not null)
        {
            string message =
                "Number of items to capture can be specified only for " +
                "fixed context capture.";

            throw new ArgumentException(message, nameof(numberOfItemsToCapture));
        }

        return contextCaptureType switch
        {
            ContextCaptureType.Empty => new EmptyContextCapture(),
            ContextCaptureType.OneOrMore => new OneOrMoreContextCapture(),
            ContextCaptureType.ZeroOrMore => new ZeroOrMoreContextCapture(),
            ContextCaptureType.ZeroOrOne => new ZeroOrOneContextCapture(),

            ContextCaptureType.Fixed => new FixedContextCapture(numberOfItemsToCapture
                ?? throw new ContextCaptureNotRecognizedException(null, double.NaN, double.NaN)),

            _ => throw new ContextCaptureNotRecognizedException(null, double.NaN, double.NaN)
        };
    }

    public static IContextCapture Recognize(double minValue, double maxValue)
    {
        if (!double.IsNaN(minValue)
            && !double.IsNaN(maxValue)
            && minValue == maxValue)
        {
            int numberOfItemsToCapture = (int)minValue;
            return new FixedContextCapture(numberOfItemsToCapture);
        }

        return (minValue, maxValue) switch
        {
            (double.NaN, double.NaN) => new EmptyContextCapture(),
            (1, double.PositiveInfinity) => new OneOrMoreContextCapture(),
            (0, double.PositiveInfinity) => new ZeroOrMoreContextCapture(),
            (0, 1) => new ZeroOrOneContextCapture(),

            _ => throw new ContextCaptureNotRecognizedException(null, minValue, maxValue)
        };
    }
}
