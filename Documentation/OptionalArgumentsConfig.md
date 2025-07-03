# Optional Arguments Configuaration
Optional arguments can be configured differently. Now you will find out exactly how.

## Table of Contents
*    [Options View](#options-view)
     *    [Short Minus Options](#short-minus-options)
     *    [Compound Options](#compound-options)
     *    [Double Minus Options](#double-minus-options)
     *    [Slash Options](#slash-options)
     *    [Custom Option View](#custom-option-view)
*    [Options Configuration](#options-configuration)
     *    [Names and Description](#names-and-description)
     *    [Meta Variable](#meta-variable)
     *    [Value Choices](#value-choices)
     *    [Default Value](#default-value)
     *    [Value Restrictions](#value-restrictions)
     *    [Required Options](#required-options)
     *    [Hidden Options And Aliases](#hidden-options-and-aliases)
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

### Custom Option View
You can change assignmnet character, slash option prefix and short named (short minus) option prefix. To do this, you need to specify a specific value in the corresponding property of class `SpecialCharacters`. You can do it the same way as in the following example:

```cs
using NetArgumentParser.Configuration;

SpecialCharacters.AssignmentCharacter = '>';
SpecialCharacters.SlashOptionPrefix = '|';
SpecialCharacters.ShortNamedOptionPrefix = '+';
```

This configuration will be applied to all existing parsers.

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

### Value Choices
You can specify a set of allowed values for the option. Value choices is only available for value options.

```cs
var valueOption = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    choices: [0, 45, 90]);

var multipleValueOption = new MultipleValueOption<byte>("margin", "m",
    choices: [[0, 0, 0, 0], [10, 10, 10, 10]]);

var enumValueOption = new EnumValueOption<StringSplitOptions>("options", "o",
    choices: [StringSplitOptions.TrimEntries, StringSplitOptions.RemoveEmptyEntries]);
```

You can also specify a set of allowed before parse values for the option. These choices are available for value options and enum value options, but not for multiple value options. It can be useful if you have a custom converter and want to validate values before they are passed to it.

```cs
var valueOption = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    beforeParseChoices: ["0", "45", "90"]);

var enumValueOption = new EnumValueOption<StringSplitOptions>("options", "o",
    beforeParseChoices: ["TrimEntries", "RemoveEmptyEntries"]);
```

If necessary, before parse choices can be combined with standard choices.

```cs
var valueOption = new ValueOption<int>("width", "w",
    description: "new width of the image",
    choices: [1920, 1920.5]
    beforeParseChoices: ["1920", "1920,5"]);
```

Enum value options have default choices. It is all values of the corresponding enum that satisfy the restriction (if it specified). You can disable the use of default choices by setting `useDefaultChoices` parameter to `false` or by specifying your own choices.

```cs
var stringSplitOptions = new EnumValueOption<StringSplitOptions>("options", "o",
    useDefaultChoices: false);

var fileMode = new EnumValueOption<FileMode>("mode", "m",
    choices: [FileMode.Open, FileMode.Create]);
```

You can allow values ​​to be specified for a multiple value option in any order by passing `true` as the `ignoreOrderInChoices` argument. For example, if you specified [1, 2] as a choices, the parser will allow both [1, 2] and [2, 1] to be entered.

```cs
var numbersOption = new MultipleValueOption<byte>(
    "numbers",
    "n",
    ignoreOrderInChoices: true,
    contextCapture: new FixedContextCapture(2),
    choices: [[1, 2]]);
```

You can also allow string data to be entered in any case by passing `true` as the `ignoreCaseInChoices` argument. For example, if you specified ["Max"] as a choices, the parser will allow the string "Max" to be entered in any case (i.e. "Max", "max", "MAX", "mAx", etc. will be considered valid data).

```cs
var namesOption = new MultipleValueOption<string>(
    "names",
    "n",
    ignoreCaseInChoices: true,
    contextCapture: new FixedContextCapture(1),
    choices: [["Max"]]);

var firstNameOption = new ValueOption<string>(
    "first-name",
    string.Empty,
    ignoreCaseInChoices: true,
    choices: ["Max"]);
```

### Default Value
You can specify a default value for the option. In this case, if the input argument list doesn't contain a matching argument, the option will be assigned its default value. Default value is only available for value options.

```cs
var option = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    defaultValue: 60);
```

### Value Restrictions
You can specify a restriction for option values. For example, you can limit the value only positive numbers. Value restrictions is only available for value options.

```cs
var valueOption = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    valueRestriction: new OptionValueRestriction<int>(t => t > 0));

var multipleValueOption = new MultipleValueOption<byte>("margin", "m",
    valueRestriction: new OptionValueRestriction<IList<byte>>(t => t.Contains(5)));

var enumValueOption = new EnumValueOption<StringSplitOptions>("options", "o",
    valueRestriction: new OptionValueRestriction<StringSplitOptions>(t => t != StringSplitOptions.None));
```

### Required Options
You can indicate that the option is required. In this case, if the input argument list doesn't contain a matching argument, an exception will be thrown.

```cs
var option = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    isRequired: true);
```

### Hidden Options And Aliases
Sometimes you need to hide an option from being used when printing command-line help. For example, to gradually refuse a deprecated argument name without breaking backwards compatibility. You can make the option hidden as follows:

```cs
var option = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    isHidden: true);
```

Another way to maintain backward compatibility is to use aliases. They will not be displayed when printing text, but can be used as an option identifier, just like the short and long names. You can specify option aliases as follows:

```cs
var option = new ValueOption<int>("angle", "a",
    description: "angle by which you want to rotate the image",
    aliases: ["rotation-angle", "rotation", "A"]);
```

Please note that aliases must be unique like short and long names.

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
