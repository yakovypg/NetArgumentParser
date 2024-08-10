# Subcommands
Many programs split up their functionality into a number of subcommands. For example, `dotnet` can invoke subcommands like `dotnet new`, `dotnet add`, and `dotnet build`. Splitting up functionality this way can be an extremely good idea when a program performs several different functions which require different kinds of command-line arguments.

outputweiret
descriptiongeneratorcreator

## Table of Contents
*    [Basics](#basics)
     *    [Create Subcommand](#create-subcommand)
     *    [Configure Subcommand](#configure-subcommand)
     *    [Default Help Option](#default-help-option)
*    [Nested Subcommands](#nested-subcommands)

## Basics
You can create and configure any number of subcommands and nested subcommands. Please note that subcommand names must be unique within each subcommand. Furthermore, subcommands have its own option namespace, which allows you to have options with the same name in different subcommands.

### Create Subcommand
`ArgumentParser` supports the creation of subcommands with the `AddSubcommand()` method.

```cs
var parser = new ArgumentParser();
Subcommand subcommand = parser.AddSubcommand("name", "description");
```

### Configure Subcommand
Subcommands have the same properties and methods for working with options and converters as the parser. For example, you can add options to the subcommand using `AddOptions()` method, add converters using `AddConverters()` method and add option group using `AddOptionGroup()` method.

```cs
bool verbose = false;
bool debug = false;

var parser = new ArgumentParser();
Subcommand subcommand = parser.AddSubcommand("name", "description");

subcommand.AddOptions(new FlagOption("verbose", afterHandlingAction: () => verbose = true));
subcommand.AddConverters(new ValueConverter<int>(Convert.ToInt32));

OptionGroup<ICommonOption> subcommandGroup = subcommand.AddOptionGroup("group", string.Empty);
subcommandGroup.AddOptions(new FlagOption("debug", afterHandlingAction: () => debug = true));
```

### Default Help Option
By default, subcommands are supplied with standard help option. However, you can specify that help option isn't needed. To do this, you can set the appropriate value for property `UseDefaultHelpOption` or add your own help option.

```cs
var parser = new ArgumentParser();

Subcommand subcommand = parser.AddSubcommand("name", "description");
resizeSubcommand.UseDefaultHelpOption = false;
```

```cs
var parser = new ArgumentParser();

Subcommand subcommand = parser.AddSubcommand("name", "description");
subcommand.AddOptions(new HelpOption());
```

## Nested Subcommands
You can create nested subcommands. It means that each subcommand can have its own subcommands. For example, `dotnet` program has `dotnet add` subcommand, which has a `dotnet add reference` subcommand.

```cs
var parser = new ArgumentParser();

Subcommand subcommand = parser.AddSubcommand("name1", "description1");
Subcommand subsubcommand = subcommand.AddSubcommand("name2", "description2");
Subcommand subsubsubcommand = subsubcommand.AddSubcommand("name3", "description3");
```
