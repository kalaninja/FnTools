# FnTools

A practical functional library for C# developers.

[![Build status](https://ci.appveyor.com/api/projects/status/9fsm093mmgrsfqtu/branch/master?svg=true)](https://ci.appveyor.com/project/kalaninja/fntools/branch/master)
[![codecov](https://codecov.io/gh/kalaninja/FnTools/branch/master/graph/badge.svg)](https://codecov.io/gh/kalaninja/FnTools)
![Nuget](https://img.shields.io/nuget/v/fntools)

## Quickstart

FnTools can be found [here on NuGet](https://www.nuget.org/packages/fntools/) and can be installed 
with the following command in your Package Manager Console.

```bash
Install-Package FnTools
```

Alternatively if you're using .NET Core then you can install FnTools via the command line interface
with the following command:

```bash
dotnet add package FnTools
```

To have the best experience with FnTools statically import FnTools.Prelude

```c#
using static FnTools.Prelude;
```

## Working with functions

FnTools provides different ways of manipulating functions:

### Def()
Infers function types

```c#
// Doesn't compile
// [CS0815] Cannot assign lambda expression to an implicitly-typed variable
// var id = (int x) => x;

var id = Def((int x) => x);

// Doesn't compile
// [CS0815] Cannot assign method group to an implicitly-typed variable
// var readLine = Console.ReadLine;

var readLine = Def(Console.ReadLine);
```

### Partial()
Does partial application. You can use `__` (double underscore) to bypass function arguments.
```c#
var version =
    Def((string text, int major, int min, char rev) => $"{text}: {major}.{min}{rev}");

var withMin = version.Partial(__, __, 0);
var versioned = withMin.Partial(__, 2);
var withTextAndVersion = versioned.Partial("Version");
var result = withTextAndVersion('b');

result.ShouldBe("Version: 2.0b");
```

### Compose()
Does function composition
```c#
var readInt = Def<string, int>(int.Parse).Compose(Console.ReadLine);
```

### Curry() and Uncurry()
Do currying and uncurrying
```c#
var min = Def<int, int, int>(Math.Min);

var minCurry = min.Curry();
minCurry(1)(2).ShouldBe(1);

var minUncurry = minCurry.Uncurry();
minUncurry(1, 2).ShouldBe(Math.Min(1, 2));
```

### Run()
Executes an action or a function immediately
```c#
var flag = Run(() => false);
var action = Def(() => { flag = true; });
Run(action);

flag.ShouldBe(true);
```

### Apply()
Applies an action or a function to the caller
```c#
(-5)
    .Apply(Math.Abs)
    .Apply(x => Math.Pow(x, 2))
    .Apply(x => Math.Min(x, 30))
    .ShouldBe(25);

var sam = new Person {Name = "Sam", Age = 20};
sam
    .Apply((ref Person x) => { x.Age++; })
    .ShouldBe(new Person {Name = "Sam", Age = 21});
```

### More samples

```c#
var location = new Location {X = 50, Y = 23};
var time = "13:57:59";

Def<string, object[], string>(string.Format)
    .Partial(__, new object[] {location.X, location.Y, time})
    .Apply(LogLocation);

void LogLocation(Func<string, string> log)
{
    log("{2}: {0},{1}").ShouldBe($"{time}: {location.X},{location.Y}");
    log("({0}, {1})").ShouldBe($"({location.X}, {location.Y})");
}
```

```c#
var substring = Def((int start, int length, string str) => str.Substring(start, length));
var firstChars = substring.Partial(0);
var firstChar = firstChars.Partial(1);
var toLower = Def((string str) => str.ToLower());
var lowerFirstChar = toLower.Compose(firstChar);

lowerFirstChar("String").ShouldBe("s");
```

## Data types

### Option
Represents optional values. Instances of Option are either `Some()` or `None`.
```c#
static Option<int> ParseInt(string val) =>
    int.TryParse(val, out var num) ? Some(num) : None;

var result = new StringBuilder().Apply(sb =>
    new[] {"1", "2", "1.7", "NotANumber", "3"}
        .Select(ParseInt)
        .ForEach(o => o.Map(sb.Append))).ToString();

result.ShouldBe("123");
```

### Either
Represents a value of one of two possible types (a disjoint union.) Instances of Either are either `Left()` or `Right()`.
```c#
static Either<string, int> Div(int x, int y)
{
    if (y == 0)
        return Left("cannot divide by 0");
    else
        return Right(x / y);
}

static string PrintResult(Either<string, int> result) =>
    result.Fold(
        left => $"Error: {left}",
        right => right.ToString()
    );

Div(10, 1).Apply(PrintResult).ShouldBe("10");
Div(10, 0).Apply(PrintResult).ShouldBe("Error: cannot divide by 0");
```

### Try
The Try type represents a computation that may either result in an exception (`Failure()`), or return a successfully computed value (`Success()`).
It's similar to, but semantically different from the Either type.
```c#
var tryParse =
    Def((string x) =>
        Try(() => int.Parse(x))
            .Recover<FormatException>(_ => 0)
    );

var tryMax =
    from x in tryParse(Console.ReadLine())
    from y in tryParse(Console.ReadLine())
    from z in tryParse(Console.ReadLine())
    select x + y + z;

tryMax.IsSuccess.ShouldBe(true);

var max = tryMax.Get();
```
