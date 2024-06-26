# Optional Arguments Configuaration
Optional arguments can be configured differently. Now you will find out exactly how.

## Table of Contents
*    [Options View](#options-view)
     *    [Short Minus Options](#short-minus-options)
     *    [Compound Options](#compound-options)
     *    [Double Minus Options](#double-minus-options)
     *    [Slash Options](#slash-options)
*    [Options Configuration](#options-configuration)
     *    [Names and Description](#names-and-description)
     *    [Meta Variable](#meta-variable)
     *    [Default Value](#default-value)
     *    [Required Options](#required-options)
     *    [Context Capture](#context-capture)
     *    [After Handling Action](#after-handling-action)

## Options View
**NetArgumentParser** supports all kind of options view.

### Short Minus Options
**Short minus options** are options like `-v`. Support of short minus options is enabled by default and cannot be disabled.

### Compound Options
**Compound options** are options that are combined and provided as a single argument. Example: `-lah`. This only works with a short single-character option names startes with a single minus.

Support of compound options is enabled by default. You can disable it the same way as in the following example:

```cs
var parser = new ArgumentParser()
{
    RecognizeCompoundOptions = false
};
```

### Double Minus Options
**Double minus options** are options like `--version`. Support of double minus options is enabled by default and cannot be disabled.

### Slash Options
**Slash options** are options like `/v` or `/version`. Support of slash options isn't enabled by default. You can enable it the same way as in the following example:

```cs
var parser = new ArgumentParser()
{
    RecognizeSlashOptions = true
};
```

Please note that you won't be able to pass Linux-style absolute paths (e.g. `/home/user/Downloads/file.txt`) as arguments after doing this, as they will be handled as options. The same applies to other arguments starting with a slash.

## Options Configuration
**NetArgumentParser** supports many configurations for options.

### Names and Description
You can specify long name, short name and description of the option. Please note that you can omit (pass empty string to do it) a long name or a short name, but you can't skip both of them.

```cs
var verboseOption = new FlagOption("verbose", "v", "be verbose");

var helpOption = new HelpOption(
    longName: string.Empty,
    shortName: "h",
    description: "show command-line help");
```

### Meta Variable
Meta variable is a varible that will be presented in the command-line help. It is only available for value options. By default meta variable is set as the option name in the upper case. But you can change it the same way as in the following example.

```cs
var option = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    metaVariable: "A");

var parser = new ArgumentParser();
parser.AddOptions(option);

Console.WriteLine(parser.ToString());
```

The output will be like the following:

```
Usage: [--angle A]

Options:
  --angle A, -a A  angle by which you want to rotate the image
```

### Default Value
You can specify a default value for the option. In this case, if the input argument list doesn't contain a matching argument, the option will be assigned its default value. Default value is only available for value options.

```cs
var option = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    defaultValue: 60);
```

### Required Options
You can indicate that the option is required. In this case, if the input argument list doesn't contain a matching argument, an exception will be thrown.

```cs
var option = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    isRequired: true);
```

### Context Capture
For multiple value options you can specify the number of arguments to be captured as their value.

The following possible context capture configurations are available:
- `EmptyContextCapture` means that no arguments will be captured from the context.
- `FixedContextCapture` means that the fixed number of arguments will be captured from the context.
- `OneOrMoreContextCapture` means that one or more arguments will be captured from the context.
- `ZeroOrMoreContextCapture` means that zero or more arguments will be captured from the context.
- `ZeroOrOneContextCapture` means that one or one argument will be captured from the context.

```cs
var option = new MultipleValueOption<string>("input", "i",
    description: "files that need to be processed",
    contextCapture: new OneOrMoreContextCapture(),
    afterValueParsingAction: t => inputFiles = new List<string>(t));
```

### After Handling Action
Options can perform an action after they are handled. The value options make it possible to process the parsed value.

You can understand it better in the following example:

```cs
int? angle = null;
bool verbose = false;
List<string> inputFiles = [];

var verboseOption = new FlagOption("verbose", "v",
    description: "be verbose",
    afterHandlingAction: () => verbose = true);

var angleOption = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    afterValueParsingAction: t => angle = t);

var filesOption = new MultipleValueOption<string>("input", "i",
    description: "files that need to be processed",
    contextCapture: new OneOrMoreContextCapture(),
    afterValueParsingAction: t => inputFiles = new List<string>(t));
```

This system of actions made due to the lack of a safe ability to work with references and pointers in the C# language.
