using System;

namespace NetArgumentParser.Generators;

public class ConsoleTextWriter : TextWriter
{
    public ConsoleTextWriter()
        : base(Console.Out) {}
}
