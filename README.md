[![License](https://img.shields.io/badge/License-BSD%203--Clause-blue.svg)](https://github.com/MathosProject/Mathos-Parser/blob/master/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/dt/MathosParser.svg)](https://www.nuget.org/packages/MathosParser/)

Mathos Parser
=============
**Mathos Parser** is a mathematical expression parser for the .NET Framework and .NET Standard. It can parse all kinds of mathematical expressions with the ability to use custom functions, operators, and variables.

* The CIL version (compiles expressions into IL code): https://github.com/MathosProject/Mathos-Parser-CIL

You can find documentation and examples on the [wiki](https://github.com/MathosProject/Mathos-Parser/wiki).

## Features

* Parse and execute mathematical expressions.
* Customize and override existing operators, functions, and variables.
* Supports common mathematical functions, such as pow, round, sqrt, rem, and abs.
* Culture independent.
* And much more!

## Introduction

Mathos Parser is a part of the Mathos Project, a project that aims to provide useful methods, structures, and utilities to make life easier! This math parser is fully independent of the Mathos Core Library, so you can use this library to achieve powerful math parsing without external dependencies.

## How to use

It's really easy to use and understand. In this topic I will try to show you some key features of the library.

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

// Parsing
Assert.AreEqual(30, parser.Parse("a + 猫"));
```

### Custom Operators
```csharp
// Create a parser.
MathParser parser = new MathParser();

// Add a custom operator
parser.Operators.Add("λ", (left, right) => Math.Pow(left, right));

// Parsing
Assert.AreEqual(Math.Pow(3, 2), parser.Parse("3 λ 2"));
```

### Custom Functions
```csharp
// Create a parser.
MathParser parser = new MathParser();

// Add the function and its implementation.
parser.LocalFunctions.Add("timesTwo", inputs => inputs[0] * 2);

// Parsing
Assert.AreEqual(8, parser.Parse("timesTwo(4)"));
```

### Variables Through Parsing
```csharp
// Create a parser.
MathParser parser = new MathParser();

// Define the variable
parser.ProgrammaticallyParse("let a = 25");

// Parsing
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

// Parsing
Assert.AreEqual(3, parser.Parse("clamp(3,-1,5)"));
Assert.AreEqual(-1, parser.Parse("clamp(-5,-1,5)"));
Assert.AreEqual(5, parser.Parse("clamp(8,-1,5)"));
```
