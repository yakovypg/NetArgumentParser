using System;
using System.Collections.Generic;
using System.IO;
using NetArgumentParser;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Context;

int? angle = default;
FileMode? fileMode = default;
List<string> inputFiles = [];

var options = new ICommonOption[]
{
    new MultipleValueOption<string>(
        "input",
        "i",
        description: "images that need to be processed",
        isRequired: true,
        contextCapture: new OneOrMoreContextCapture(),
        afterValueParsingAction: t => inputFiles = new List<string>(t)),

    new ValueOption<int>(
        "angle",
        "a",
        description: "angle by which you want to rotate the image",
        isRequired: true,
        afterValueParsingAction: t => angle = t),

    new EnumValueOption<FileMode>(
        "file-mode",
        string.Empty,
        description: "specifies how the operatng system should open a file",
        defaultValue: new DefaultOptionValue<FileMode>(FileMode.OpenOrCreate),
        afterValueParsingAction: t => fileMode = t)
};

var parser = new ArgumentParser();
parser.AddOptions(options);

try
{
    _ = parser.Parse(args);
}
#pragma warning disable CA1031
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    return;
}
#pragma warning restore CA1031

Console.WriteLine($"Angle: {angle}");
Console.WriteLine($"File mode: {fileMode}");
Console.WriteLine($"Input files: {string.Join(' ', inputFiles)}");
