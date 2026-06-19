## About
**NetArgumentParser** is a cross-platform, free, open source C# library for parsing command-line options, arguments and subcommands. This library contains the main features of popular argument parsers such as `argparse`, as well as many of its own.

**NetArgumentParser** supports many frameworks, so you can use it in most of your projects. Moreover, you can find clear examples of using this library [here](https://github.com/yakovypg/NetArgumentParser/blob/main/Examples). You may also find it helpful to look at the example of **NetArgumentParser** integration in the [Ypdf.CommandLine](https://github.com/yakovypg/Ypdf.CommandLine) project — a cross-platform command-line tool for PDF processing. It is a full, end-to-end integration example.

## Main Features
This library supports the following main features:
- Parse short-named options (such as `-v`).
- Parse long-named options (such as `--version` or `-version`).
- Parse windows-based options (such as `/v` or `/version`).
- Parse compound options (such as `-lah`).
- Parse nested subcommands (such as `app subcommand subsubcommand`).
- Extract extra arguments.
- Provide a lot of default option types.
- Support custom options and converters.
- Configure command-line help generation and output stream.
- Generation of argument parser using attributes.

Many other features with examples you can find in [documentation](https://github.com/yakovypg/NetArgumentParser/blob/main/DOCUMENTATION.md).
