using System;
using NetArgumentParser;
using NetArgumentParser.Options;
using NetArgumentParser.Subcommands;

var parser = new ArgumentParser();

Subcommand slnSubcommand = parser.AddSubcommand("sln", "description1");

slnSubcommand.AddOptions([
    new ValueOption<string>(
        "sln-path",
        afterValueParsingAction: t => Console.WriteLine($"sln: {t}"))
]);

Subcommand addSubSubcommand = slnSubcommand.AddSubcommand("add", "description2");

addSubSubcommand.AddOptions([
    new ValueOption<string>(
        "project-path",
        afterValueParsingAction: t => Console.WriteLine($"project: {t}"))
]);

Subcommand removeSubSubcommand = slnSubcommand.AddSubcommand("remove", "description3");

removeSubSubcommand.AddOptions([
    new ValueOption<string>(
        "project-path",
        afterValueParsingAction: t => Console.WriteLine($"project: {t}"))
]);

Subcommand restoreSubSubSubcommand = addSubSubcommand.AddSubcommand("restore", "description4");

restoreSubSubSubcommand.AddOptions([
    new FlagOption(
        "nuget",
        afterHandlingAction: () => Console.WriteLine($"nuget packages restored")),

    new FlagOption(
        "dependencies",
        afterHandlingAction: () => Console.WriteLine($"dependencies restored"))
]);

_ = parser.Parse([
    "sln",
    "--sln-path",
    "/app/app.sln",
    "add",
    "--project-path",
    "/app/project.csproj",
    "restore",
    "--nuget"]);
