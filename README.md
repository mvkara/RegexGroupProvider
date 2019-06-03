# Regex Group Provider

This is a type provider that wraps System.Text.RegularExpressions.Match and converts a regex into a type-safe type based on groups defined within.
Note: It does not as of yet attempt to do any parsing of the regex inside a group to infer return types.

As an example:

```
type RegexProviderTestTwo = RegexGroupProvider<"test(?<group1>.*)/(?<group2>.*)">
```

will expose a type with the "group1" and "group2" members.

For usage examples see the test suite.

Note: I'm aware of another Regex Provider with potentially more features but at time of writing isn't NET Core 2.0 compatible despite a PR from 2017 to migrate it over. Doubt it will be looked at anytime soon.

## How to build ##

    .paket\paket.exe update

    dotnet build -c release

    .paket\paket.exe pack src\RegexGroupProvider.Runtime\paket.template --version 0.0.1