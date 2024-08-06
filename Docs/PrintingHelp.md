# Printing Help
Argument parser uses description generator to geterate command-line help. If you don't specify it, the default generator will be used.

## Table of Contents
*    [Add Program Information](#add-program-information)
*    [Customizing Output](#customizing-output)
     *    [Change Headers](#change-headers)
          *    [Change usage header](#change-usage-header)
          *    [Change default option group header](#change-default-option-group-header)
          *    [Change subcommands header](#change-subcommands-header)
     *    [Change Delimiters](#change-delimiters)
          *    [Change option example delimiters](#change-option-example-delimiters)
          *    [Change subcommand name delimiters](#change-subcommand-name-delimiters)
     *    [Configure Printing Space](#configure-printing-space)
          *    [Configure window width](#configure-window-width)
          *    [Configure maximum space for option example](#configure-maximum-space-for-option-example)
*    [Change Output Stream](#change-output-stream)
*    [Custom Description Generators](#custom-description-generators)
     *    [Custom Application Description Generator](#custom-application-description-generator)
     *    [Custom Subcommand Description Generator](#custom-subcommand-description-generator)

## Add Program Information
You can specify name, version, description and epilog of your program. To do this, you need to set values in the corresponding properties, as shown in the example below.

```cs
var parser = new ArgumentParser()
{
    ProgramName = "ProgramName",
    ProgramVersion = "ProgramName v1.0.0",
    ProgramDescription = "What the program does",
    ProgramEpilog = "Text at the bottom"
};
```

The output will be like the following:

```
Usage: ProgramName ...

What the program does

Options:
  ...

Text at the bottom
```

## Customizing Output
**NetArgumentParser** allows you to customize the generating of command-line help. You can configure an existing generator or [create](#custom-description-generator) your own.

### Change Headers
You can change header of the `usage` section, header of the `subcommands` section and a default option group header.

#### Change usage header
To change a header of the `usage` section you need to create description generator and specify corresponding property in it. You can do this the same way as in the following example.

```cs
var parser = new ArgumentParser();

var generator = new ApplicationDescriptionGenerator(parser)
{
    UsageHeader = "My Usage: "
};

parser.DescriptionGenerator = generator;
```

The output will be like the following:

```
My Usage: ...

Options:
  ...
```

#### Change default option group header
You can change a default option group header. You can do this the same way as in the following example.

```cs
var parser = new ArgumentParser();
parser.DefaultGroup.Header = "Default group:";
```

The output will be like the following:

```
Usage: ...

Default group:
  ...
```

#### Change subcommands header
To change a header of the `subcommands` section you need to create description generator and specify corresponding property in it. You can do this the same way as in the following example.

```cs
var parser = new ArgumentParser();

var generator = new ApplicationDescriptionGenerator(parser)
{
    SubcommandsHeader = "My Subcommands:"
};

parser.DescriptionGenerator = generator;
```

The output will be like the following:

```
Usage: ...

Options:
  ...

My Subcommands: ...
  ...
```

### Change Delimiters
You can change the following delimiters:
- Before and after option example.
- Before and after subcommand name.

#### Change option example delimiters
You can change delimiters before and after option example. You can do this the same way as in the following example.

```cs
var parser = new ArgumentParser();

var generator = new ApplicationDescriptionGenerator(parser)
{
    OptionExamplePrefix = "@@",
    DelimiterAfterOptionExample = " -> "
};

parser.DescriptionGenerator = generator;
```

The output will be like the following:

```
Usage: ...

Options:
@@--angle ANGLE, -a ANGLE -> angle by which you want to rotate the image
@@--version               -> show version information
```

#### Change subcommand name delimiters
You can change delimiters before and after subcommand name. You can do this the same way as in the following example.

```cs
var parser = new ArgumentParser();

var generator = new ApplicationDescriptionGenerator(parser)
{
    SubcommandNamePrefix = "@@",
    DelimiterAfterSubcommandName = " -> "
};

parser.DescriptionGenerator = generator;
```

The output will be like the following:

```
Usage: ...

Options:
  ...

Subcommands:
@@resize -> resize the image
```

### Configure Printing Space
You can specify window width and maximum space for option example.

#### Configure window width
The width of the window is set in order to correctly wrap the components of the command-line help. The easiest way to set the width is using the `Console.WindowWidth` property. You can do it like this:

```cs
var parser = new ArgumentParser();

var generator = new ApplicationDescriptionGenerator(parser)
{
    WindowWidth = Console.WindowWidth
};

parser.DescriptionGenerator = generator;
```

#### Configure maximum space for option example
You can specify a character limit for the option examples. This is useful for small windows to prevent running out of space to describe options. The default value for this limit is 30, but you can change it as follows:

```cs
var parser = new ArgumentParser();

var generator = new ApplicationDescriptionGenerator(parser)
{
    OptionExampleCharsLimit = 25
};

parser.DescriptionGenerator = generator;
```

## Change Output Stream
You can change output stream in which all information and messages are written. To do this you need to create a custom class inherited from the `ITextWriter` interface and pass it to the `ChangeOutputWriter()` method. You can also use an existing text writer class as a base class. See example of this kind of inheritance, for example, by looking at the implementation of the `ConsoleTextWriter` class.

```cs
var parser = new ArgumentParser();
parser.ChangeOutputWriter(new ConsoleTextWriter());
```

## Custom Description Generators
You can create your own application description generator and subcommand description generator. This can be useful if you want to implement your own command-line help generation system.

### Custom Application Description Generator
You can create your own application description generator. To do this you need to inherit your class from the `IDescriptionGenerator` interface and implement it. You can also use an existing `ParserQuantumDescriptionGenerator` class as a base class. See example of this kind of inheritance, for example, by looking at the implementation of the `ApplicationDescriptionGenerator` class. Next, you can use this class in the same way as the standard ones.

```cs
var parser = new ArgumentParser();
var generator = new ApplicationDescriptionGenerator(parser);

parser.DescriptionGenerator = generator;
```

### Custom Subcommand Description Generator
You can create your own subcommand description generator. To do this you need to inherit your class from the `IDescriptionGenerator` interface and implement it. You can also use an existing `ParserQuantumDescriptionGenerator` class as a base class. See example of this kind of inheritance, for example, by looking at the implementation of the `SubcommandDescriptionGenerator` class. Next, you can use this class in the same way as the standard ones.

```cs
var parser = new ArgumentParser()
{
    SubcommandDescriptionGeneratorCreator = t => new SubcommandDescriptionGenerator(t)
};
```
