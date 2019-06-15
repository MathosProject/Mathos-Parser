# Version 3.0.0 (???)
* [Fixed some issues with leading minus signs.][cfb4fd1c4cb5d9d622e98d3374d6b49fe3ebd908] (See [#18][18] and [#21][21])
* [Changed the rounding behavior of `round` from `MidpointRounding.ToEven` to `MidpointRounding.AwayFromZero`. This means 0.5 goes to 1 instead of 0.][0924543515062b1df0ecbe8450beb4f26edbbc16]
* [Added the boolean operators ">=", "<=", "!=", and "==".][e568d2d52f62c337846cc098868eb86bde9474a2] (See [#20][20])
* [Fixed a divide-by-zero error with the "/" and ":" operators. They now return zero in such case.][fe03fc9d9182e96fef16b5d692b7a78b9c9110f4]
* [Only target the .NET Standard (1.0 - 2.0).][9187f397f6be341979f1e54b34fac42f05e89c35]
* [Added `MathParserException` for parser-related exceptions.][66c256a7a887d58a17a48abebee918d705a07f1c]
* [Added `random` to the list of predefined functions for getting random numbers within a range:][01ccd20c12ac59a20307f4fff9090a780df62adc]

```
- 0 arguments: 0 inclusive - 1 exclusive
- 1 argument : 0 inclusive - arg1 exclusive
- 2 arguments: arg1 inclusive - arg2 exclusive
```

* [The variable declarator ("let") may now be changed.][01c297bdaf870db5690e71832d966f6924c6cc37]
* [Fixed an issue where numbers would lose their sign.][4fef360f03a6c230fcbdf71e2a5e32fe0d53f420]
* [Added `BooleanParser` for parsing and evaluating boolean expressions.][eceb5378b500c6e4abf495b299c8dbe6dcd41991]
* [Added `ScriptParser` and other related items for parsing and evaluating "script"-like expressions.][55c610ebf4f8b55a3296933ed61a6f071501028f]

`ScriptParser` can handle things like if statements:
```
let x = 5

0
if (x >= 4)
    1

end if
```

* Added more unit tests.
* Documentation improvements.
* Simplified some internal logic.
* Other small changes and improvements.

### Special Thanks
dennisvg111
Ctznkane525

[18]: https://github.com/MathosProject/Mathos-Parser/issues/18
[21]: https://github.com/MathosProject/Mathos-Parser/issues/21
[20]: https://github.com/MathosProject/Mathos-Parser/issues/20
[cfb4fd1c4cb5d9d622e98d3374d6b49fe3ebd908]: https://github.com/MathosProject/Mathos-Parser/commit/cfb4fd1c4cb5d9d622e98d3374d6b49fe3ebd908
[0924543515062b1df0ecbe8450beb4f26edbbc16]: https://github.com/MathosProject/Mathos-Parser/commit/0924543515062b1df0ecbe8450beb4f26edbbc16
[e568d2d52f62c337846cc098868eb86bde9474a2]: https://github.com/MathosProject/Mathos-Parser/commit/e568d2d52f62c337846cc098868eb86bde9474a2
[fe03fc9d9182e96fef16b5d692b7a78b9c9110f4]: https://github.com/MathosProject/Mathos-Parser/commit/fe03fc9d9182e96fef16b5d692b7a78b9c9110f4
[9187f397f6be341979f1e54b34fac42f05e89c35]: https://github.com/MathosProject/Mathos-Parser/commit/9187f397f6be341979f1e54b34fac42f05e89c35
[66c256a7a887d58a17a48abebee918d705a07f1c]: https://github.com/MathosProject/Mathos-Parser/commit/66c256a7a887d58a17a48abebee918d705a07f1c
[01ccd20c12ac59a20307f4fff9090a780df62adc]: https://github.com/MathosProject/Mathos-Parser/commit/01ccd20c12ac59a20307f4fff9090a780df62adc
[01c297bdaf870db5690e71832d966f6924c6cc37]: https://github.com/MathosProject/Mathos-Parser/commit/01c297bdaf870db5690e71832d966f6924c6cc37
[4fef360f03a6c230fcbdf71e2a5e32fe0d53f420]: https://github.com/MathosProject/Mathos-Parser/commit/4fef360f03a6c230fcbdf71e2a5e32fe0d53f420
[eceb5378b500c6e4abf495b299c8dbe6dcd41991]: https://github.com/MathosProject/Mathos-Parser/commit/eceb5378b500c6e4abf495b299c8dbe6dcd41991
[55c610ebf4f8b55a3296933ed61a6f071501028f]: https://github.com/MathosProject/Mathos-Parser/commit/55c610ebf4f8b55a3296933ed61a6f071501028f

# Version 2.0.0 (2018-6-24)
* Target the .Net Standard.
* Dropped support of for `decimal` in favor of `double`.
* `OperatorList` has been removed and `OperatorAction` is now `Operators`.
* Added a `CultureInfo` parameter to the constructor for better localization.
* Added ln, acos, asin, atan, and ceil to the list of predefined functions.
* Leading zeros may now be omitted before decimal points.

```
0.5 # Before
 .5 # After
```

* Minor performance improvements.
* Updates to the documentation
* Other small changes and improvements.

# Version 1.0.10.1 (2015-1-3)
* Fixed a problem with functions and unary operators. (#1)

The following would throw an exception:
```
-sin(5)
```

* Fixed some internal logic.
