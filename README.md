# PureConfigSharp

A (partial) C# (.NET Core) implementation of [pureformat](https://github.com/pureformat/pureformat).

# Usage

```
PureConfig.Parser pure = new PureConfig.Parser("config.pure");

// Get config values
int port = pure.Get<int>("server.port");
string logPath = pure.Get<string>("server.log_path");
int bufferLength = pure.Get<"main.buffer length");

```

# Progress

- [X] Int, Double, String, Quantity, Bool
- [ ] Int, Double overflow
- [ ] Arrays
- [ ] Key scope by indentation
- [X] Key scope by period
- [X] Comments
- [ ] Backslash-escaped characters in strings (e.g. \n)
- [ ] Referencing
- [ ] Schema support
- [X] Multi-line values denoted by lines ending with '\'
- [ ] Include files
- [ ] Write to config files
- [ ] Meaningful error messages during parse failures
- [X] Meaningful error messages when failing to retrieve a value

...
