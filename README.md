Mathos Parser
=============
**Mathos Parser** is a mathematical expression parser built on top of the .NET Framework, which parses all kinds of mathematical expressions with the ability to use custom functions, operators, and variables.

* The PCL compliant version: https://github.com/MathosProject/Mathos-Parser-Portable
* The CIL version (compiles expressions into IL code): https://github.com/MathosProject/Mathos-Parser-CIL

## Features

* Parse mathematical expressions.
* Add conditional statements.
* Customize and override existing operators, functions, and variables.
* Supports common mathematical functions, such as pow, round, sqrt, rem, and abs.
* Culture independent.
* And much more!

## Introduction

Mathos Parser is a part of Mathos Project, a project that aims to provide useful methods, structures, and utilities, to make life easier! This math parser is fully independent of the Mathos Core Library, so you can use this library to achieve powerful math parsing without external dependencies.

## How to use

It's really easy to use and understand. In this topic I will try to show you some key features of the library.

* Custom operators
* Custom functions
* Multi-argument functions
* Custom variables
* Variables through parsing

### Custom Operators

```csharp
// Create a parser.
MathParser parser = new MathParser();

// Add the operator to the operator list.
parser.OperatorList.Add("^");

// Add an action for the newly added operator.
parser.OperatorAction.Add("^", delegate(double numA, double numB)
{
    return (decimal) Math.Pow((double) numA, (double) numB);
});

// Parsing
Assert.IsTrue(parser.Parse("3^2") == Math.Pow(3,2));
```

### Custom Functions
```csharp
// Create a parser.
MathParser parser = new MathParser();

// Add the function and its contents.
parser.LocalFunctions.Add("timesTwo", inputs => inputs[0] * 2);

// Parsing
Assert.IsTrue(parser.Parse("timesTwo(4)") == 8);
```

### Multi-Argument Functions
```csharp
// Create a parser.
MathParser parser = new MathParser();

// Add the function and its contents.
parser.LocalFunctions.Add("log", delegate(double[] inputs)
{
    // inputs[0] is the number.
    // inputs[1] is the base (optional).
    
    if(inputs.Length == 1)
        return Math.Log(inputs[0]);
    else if(inputs.Length == 2)
        return Math.Log(inputs[0], inputs[1]);
    else
        return 0; // Error.
});

// Parsing
Assert.IsTrue(parser.Parse("log(100)") == 2);
Assert.IsTrue(parser.Parse("log(8, 2)") == 3);
```

### Custom Variables
```csharp
// Create a parser.
MathParser parser = new MathParser();

// Add the variable.
parser.LocalVariables.Add("a", 25);

// Parsing
Assert.IsTrue(parser.Parse("a+5") == 30);
```

### Variables Through Parsing
```csharp
// Create a parser.
MathParser parser = new MathParser();

// Add the variable
parser.ProgrammaticallyParse("let a = 25");

// Parsing
Assert.IsTrue(parser.Parse("a+5") == 30);
```
