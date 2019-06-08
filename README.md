[![License](https://img.shields.io/github/license/MathosProject/Mathos-Parser.svg?label=LICENSE&style=for-the-badge)](https://github.com/MathosProject/Mathos-Parser/blob/master/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/dt/MathosParser.svg?label=NUGET%20DOWNLOADS&style=for-the-badge)](https://www.nuget.org/packages/MathosParser/)

Mathos Parser
=============
**Mathos Parser** is a mathematical expression parser and evaluator for the .NET Standard. It can parse various kinds of mathematical expressions out of the box and can be extended with custom functions, operators, and variables.

* The CIL version (compiles expressions into IL code): https://github.com/MathosProject/Mathos-Parser-CIL (Outdated)

Documentation and examples are on the [wiki](https://github.com/MathosProject/Mathos-Parser/wiki).

## Features

* Parse and evaluate mathematical expressions out of the box.
* Customize and override existing operators, functions, and variables.
* Culture independent.
* And much more!

## Introduction

Mathos Parser is a part of the Mathos Project, a project that aims to provide useful mathematics APIs and utilities to make life easier. This math parser is fully independent of the [Mathos Core Library](https://github.com/MathosProject/Mathos-Project), so this library achieves powerful math parsing and evaluation without external dependencies.

## How to use

Mathos Parser is very easy to use and understand. This section provides examples for the following:

* Custom variables
* Custom operators
* Custom functions
* Variables through parsing
* Multi-argument functions

### Custom Variables
```csharp
// Create a parser.
MathParser parser = new MathParser();

// Add a variable.
parser.LocalVariables.Add("a", 25);

// How about another.
parser.LocalVariables.Add("猫", 5);

// Use the variables as you would normally.
Assert.AreEqual(30, parser.Parse("a + 猫"));
```

### Custom Operators
```csharp
// Create a parser.
MathParser parser = new MathParser();

// Add the custom operator.
parser.Operators.Add("λ", (left, right) => Math.Pow(left, right));

// Evaluate using the new operator.
Assert.AreEqual(Math.Pow(3, 2), parser.Parse("3 λ 2"));
```

### Custom Functions
```csharp
// Create a parser.
MathParser parser = new MathParser();

// Add the function.
parser.LocalFunctions.Add("timesTwo", inputs => inputs[0] * 2);

// Use the new function.
Assert.AreEqual(8, parser.Parse("timesTwo(4)"));
```

### Variables Through Parsing
```csharp
// Create a parser.
MathParser parser = new MathParser();

// Define the variable.
parser.ProgrammaticallyParse("let a = 25");

// Evaluation.
Assert.AreEqual(30, parser.Parse("a + 5"));
```

### Multi-Argument Functions
```csharp
// Create a parser.
MathParser parser = new MathParser();

// Add the function and its implementation.
parser.LocalFunctions.Add("clamp", delegate (double[] inputs)
{
    // The value.
    var value = inputs[0];

    // The maximum value.
    var min = inputs[1];

    // The minimum value.
    var max = inputs[2];

    if (value > max)
        return max;

    if (value < min)
        return min;

    return value;
});

// Use the new function.
Assert.AreEqual(3, parser.Parse("clamp(3,-1,5)"));
Assert.AreEqual(-1, parser.Parse("clamp(-5,-1,5)"));
Assert.AreEqual(5, parser.Parse("clamp(8,-1,5)"));
```
