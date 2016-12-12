# PureConfigSharp

A (partial) C# (.NET Core) implementation of [pureconfig](https://github.com/pureconfig/pureconfig).

# Usage

```
PureConfig.Parser pure = new PureConfig.Parser("config.pure");

// Get config values
int port = pure.Get<int>("server.port");
string logPath = pure.Get<string>("server.log_path");

// Write (or overwrite) values to the config file
pure.Put<string>("server.temp_path", @"/tmp/srv/");
```

# Progress

- [X] Int, Double, String, Quantity, Bool
- [ ] Arrays
- [ ] Key scope by indentation
- [X] Key scope by period
- [ ] Comments
- [ ] Referencing
- [ ] Schema support
- [X] Multi-line values denoted by lines ending with '\'
- [ ] Include files
- [ ] Env Vars
- [ ] Write to config files

...