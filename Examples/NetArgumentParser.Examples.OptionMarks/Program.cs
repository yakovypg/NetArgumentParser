using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NetArgumentParser;
using NetArgumentParser.Attributes;
using NetArgumentParser.Converters;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Configuration;
using NetArgumentParser.Options.Context;

var generator = new ArgumentParserGenerator();
var parser = new ArgumentParser();
var config = new CustomParserConfig();

generator.ConfigureParser(parser, config, out IList<OptionMark> optionMarks);

IEnumerable<OptionMark> filePathOptionMarks = optionMarks
    .Where(t => t.Name == FilePathOptionMarkAttribute.DefaultName);

foreach (OptionMark optionMark in filePathOptionMarks)
{
    if (optionMark.Option is MultipleValueOption<string> filePathOption)
        filePathOption.Converter = new MultipleValueConverter<string>(Path.GetFullPath);
}

IEnumerable<OptionMark> directoryPathOptionMarks = optionMarks
    .Where(t => t.Name == OptionMarkNames.DirectoryPath);

foreach (OptionMark optionMark in directoryPathOptionMarks)
{
    if (optionMark.Option is ValueOption<string> directoryPathOption)
    {
        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        directoryPathOption.DefaultValue = new DefaultOptionValue<string>(defaultPath);
    }
}

IEnumerable<OptionMark> uppercaseDescriptionOptionMarks = optionMarks
    .Where(t => t.Name == OptionMarkNames.UppercaseDescription);

foreach (OptionMark optionMark in uppercaseDescriptionOptionMarks)
{
    if (optionMark.Option is ValueOption<string> uppercaseDescriptionOption)
        uppercaseDescriptionOption.Description = uppercaseDescriptionOption.Description.ToUpperInvariant();
}

_ = parser.FindFirstOptionByLongName(
    CustomParserConfig.OutputDirectoryLongName,
    false,
    out ICommonOption? outputDirectoryOption);

Console.WriteLine($"Description: {outputDirectoryOption?.Description}");

try
{
    _ = parser.Parse(args);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    return;
}

Console.WriteLine($"Angle: {config.Angle}");
Console.WriteLine($"InputFiles: {string.Join(" ", config.InputFiles)}");
Console.WriteLine($"OutputDirectoryPath: {config.OutputDirectory}");

#pragma warning disable
internal static class OptionMarkNames
{
    public const string DirectoryPath = nameof(DirectoryPath);
    public const string UppercaseDescription = nameof(UppercaseDescription);
}

[AttributeUsage(
    AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = true)
]
internal sealed class FilePathOptionMarkAttribute : OptionMarkAttribute
{
    public const string DefaultName = nameof(FilePathOptionMarkAttribute);

    public FilePathOptionMarkAttribute()
        : base(DefaultName)
    { }
}

[ParserConfig]
internal class CustomParserConfig
{
    public const string OutputDirectoryLongName = "output";

    [ValueOption<double>(longName: "angle")]
    public double Angle { get; set; }

    [MultipleValueOption<string>(
        longName: "input",
        contextCaptureType: ContextCaptureType.OneOrMore)]
    [FilePathOptionMark]
    public List<string> InputFiles { get; set; } = [];

    [ValueOption<string>(
        longName: OutputDirectoryLongName,
        description: "description")]
    [OptionMark(OptionMarkNames.DirectoryPath)]
    [OptionMark(OptionMarkNames.UppercaseDescription)]
    public string? OutputDirectory { get; set; }
}
#pragma warning restore

/*
./NetArgumentParser.Examples.OptionMarks \
    --input ./text.txt dir/image.jpg \
    --angle 0.5
*/
