using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NetArgumentParser.Attributes;

namespace NetArgumentParser.Tests.Models.Configurations;

[ParserConfig]
internal partial class OptionValueRestrictionParserGeneratorConfig
{
    public const string NamePattern = @"^[A-Z][a-z]*$";
    public const string PhonePattern = @"^\+?[1-9] \d{3} \d{3} \d{2} \d{2}$";

    public const string AngleLongName = "Angle";
    public const string? AngleValueRestriction = "== 45";

    public const string WeightLongName = "Weight";
    public const string? WeightValueRestriction = "!= 100";

    public const string AgeLongName = "Age";
    public const string? AgeValueRestriction = "< 20";

    public const string WidthLongName = "Width";
    public const string? WidthValueRestriction = "<= 21";

    public const string HeightLongName = "Height";
    public const string? HeightValueRestriction = "> 22";

    public const string LengthLongName = "Length";
    public const string? LengthValueRestriction = ">= 23";

    public const string VerbosityLongName = "Verbosity";
    public const string? VerbosityValueRestriction = "inrange 0 4";

    public const string NameLongName = "Name";
    public const string? NameValueRestriction = $"match {NamePattern}";

    public const string PhoneLongName = "Phone";
    public const string? PhoneValueRestriction = $"match {PhonePattern}";

    public const string OutputFilePathLongName = "OutputFilePath";
    public const string? OutputFilePathValueRestriction = "extension jpg .png GiF";

    public const string ModeLongName = "Mode";
    public const string? ModeValueRestriction = $"oneof {nameof(FileMode.Append)} {nameof(FileMode.Open)}";

    public const string NumbersLongName = "Numbers";
    public const string? NumbersValueRestriction = "< -100\nOR > 100\nOR oneof 1 5 7 10\nAND inrange -200 200";

#pragma warning disable SYSLIB1045 // Use GeneratedRegexAttribute to generate the regular expression implementation at compile time
    public static Predicate<double> AngleValueRestrictionPredicate { get; } = t => t == 45;
    public static Predicate<float> WeightValueRestrictionPredicate { get; } = t => t != 100;
    public static Predicate<int> AgeValueRestrictionPredicate { get; } = t => t < 20;
    public static Predicate<uint> WidthValueRestrictionPredicate { get; } = t => t <= 21;
    public static Predicate<long> HeightValueRestrictionPredicate { get; } = t => t > 22;
    public static Predicate<ulong> LengthValueRestrictionPredicate { get; } = t => t >= 23;
    public static Predicate<byte> VerbosityValueRestrictionPredicate { get; } = t => t >= 0 && t <= 4;
    public static Predicate<string> NameValueRestrictionPredicate { get; } = t => new Regex(NamePattern).IsMatch(t);
    public static Predicate<string> PhoneValueRestrictionPredicate { get; } = t => new Regex(PhonePattern).IsMatch(t);
#pragma warning restore SYSLIB1045 // Use GeneratedRegexAttribute to generate the regular expression implementation at compile time

    public static Predicate<string> OutputFilePathValueRestrictionPredicate { get; } = t =>
    {
        var allowedExtensions = new List<string> { ".JPG", ".PNG", ".GIF" };

        try
        {
            string extension = Path.GetExtension(t).ToUpper(CultureInfo.InvariantCulture);
            return allowedExtensions.Contains(extension);
        }
        catch
        {
            return false;
        }
    };

    public static Predicate<FileMode> ModeValueRestrictionPredicate { get; } = t =>
    {
        var allowedValues = new List<string> { nameof(FileMode.Append), nameof(FileMode.Open) };
        return allowedValues.Contains(t.ToString());
    };

    public static Predicate<IList<Number>> NumbersValueRestrictionPredicate { get; } = t =>
    {
        static bool Less(Number t) => t < -100;
        static bool Greater(Number t) => t > 100;
        static bool OneOf(Number t) => new List<double>() { 1, 5, 7, 10 }.Any(x => t == x);
        static bool InRange(Number t) => t >= -200 && t <= 200;
        static bool CombinedPredicate(Number t) => (Less(t) || Greater(t) || OneOf(t)) && InRange(t);

        return t.All(CombinedPredicate);
    };

    [ValueOption<double>(
        AngleLongName,
        valueRestriction: AngleValueRestriction)
    ]
    public double Angle { get; set; }

    [ValueOption<float>(
        WeightLongName,
        valueRestriction: WeightValueRestriction)
    ]
    public float Weight { get; set; }

    [ValueOption<int>(
        AgeLongName,
        valueRestriction: AgeValueRestriction)
    ]
    public int Age { get; set; }

    [ValueOption<uint>(
        WidthLongName,
        valueRestriction: WidthValueRestriction)
    ]
    public uint Width { get; set; }

    [ValueOption<long>(
        HeightLongName,
        valueRestriction: HeightValueRestriction)
    ]
    public long Height { get; set; }

    [ValueOption<ulong>(
        LengthLongName,
        valueRestriction: LengthValueRestriction)
    ]
    public ulong Length { get; set; }

    [ValueOption<byte>(
        VerbosityLongName,
        valueRestriction: VerbosityValueRestriction)
    ]
    public byte Verbosity { get; set; }

    [ValueOption<string>(
        NameLongName,
        valueRestriction: NameValueRestriction)
    ]
    public string? Name { get; set; }

    [ValueOption<string>(
        PhoneLongName,
        valueRestriction: PhoneValueRestriction)
    ]
    public string? Phone { get; set; }

    [ValueOption<string>(
        OutputFilePathLongName,
        valueRestriction: OutputFilePathValueRestriction)
    ]
    public string? OutputFilePath { get; set; }

    [EnumValueOption<FileMode>(
        ModeLongName,
        valueRestriction: ModeValueRestriction)
    ]
    public FileMode? Mode { get; set; }

    [MultipleValueOption<Number>(
        NumbersLongName,
        valueRestriction: NumbersValueRestriction)
    ]
    public List<Number>? Numbers { get; set; }
}
