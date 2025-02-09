using System;
using System.IO;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Context;

var angleOption = new ValueOption<int>(
    longName: "angle",
    shortName: "a",
    description: "Angle",
    choices: [0, 45, 90]);

angleOption.AddChoicesToDescription();

var namesOption = new MultipleValueOption<string>(
    longName: "input",
    shortName: "i",
    description: "Names",
    contextCapture: new OneOrMoreContextCapture(),
    choices: [["John", "Max"], ["James", "Michael"]]);

namesOption.AddChoicesToDescription(
    separator: ", ",
    prefix: " (",
    postfix: ")",
    arraySeparator: "; ",
    arrayPrefix: "[",
    arrayPostfix: "]");

var fileModeOption = new EnumValueOption<FileMode>(
    longName: "mode",
    shortName: "m",
    description: "File mode",
    useDefaultChoices: true);

fileModeOption.AddChoicesToDescription(
    separator: " | ",
    prefix: ". Choices: { ",
    postfix: " }");

// Angle (0, 45, 90)
Console.WriteLine(angleOption.Description);

// Names ([John, Max]; [James, Michael])
Console.WriteLine(namesOption.Description);

// File mode. Choices: { CreateNew | Create | Open | OpenOrCreate | Truncate | Append }
Console.WriteLine(fileModeOption.Description);
