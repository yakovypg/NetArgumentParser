using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NetArgumentParser.Extensions;

namespace NetArgumentParser.Options.Configuration;

public static class OptionValueRestrictionParser
{
    public const string ExpectedFormat = "f1 p1 ...\\nOP f2 p1 ...\\n ...\\n?msg";
    public const char PartsSeparator = '\n';
    public const char SubpartsSeparator = ' ';
    public const char MessageIndicator = '?';

    private enum LogicalOperator
    {
        And,
        Or
    }

    public static OptionValueRestriction<T> Parse<T>(string data, bool makePredicatesSafe = false)
    {
        ExtendedArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));

        string[] parts = data.Split(PartsSeparator);

        var predicates = new List<Predicate<T>>();
        var logicalOperators = new List<LogicalOperator>();

        string? valueNotSatisfyRestrictionMessage = null;

        for (int i = 0; i < parts.Length; ++i)
        {
            if (parts[i].StartsWith(MessageIndicator))
            {
                if (parts[i].Length > 1)
                    valueNotSatisfyRestrictionMessage = parts[i].Substring(1);

                continue;
            }

            string[] subparts = parts[i].Split(SubpartsSeparator);
            bool hasLogicalOperator = TryParseLogicalOperator(subparts[0], out LogicalOperator logicalOperator);

            if (i > 0)
            {
                LogicalOperator logicalOperatorToAdd = hasLogicalOperator ? logicalOperator : LogicalOperator.And;
                logicalOperators.Add(logicalOperatorToAdd);
            }

            string[] predicateData = hasLogicalOperator
                ? [.. subparts.Skip(1)]
                : subparts;

            Predicate<T> predicate = ParsePredicate<T>(predicateData);

            Predicate<T> predicateToAdd = makePredicatesSafe
                ? MakeSafePredicate(predicate)
                : predicate;

            predicates.Add(predicateToAdd);
        }

        Predicate<T> isValueAllowed = CombinePredicates(predicates, logicalOperators);

        return new OptionValueRestriction<T>(isValueAllowed, valueNotSatisfyRestrictionMessage);
    }

    public static OptionValueRestriction<IList<T>> ParseForList<T>(string data, bool makePredicatesSafe = false)
    {
        ExtendedArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));

        OptionValueRestriction<T> singleValueRestriction = Parse<T>(data, makePredicatesSafe);

        return new OptionValueRestriction<IList<T>>(
            t => t.All(x => singleValueRestriction.IsValueAllowed(x)),
            singleValueRestriction.ValueNotSatisfyRestrictionMessage);
    }

    private static Predicate<T> MakeSafePredicate<T>(Predicate<T> predicate)
    {
        ExtendedArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        return t =>
        {
            try
            {
                return predicate.Invoke(t);
            }
            catch
            {
                return false;
            }
        };
    }

    private static Predicate<T> CombinePredicates<T>(List<Predicate<T>> predicates, List<LogicalOperator> connections)
    {
        ExtendedArgumentNullException.ThrowIfNull(predicates, nameof(predicates));
        ExtendedArgumentNullException.ThrowIfNull(connections, nameof(connections));
        DefaultExceptions.ThrowIfNotEqual(connections.Count, predicates.Count - 1, nameof(predicates));

        if (predicates.Count == 0)
            return _ => true;

        if (predicates.Count == 1)
            return predicates[0];

        return item =>
        {
            bool result = predicates[0](item);

            for (int i = 1; i < predicates.Count; ++i)
            {
                bool next = predicates[i](item);
                LogicalOperator logicalOperator = connections[i - 1];

                if (logicalOperator == LogicalOperator.And)
                    result = result && next;
                else if (logicalOperator == LogicalOperator.Or)
                    result = result || next;
                else
                    throw new NotSupportedException($"Logical operator '{logicalOperator}' not supported");
            }

            return result;
        };
    }

    private static bool TryParseLogicalOperator(string data, out LogicalOperator logicalOperator)
    {
        ExtendedArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));

        LogicalOperator? result = data.ToUpper(CultureInfo.CurrentCulture) switch
        {
            "AND" or "&&" or "&" => LogicalOperator.And,
            "OR" or "||" or "|" => LogicalOperator.Or,
            _ => null
        };

        logicalOperator = result ?? LogicalOperator.And;
        return result is not null;
    }

    private static Predicate<T> ParsePredicate<T>(string[] data)
    {
        ExtendedArgumentNullException.ThrowIfNull(data, nameof(data));

        if (data.Length == 0)
        {
            throw new ArgumentException(
                $"Recieved data has incorrect format. Expected: {ExpectedFormat}",
                nameof(data));
        }

        string name = data[0];
        string[] parameters = [.. data.Skip(1)];

        return name.ToUpper(CultureInfo.CurrentCulture) switch
        {
            "EQUAL" or "==" or "=" => ParseEqualPredicate<T>(parameters),
            "NOTEQUAL" or "!=" or "<>" => ParseNotEqualPredicate<T>(parameters),
            "LESS" or "<" => ParseLessPredicate<T>(parameters),
            "LESSOREQUAL" or "<=" => ParseLessOrEqualPredicate<T>(parameters),
            "GREATER" or ">" => ParseGreaterPredicate<T>(parameters),
            "GREATEROREQUAL" or ">=" => ParseGreaterOrEqualPredicate<T>(parameters),
            "INRANGE" or "MINMAX" => ParseInRangePredicate<T>(parameters),
            "ONEOF" or "INLIST" => ParseOneOfPredicate<T>(parameters),
            "MATCH" or "REGEX" => ParseMatchPredicate<T>(parameters),
            "DIRECTORYEXISTS" or "DIRECTORY" => ParseDirectoryExistsPredicate<T>(parameters),
            "FILEEXISTS" or "FILE" => ParseFileExistsPredicate<T>(parameters),
            "MAXFILESIZE" or "MAXSIZE" => ParseMaxFileSizePredicate<T>(parameters),
            "EXTENSION" or "EXT" => ParseFileExtensionPredicate<T>(parameters),

            _ => throw new ArgumentOutOfRangeException(nameof(data), "Unknown predicate name")
        };
    }

    private static Predicate<T> ParseComparePredicate<T>(
        string[] parameters,
        Func<dynamic, double, bool> compareFunc)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        DefaultExceptions.ThrowIfNotEqual(parameters.Length, 1, nameof(parameters.Length));

        double rhs = double.Parse(parameters[0], CultureInfo.CurrentCulture);
        return value => value is null || compareFunc(value, rhs);
    }

    private static Predicate<T> ParseEqualPredicate<T>(string[] parameters)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        return ParseComparePredicate<T>(parameters, (lhs, rhs) => lhs == rhs);
    }

    private static Predicate<T> ParseNotEqualPredicate<T>(string[] parameters)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        return ParseComparePredicate<T>(parameters, (lhs, rhs) => lhs != rhs);
    }

    private static Predicate<T> ParseLessPredicate<T>(string[] parameters)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        return ParseComparePredicate<T>(parameters, (lhs, rhs) => lhs < rhs);
    }

    private static Predicate<T> ParseLessOrEqualPredicate<T>(string[] parameters)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        return ParseComparePredicate<T>(parameters, (lhs, rhs) => lhs <= rhs);
    }

    private static Predicate<T> ParseGreaterPredicate<T>(string[] parameters)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        return ParseComparePredicate<T>(parameters, (lhs, rhs) => lhs > rhs);
    }

    private static Predicate<T> ParseGreaterOrEqualPredicate<T>(string[] parameters)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        return ParseComparePredicate<T>(parameters, (lhs, rhs) => lhs >= rhs);
    }

    private static Predicate<T> ParseInRangePredicate<T>(string[] parameters)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        DefaultExceptions.ThrowIfNotEqual(parameters.Length, 2, nameof(parameters.Length));

        dynamic minValue = double.Parse(parameters[0], CultureInfo.CurrentCulture);
        dynamic maxValue = double.Parse(parameters[1], CultureInfo.CurrentCulture);

        return value => value is null || (value >= minValue && value <= maxValue);
    }

    private static Predicate<T> ParseOneOfPredicate<T>(string[] parameters)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        DefaultExceptions.ThrowIfEqual(parameters.Length, 0, nameof(parameters.Length));

        return value => parameters.Contains(value?.ToString() ?? string.Empty);
    }

    private static Predicate<T> ParseMatchPredicate<T>(string[] parameters)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        DefaultExceptions.ThrowIfEqual(parameters.Length, 0, nameof(parameters.Length));

        string pattern = string.Join($"{SubpartsSeparator}", parameters);

        var regex = new Regex(pattern);
        return value => value is not null && regex.IsMatch(value.ToString() ?? string.Empty);
    }

    private static Predicate<T> ParseDirectoryExistsPredicate<T>(string[] parameters)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        DefaultExceptions.ThrowIfNotEqual(parameters.Length, 0, nameof(parameters.Length));

        return value => value is string path && Directory.Exists(path);
    }

    private static Predicate<T> ParseFileExistsPredicate<T>(string[] parameters)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        DefaultExceptions.ThrowIfNotEqual(parameters.Length, 0, nameof(parameters.Length));

        return value => value is string path && File.Exists(path);
    }

    private static Predicate<T> ParseMaxFileSizePredicate<T>(string[] parameters)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        DefaultExceptions.ThrowIfNotEqual(parameters.Length, 1, nameof(parameters.Length));

        dynamic maxFileSizeInBytes = long.Parse(parameters[0], CultureInfo.CurrentCulture);

        return value =>
        {
            if (value is not string filePath || !File.Exists(filePath))
                return false;

            try
            {
                var fileInfo = new FileInfo(filePath);
                long fileSizeInBytes = fileInfo.Length;

                return fileSizeInBytes <= maxFileSizeInBytes;
            }
            catch
            {
                return false;
            }
        };
    }

    private static Predicate<T> ParseFileExtensionPredicate<T>(string[] parameters)
    {
        ExtendedArgumentNullException.ThrowIfNull(parameters, nameof(parameters));
        DefaultExceptions.ThrowIfEqual(parameters.Length, 0, nameof(parameters.Length));

        IEnumerable<string> allowedExtensions = parameters.Select(t =>
        {
            const string periodPrefix = ".";

            string extension = t.StartsWith(periodPrefix, StringComparison.CurrentCulture)
                ? t
                : $"{periodPrefix}{t}";

            return extension.ToUpper(CultureInfo.CurrentCulture);
        });

        return value =>
        {
            if (value is not string filePath)
                return false;

            try
            {
                string fileExtension = Path
                    .GetExtension(filePath)
                    .ToUpper(CultureInfo.CurrentCulture);

                return allowedExtensions.Contains(fileExtension);
            }
            catch
            {
                return false;
            }
        };
    }
}
