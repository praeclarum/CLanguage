# CLanguage

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

### Parsing Header Files

You can use `CLanguageService.ParseTranslationUnit()` to parse C/C++ header files and extract struct/enum definitions, global variables, function declarations, and typedefs without compiling to bytecode.

```csharp
using CLanguage;
using CLanguage.Syntax;

var code = File.ReadAllText("myheader.h");
TranslationUnit tu = CLanguageService.ParseTranslationUnit(code);

// Struct/class definitions
foreach (var (name, structType) in tu.Structures)
{
    Console.WriteLine($"struct {name}:");
    foreach (var member in structType.Members)
        Console.WriteLine($"  {member.Name}: {member.MemberType}");
}

// Enum definitions
foreach (var (name, enumType) in tu.Enums)
{
    Console.WriteLine($"enum {name}:");
    foreach (var member in enumType.Members)
        Console.WriteLine($"  {member.Name} = {member.Value}");
}

// Global variables
foreach (var variable in tu.Variables)
    Console.WriteLine($"var {variable.Name}: {variable.VariableType}");

// Function declarations
foreach (var func in tu.Functions)
    Console.WriteLine($"func {func.Name}: {func.FunctionType}");

// Typedefs
foreach (var (name, type) in tu.Typedefs)
    Console.WriteLine($"typedef {name} = {type}");
```

Key types you can inspect:

- **`CStructType.Members`** — list of `CStructField` (with `Name`, `MemberType`) and `CStructMethod` entries
- **`CEnumType.Members`** — list of `CEnumMember` (with `Name`, `Value`)
- **`CFunctionType`** — has `ReturnType` and `Parameters` (each with `Name` and `ParameterType`)
- **`CompiledVariable`** — has `Name` and `VariableType`

If you need system headers resolved (e.g., `#include <stdint.h>`), use the `CParser` directly and provide an include callback.

