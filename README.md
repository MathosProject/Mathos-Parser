[![Build Status](https://travis-ci.org/MathosProject/Mathos-Parser.svg?branch=master)](https://travis-ci.org/MathosProject/Mathos-Parser)

Mathos Parser
=============
**Mathos Parser** is a mathematical expression parser, built on top of the .NET Framework, which allows you to parse all kinds of mathematical expressions, and in addition, add your own custom functions, operators, and variables (see the online demo).

* The PCL compliant version: https://github.com/MathosProject/Mathos-Parser-Portable
* The CIL version (compiles expressions into IL code): https://github.com/MathosProject/Mathos-Parser-CIL

## Features

* Parse all kinds of mathematical expressions.
* Add conditional statements.
* Customize operators. Add/edit existing operators, change behaviour of operators.
* Programatically add variables before and on the run time.
* Custom own functions with almost unlimited amount of arguments.
* Using trig functions: sine, cosine, tangents, and also: arc sine, arc cosine, arc tangent.
* Supports almost all of the functions available in System.Math, such as: pow, round, sqrt, rem, abs, and more!
* Culture independent. No matter on what machine the library is being used - the same configurations for everyone.
* Decimal operations.
* And much more!

## Introduction

Mathos Parser is a part of Mathos Project, a project that provides useful methods, structures, etc, to make the life a little bit easier! This math parser is fully independent of Mathos project, so you can just use this library to archive a powerful math parsing experience.

## How to use

It's really easy to use and understand this math parser. In this topic I will try to show you some (not all) key features of this library.

* Adding a custom operator.
* Adding a custom function.
* Functions with more than one argument.
* Programatically add variables.

### Adding custom operator

````csharp
// declaring the parser
MathParser parser = new MathParser();

//customize the operator list
parser.OperatorList = new List<string>() { "^", "%", "*", ":", "/", "+", "-", ">", "<", "=" };

// adding sqrt to the OperatorAction list
parser.OperatorAction.Add("^", delegate(decimal numA, decimal numB)
{
    return (decimal)Math.Pow((double)numA, (double)numB);
});

// parsing and comparing
Assert.IsTrue(parser.Parse("3^2") == (decimal)Math.Pow (3,2));
````
### Adding custom function
````csharp
public void CustomFunctions()
{
    /*
     * This test demonstrates three ways of adding a function
     * to the Math Parser
     * 
     * 1) directly pointing to the function
     * 2) lambda expression
     * 3) anonymous method
     */

    MathParser parser = new MathParser();

    //for long functions
    parser.LocalFunctions.Add("numberTimesTwo", NumberTimesTwoCustomFunction); // adding the function
    decimal resultA = parser.Parse("numberTimesTwo(3)");

    //for short functions, use lambda expression, or anonymous method
    // 1) using lambda epxression (recommended)
    parser.LocalFunctions.Add("square", x => x[0] * x[0]);
    decimal resultB = parser.Parse("square(4)");

    // 2) using anonymous method
    parser.LocalFunctions.Add("cube", delegate(decimal[] x)
    {
        return x[0] * x[0] * x[0];
    });
    decimal resultC = parser.Parse("cube(2)");

}
public decimal NumberTimesTwoCustomFunction(decimal[] input)
{
    return input[0] * 2;
}
````
### Functions with more than one operator
````csharp
/*
 * This example demonstrates the "anonymous method" way of adding
 * a function that can take more than one agument.
 */

 MathParser parser = new MathParser();

//for long functions
parser.LocalFunctions.Add("log", delegate(decimal[] input) // adding the function
{
    // input[0] is the number
    // input[1] is the base

  if (input.Length == 1)
  {
      return (decimal)Math.Log((double)input[0]);
  }
  else if (input.Length == 2)
  {
      return (decimal)Math.Log((double)input[0], (double)input[1]);
  }
  else
  {
      return 0; // false
  }
});

decimal resultA = parser.Parse("log(2)");
decimal resultB = parser.Parse("log(2,3)");
````
### Programatically add variables
```csharp
/* 
* when parsing an expression that requires 
* for instance a variable name declaration 
* or change, use ProgramaticallyParse().
*/
MathParser parser = new MathParser();
          
// first way, using let varname = value
decimal resultA = parser.ProgrammaticallyParse("let a = 2pi");
Assert.IsTrue (parser.Parse ("a") == (decimal)Math.PI*2);

// second way, using varname :=  value
decimal resultC = parser.ProgrammaticallyParse("b := 20");
Assert.IsTrue(parser.Parse("b") == 20);

// third way, using let varname be value
decimal resultD = parser.ProgrammaticallyParse("let c be 25");
Assert.IsTrue(resultD == 25);
```
