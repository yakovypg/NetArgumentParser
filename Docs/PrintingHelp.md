# Printing Help
Argument parser uses description generator to geterate command-line help. If you don't specify it, the default generator will be used.

## Table of Contents
*    [Add Program Information](#add-program-information)
*    [Customizing Output](#customizing-output)
     *    [Change Headers](#change-headers)
          *    [Change usage header](#change-usage-header)
          *    [Change default option group header](#change-default-option-group-header)
     *    [Change Delimiters](#change-delimiters)
     *    [Configure Printing Space](#configure-printing-space)
          *    [Configure window width](#configure-window-width)
          *    [Configure maximum space for option example](#configure-maximum-space-for-option-example)
*    [Custom Description Generator](#custom-description-generator)

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
You can change a header of the `usage` section and a default option group header.

#### Change usage header
To change a header of the `usage` section you need to create description generator and specify corresponding property in it. You can do this the same way as in the following example.

```cs
var parser = new ArgumentParser();
var generator = new DescriptionGenerator(parser)
{
    UsageHeader = "My Usage: "
};
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

### Change Delimiters
You can change a delimiters before and after option example. You can do this the same way as in the following example.

```cs
var parser = new ArgumentParser();
var generator = new DescriptionGenerator(parser)
{
    OptionExamplePrefix = "@@",
    DelimiterAfterOptionExample = " -> "
};
```

The output will be like the following:

```
Usage: ...

Options:
@@--angle ANGLE, -a ANGLE -> angle by which you want to rotate the image
@@--version               -> show version information
```

### Configure Printing Space
You can specify window width and maximum space for option example.

#### Configure window width
The width of the window is set in order to correctly wrap the components of the command-line help. The easiest way to set the width is using the `Console.WindowWidth` property. You can do it like this:

```cs
var parser = new ArgumentParser();
var generator = new DescriptionGenerator(parser)
{
    WindowWidth = Console.WindowWidth
};
```

#### Configure maximum space for option example
You can specify a character limit for the option examples. This is useful for small windows to prevent running out of space to describe options. The default value for this limit is 30, but you can change it as follows:

```cs
var parser = new ArgumentParser();
var generator = new DescriptionGenerator(parser)
{
    OptionExampleCharsLimit = 25
};
```

## Custom Description Generator
You can create your own description generator. To do this you need to inherit your class from the `IDescriptionGenerator` interface. You can also use an existing description generator class as a base class. See example of this kind of inheritance, for example, by looking at the implementation of the `DescriptionGenerator` class.
