# Custom Converters
Custom converters allow you to easily work with options whose values are non-standard types. For standard types default converters have already been implemented. However, you can create your own converter for the standard type.

## Table of Contents
*    [Converter Types](#converter-types)
     *    [Value Converter](#value-converter)
*    [Converter Set](#converter-set)

## Converter Types
There are three converter types: value converter, multiple value converter and enum value converter. They are used in value options, multiple value options and enum value options. However, in the vast majority of situations it is sufficient to use only the value converter.

Furthermore, you can create your own converters. To do this you need to inherit your class from the `IValueConverter` interface and implement it. You can also use an existing option class as a base class. See examples of this kind of inheritance, for example, by looking at the implementation of the `MultipleValueConverter` and `EnumValueConverter` classes. Next, you can use this class in the same way as the standard ones.

### Value Converter
To create a value converter for type T, you need a function that takes a string and returns T. Pass this function to the constructor of the appropriate class as shown in the example below.

Here is an example of creating value converter and using it in the parser:

```cs
string name = string.Empty;

var firstNameOption = new ValueOption<string>("name",
    afterValueParsingAction: t => name = t);

var converter = new ValueConverter<string>(t => t.ToUpper());

var parser = new ArgumentParser();

parser.AddOptions(firstNameOption);
parser.AddConverters(converter);

parser.Parse(new string[] { "--name", "name" });
// name: NAME
```

Note that this use case will only add the converter to one visibility level. This means that subcommands will not receive this converter. Here is an example showing this.

```cs
string firstName = string.Empty;
string secondName = string.Empty;

var firstNameOption = new ValueOption<string>("first",
    afterValueParsingAction: t => firstName = t);

var secondNameOption = new ValueOption<string>("second",
    afterValueParsingAction: t => secondName = t);

var converter = new ValueConverter<string>(t => t.ToUpper());
var parser = new ArgumentParser();

parser.AddOptions(firstNameOption);
parser.AddConverters(converter);

Subcommand subcommand = parser.AddSubcommand("subcommand", "description");
subcommand.AddOptions(secondNameOption);

parser.Parse(new string[] { "--first", "name", "subcommand", "--second", "name" });
// firstName: NAME
// secondName: name
```

You can add the same converter to a subcommand. In this case, the variable `secondName` will have the value "NAME".

```cs
// ...
// Same code here

Subcommand subcommand = parser.AddSubcommand("subcommand", "description");

subcommand.AddOptions(secondNameOption);
subcommand.AddConverters(converter);

parser.Parse(new string[] { "--first", "name", "subcommand", "--second", "name" });
// firstName: NAME
// secondName: NAME
```

If you want a converter to be added to all visibility levels (starting from the current one), use the `AddConverters()` method overload as shown below. This call will add coverter to all subcommands (including nested subcommands) present at the time of the call, which are at all levels, starting from the current one. Accordingly, it is best to call it right before parsing arguments.

```cs
string firstName = string.Empty;
string secondName = string.Empty;

var firstNameOption = new ValueOption<string>("first",
    afterValueParsingAction: t => firstName = t);

var secondNameOption = new ValueOption<string>("second",
    afterValueParsingAction: t => secondName = t);

var converter = new ValueConverter<string>(t => t.ToUpper());
var parser = new ArgumentParser();

parser.AddOptions(firstNameOption);

Subcommand subcommand = parser.AddSubcommand("subcommand", "description");
subcommand.AddOptions(secondNameOption);

parser.AddConverters(true, converter);
parser.Parse(new string[] { "--first", "name", "subcommand", "--second", "name" });
// firstName: NAME
// secondName: NAME
```

## Converter Set
You can put just one converter for each type in converter set.
```cs
var defaultIntConverter = new ValueConverter<int>(t => Math.Abs(int.Parse(t)));
var defaultStringConverter = new ValueConverter<string>(t => t.ToUpper());
var additionalStringConverter = new ValueConverter<string>(t => t.ToLower());

var parser = new ArgumentParser();
parser.AddConverters(defaultIntConverter); // Ok
parser.AddConverters(defaultStringConverter); // Ok
parser.AddConverters(additionalStringConverter); // Error
```

However, you can transfer an individual ownership of any suitable converter to the option, even if a converter for the same type is already in the converter set. Options without their own converter will have a converter from the converter set.

```cs
var defaultStringConverter = new ValueConverter<string>(t => t.ToUpper());
var additionalStringConverter = new ValueConverter<string>(t => t.ToLower());

string firstName = string.Empty;
string secondName = string.Empty;

var firstNameOption = new ValueOption<string>("f-name",
    afterValueParsingAction: t => firstName = t);

var secondNameOption = new ValueOption<string>("s-name",
    afterValueParsingAction: t => secondName = t)
{
    Converter = additionalStringConverter
};

var parser = new ArgumentParser();

parser.AddOptions(firstNameOption, secondNameOption);
parser.AddConverters(defaultStringConverter);

parser.Parse(new string[] { "--f-name", "Name", "--s-name", "Name" });
// firstName: NAME
// secondName: name
```
