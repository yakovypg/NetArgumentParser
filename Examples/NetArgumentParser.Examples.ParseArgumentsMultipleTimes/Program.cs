using System;
using NetArgumentParser;
using NetArgumentParser.Options;

int angle = 0;
int width = 0;
int height = 0;

var options = new ICommonOption[]
{
    new ValueOption<int>("angle", afterValueParsingAction: t => angle = t),
    new ValueOption<int>("width", afterValueParsingAction: t => width = t),
    new ValueOption<int>("height", afterValueParsingAction: t => height = t)
};

var parser = new ArgumentParser();
parser.AddOptions(options);

_ = parser.Parse(["--angle", "1", "--width", "2", "--height", "3"]);
PrintSummary("=== Parse 1 ===");

_ = parser.Parse(["--angle", "4", "--width", "5", "--height", "6"]);
PrintSummary("\n=== Parse 2 ===");

void PrintSummary(string name)
{
    Console.WriteLine(name);
    Console.WriteLine($"Angle: {angle}");
    Console.WriteLine($"Width: {width}");
    Console.WriteLine($"Height: {height}");
}
