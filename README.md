# CLanguage [![Build Status](https://app.bitrise.io/app/cfa2ebe75549e772/status.svg?token=iZkWdHffpXypGmojf2MgGQ&branch=master)](https://app.bitrise.io/app/cfa2ebe75549e772)

<img src="https://github.com/praeclarum/CLanguage/raw/master/Documentation/Icon.png" height="20"> [![NuGet Package](https://img.shields.io/nuget/v/CLanguage.svg)](https://www.nuget.org/packages/CLanguage)

**CLanguage** is a .NET Standard library that contains a C/C++ parser, a compiler (to its own VM),
and an interpreter (for its VM). It's a very small library that enables you to embed C/C++ scripts into your .NET apps.

It is used to simulate Arduinos in the app [iCircuit](http://icircuitapp.com).
It features cycle counting so that infinite loops and long computations can be paused.

I describe other details of it in my blog entry [Oops, I Wrote a C++ Compiler](https://praeclarum.org/2018/08/27/oops-i-wrote-a-c-compiler.html).

## Usage

There are two stages:

1. Compiling using `CLanguage.Compiler.CCompiler`
2. Interpreting using `CLanguage.Interpreter.CInterpreter`

Machine information, such as pointer sizes, is stored in `MachineInfo` objects.

After compilation, you must create an interpreter, `Reset` it, then `Run` it.

### Simple Evaluation

There is a static `Eval` method on `CLanguageService` to make compiling and executing expressions easier than setting everything up manually.

For example:

```csharp
var result = CLanguageService.Eval("2 + 3");
Assert.AreEqual(5, result);
```

