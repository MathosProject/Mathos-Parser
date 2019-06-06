/* 
 * Copyright (C) 2012-2018, Mathos Project.
 * All rights reserved.
 * 
 * Please see the license file in the project folder,
 * or go to https://github.com/MathosProject/Mathos-Parser/blob/master/LICENSE.md.
 * 
 * Please feel free to ask me directly at my email!
 *  artem@artemlos.net
 */

using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace Mathos.Parser
{
    /// <summary>
    /// This is a mathematical expression parser that allows you to perform calculations on string values.
    /// </summary>
    public class MathParser
    {
        private const char GEQ_SIGN = (char)8805;
        private const char LEQ_SIGN = (char)8804;
        private const char NEQ_SIGN = (char)8800;

        #region Properties

        /// <summary>
        /// All operators that you want to define should be inside this property.
        /// </summary>
        public Dictionary<string, Func<double, double, double>> Operators { get; set; }

        /// <summary>
        /// All functions that you want to define should be inside this property.
        /// </summary>
        public Dictionary<string, Func<double[], double>> LocalFunctions { get; set; }

        /// <summary>
        /// All variables that you want to define should be inside this property.
        /// </summary>
        public Dictionary<string, double> LocalVariables { get; set; }

        /// <summary>
        /// When converting the result from the Parse method or ProgrammaticallyParse method ToString(),
        /// please use this culture info.
        /// </summary>
        public CultureInfo CultureInfo { get; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the MathParser class, and optionally with
        /// predefined functions, operators, and variables.
        /// </summary>
        /// <param name="loadPreDefinedFunctions">This will load abs, cos, cosh, arccos, sin, sinh, arcsin, tan, tanh, arctan, sqrt, rem, and round.</param>
        /// <param name="loadPreDefinedOperators">This will load %, *, :, /, +, -, >, &lt;, and =</param>
        /// <param name="loadPreDefinedVariables">This will load pi, tao, e, phi, major, minor, pitograd, and piofgrad.</param>
        /// <param name="cultureInfo">The culture info to use when parsing. If null, defaults to invariant culture.</param>
        public MathParser(bool loadPreDefinedFunctions = true, bool loadPreDefinedOperators = true, bool loadPreDefinedVariables = true, CultureInfo cultureInfo = null)
        {
            if (loadPreDefinedOperators)
            {
                Operators = new Dictionary<string, Func<double, double, double>>()
                {
                    ["^"] = Math.Pow,
                    ["%"] = (a, b) => a % b,
                    [":"] = (a, b) => a / b,
                    ["/"] = (a, b) => a / b,
                    ["*"] = (a, b) => a * b,
                    ["-"] = (a, b) => a - b,
                    ["+"] = (a, b) => a + b,

                    [">"] = (a, b) => a > b ? 1 : 0,
                    ["<"] = (a, b) => a < b ? 1 : 0,
                    ["" + GEQ_SIGN] = (a, b) => a > b || Math.Abs(a - b) < 0.00000001 ? 1 : 0,
                    ["" + LEQ_SIGN] = (a, b) => a < b || Math.Abs(a - b) < 0.00000001 ? 1 : 0,
                    ["" + NEQ_SIGN] = (a, b) => Math.Abs(a - b) < 0.00000001 ? 0 : 1,
                    ["="] = (a, b) => Math.Abs(a - b) < 0.00000001 ? 1 : 0
                };
            }
            else
            {
                Operators = new Dictionary<string, Func<double, double, double>>();
            }
            if (loadPreDefinedFunctions)
            {
                LocalFunctions = new Dictionary<string, Func<double[], double>>()
                {
                    ["abs"] = inputs => Math.Abs(inputs[0]),

                    ["cos"] = inputs => Math.Cos(inputs[0]),
                    ["cosh"] = inputs => Math.Cosh(inputs[0]),
                    ["acos"] = inputs => Math.Acos(inputs[0]),
                    ["arccos"] = inputs => Math.Acos(inputs[0]),

                    ["sin"] = inputs => Math.Sin(inputs[0]),
                    ["sinh"] = inputs => Math.Sinh(inputs[0]),
                    ["asin"] = inputs => Math.Asin(inputs[0]),
                    ["arcsin"] = inputs => Math.Asin(inputs[0]),

                    ["tan"] = inputs => Math.Tan(inputs[0]),
                    ["tanh"] = inputs => Math.Tanh(inputs[0]),
                    ["atan"] = inputs => Math.Atan(inputs[0]),
                    ["arctan"] = inputs => Math.Atan(inputs[0]),

                    ["sqrt"] = inputs => Math.Sqrt(inputs[0]),
                    ["pow"] = inputs => Math.Pow(inputs[0], inputs[1]),
                    ["root"] = inputs => Math.Pow(inputs[0], 1 / inputs[1]),
                    ["rem"] = inputs => Math.IEEERemainder(inputs[0], inputs[1]),

                    ["sign"] = inputs => Math.Sign(inputs[0]),
                    ["exp"] = inputs => Math.Exp(inputs[0]),

                    ["floor"] = inputs => Math.Floor(inputs[0]),
                    ["ceil"] = inputs => Math.Ceiling(inputs[0]),
                    ["ceiling"] = inputs => Math.Ceiling(inputs[0]),
                    ["round"] = inputs => Math.Round(inputs[0], MidpointRounding.AwayFromZero),
                    ["truncate"] = inputs => inputs[0] < 0 ? -Math.Floor(-inputs[0]) : Math.Floor(inputs[0]),

                    ["log"] = inputs =>
                    {
                        switch (inputs.Length)
                        {
                            case 1:
                                return Math.Log10(inputs[0]);
                            case 2:
                                return Math.Log(inputs[0], inputs[1]);
                            default:
                                return 0;
                        }
                    },

                    ["ln"] = inputs => Math.Log(inputs[0])
                };
            }
            else
            {
                LocalFunctions = new Dictionary<string, Func<double[], double>>();
            }
            if (loadPreDefinedVariables)
            {
                LocalVariables = new Dictionary<string, double>(8)
                {
                    ["pi"] = 3.14159265358979,
                    ["tao"] = 6.28318530717959,

                    ["e"] = 2.71828182845905,
                    ["phi"] = 1.61803398874989,
                    ["major"] = 0.61803398874989,
                    ["minor"] = 0.38196601125011,

                    ["pitograd"] = 57.2957795130823,
                    ["piofgrad"] = 0.01745329251994
                };
            }
            else
            {
                LocalVariables = new Dictionary<string, double>();
            }
            CultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Enter the math expression in form of a string.
        /// </summary>
        /// <param name="mathExpression">The math expression to parse.</param>
        /// <returns>The result of executing <paramref name="mathExpression"/>.</returns>
        public double Parse(string mathExpression)
        {
            return MathParserLogic(Lexer(mathExpression));
        }

        /// <summary>
        /// Enter the math expression in form of a list of tokens.
        /// </summary>
        /// <param name="mathExpression">The math expression to parse.</param>
        /// <returns>The result of executing <paramref name="mathExpression"/>.</returns>
        public double Parse(IReadOnlyCollection<string> mathExpression)
        {
            return MathParserLogic(new List<string>(mathExpression));
        }

        /// <summary>
        /// Enter the math expression in form of a string. You might also add/edit variables using "let" keyword.
        /// For example, "let sampleVariable = 2+2".
        /// 
        /// Another way of adding/editing a variable is to type "varName := 20"
        /// 
        /// Last way of adding/editing a variable is to type "let varName be 20"
        /// </summary>
        /// <param name="mathExpression">The math expression to parse.</param>
        /// <param name="correctExpression">If true, correct <paramref name="correctExpression"/> of any typos.</param>
        /// <param name="identifyComments">If true, treat "#", "#{", and "}#" as comments.</param>
        /// <returns>The result of executing <paramref name="mathExpression"/>.</returns>
        public double ProgrammaticallyParse(string mathExpression, bool correctExpression = true, bool identifyComments = true)
        {
            if (identifyComments)
            {
                // Delete Comments #{Comment}#
                mathExpression = System.Text.RegularExpressions.Regex.Replace(mathExpression, "#\\{.*?\\}#", "");

                // Delete Comments #Comment
                mathExpression = System.Text.RegularExpressions.Regex.Replace(mathExpression, "#.*$", "");
            }

            if (correctExpression)
            {
                // this refers to the Correction function which will correct stuff like artn to arctan, etc.
                mathExpression = Correction(mathExpression);
            }

            string varName;
            double varValue;

            if (mathExpression.Contains("let"))
            {
                if (mathExpression.Contains("be"))
                {
                    varName = mathExpression.Substring(mathExpression.IndexOf("let", StringComparison.Ordinal) + 3,
                        mathExpression.IndexOf("be", StringComparison.Ordinal) -
                        mathExpression.IndexOf("let", StringComparison.Ordinal) - 3);
                    mathExpression = mathExpression.Replace(varName + "be", "");
                }
                else
                {
                    varName = mathExpression.Substring(mathExpression.IndexOf("let", StringComparison.Ordinal) + 3,
                        mathExpression.IndexOf("=", StringComparison.Ordinal) -
                        mathExpression.IndexOf("let", StringComparison.Ordinal) - 3);
                    mathExpression = mathExpression.Replace(varName + "=", "");
                }

                varName = varName.Replace(" ", "");
                mathExpression = mathExpression.Replace("let", "");

                varValue = Parse(mathExpression);

                if (LocalVariables.ContainsKey(varName))
                {
                    LocalVariables[varName] = varValue;
                }
                else
                {
                    LocalVariables.Add(varName, varValue);
                }
                return varValue;
            }

            if (!mathExpression.Contains(":="))
            {
                return Parse(mathExpression);
            }

            //mathExpression = mathExpression.Replace(" ", ""); // remove white space
            varName = mathExpression.Substring(0, mathExpression.IndexOf(":=", StringComparison.Ordinal));
            mathExpression = mathExpression.Replace(varName + ":=", "");

            varValue = Parse(mathExpression);
            varName = varName.Replace(" ", "");

            if (LocalVariables.ContainsKey(varName))
            {
                LocalVariables[varName] = varValue;
            }
            else
            {
                LocalVariables.Add(varName, varValue);
            }
            return varValue;
        }

        /// <summary>
        /// This will convert a string expression into a list of tokens that can be later executed by Parse or ProgrammaticallyParse methods.
        /// </summary>
        /// <param name="mathExpression">The math expression to tokenize.</param>
        /// <returns>The resulting tokens of <paramref name="mathExpression"/>.</returns>
        public IReadOnlyCollection<string> GetTokens(string mathExpression)
        {
            return Lexer(mathExpression);
        }

        #region Core

        /// <summary>
        /// This will correct sqrt() and arctan() written in different ways only.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string Correction(string input)
        {
            // Word corrections

            input = System.Text.RegularExpressions.Regex.Replace(input, "\\b(sqr|sqrt)\\b", "sqrt", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            input = System.Text.RegularExpressions.Regex.Replace(input, "\\b(atan2|arctan2)\\b", "arctan2", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //... and more

            return input;
        }

        /// <summary>
        /// Tokenizes <paramref name="expr"/>.
        /// </summary>
        /// <param name="expr">The expression to tokenize.</param>
        /// <returns>The tokens.</returns>
        private List<string> Lexer(string expr)
        {
            var token = "";
            var tokens = new List<string>();

            expr = expr.Replace("+-", "-");
            expr = expr.Replace("-+", "-");
            expr = expr.Replace("--", "+");
            expr = expr.Replace("==", "=");
            expr = expr.Replace(">=", "" + GEQ_SIGN);
            expr = expr.Replace("<=", "" + LEQ_SIGN);
            expr = expr.Replace("!=", "" + NEQ_SIGN);

            for (var i = 0; i < expr.Length; i++)
            {
                var ch = expr[i];

                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }

                if (char.IsLetter(ch))
                {
                    if (i != 0 && (char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
                    {
                        tokens.Add("*");
                    }

                    token += ch;

                    while (i + 1 < expr.Length && char.IsLetterOrDigit(expr[i + 1]))
                    {
                        token += expr[++i];
                    }
                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if (char.IsDigit(ch))
                {
                    token += ch;

                    while (i + 1 < expr.Length && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
                    {
                        token += expr[++i];
                    }
                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if (ch == '.')
                {
                    token += ch;

                    while (i + 1 < expr.Length && char.IsDigit(expr[i + 1]))
                    {
                        token += expr[++i];
                    }
                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if (i + 1 < expr.Length &&
                    (ch == '-' || ch == '+') &&
                    char.IsDigit(expr[i + 1]) &&
                    (i == 0 || (tokens.Count > 0 && Operators.ContainsKey(tokens.Last())) || i - 1 > 0 && expr[i - 1] == '('))
                {
                    // if the above is true, then the token for that negative number will be "-1", not "-","1".
                    // to sum up, the above will be true if the minus sign is in front of the number, but
                    // at the beginning, for example, -1+2, or, when it is inside the brakets (-1), or when it comes after another operator.
                    // NOTE: this works for + as well!

                    token += ch;

                    while (i + 1 < expr.Length && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
                        token += expr[++i];

                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if (ch == '(')
                {
                    if (i != 0 && (char.IsDigit(expr[i - 1]) || char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
                    {
                        tokens.Add("*");
                        tokens.Add("(");
                    }
                    else
                    {
                        tokens.Add("(");
                    }
                }
                else
                {
                    tokens.Add(ch.ToString());
                }
            }

            return tokens;
        }

        private double MathParserLogic(List<string> tokens)
        {
            // Variables replacement
            for (var i = 0; i < tokens.Count; i++)
            {
                if (LocalVariables.Keys.Contains(tokens[i]))
                {
                    tokens[i] = LocalVariables[tokens[i]].ToString(CultureInfo);
                }
            }

            while (tokens.IndexOf("(") != -1)
            {
                // getting data between "(" and ")"
                var open = tokens.LastIndexOf("(");
                var close = tokens.IndexOf(")", open); // in case open is -1, i.e. no "(" // , open == 0 ? 0 : open - 1

                if (open >= close)
                {
                    throw new ArithmeticException("No closing bracket/parenthesis. Token: " + open.ToString(CultureInfo));
                }

                var roughExpr = new List<string>();

                for (var i = open + 1; i < close; i++)
                {
                    roughExpr.Add(tokens[i]);
                }
                double tmpResult;

                var args = new List<double>();
                var functionName = tokens[open == 0 ? 0 : open - 1];

                if (LocalFunctions.Keys.Contains(functionName))
                {
                    if (roughExpr.Contains(","))
                    {
                        // converting all arguments into a decimal array
                        for (var i = 0; i < roughExpr.Count; i++)
                        {
                            var defaultExpr = new List<string>();
                            var firstCommaOrEndOfExpression =
                                roughExpr.IndexOf(",", i) != -1
                                    ? roughExpr.IndexOf(",", i)
                                    : roughExpr.Count;

                            while (i < firstCommaOrEndOfExpression)
                            {
                                defaultExpr.Add(roughExpr[i++]);
                            }
                            args.Add(defaultExpr.Count == 0 ? 0 : BasicArithmeticalExpression(defaultExpr));
                        }

                        // finally, passing the arguments to the given function
                        tmpResult = double.Parse(LocalFunctions[functionName](args.ToArray()).ToString(CultureInfo), CultureInfo);
                    }
                    else
                    {
                        // but if we only have one argument, then we pass it directly to the function
                        tmpResult = double.Parse(LocalFunctions[functionName](new[]
                        {
                            BasicArithmeticalExpression(roughExpr)
                        }).ToString(CultureInfo), CultureInfo);
                    }
                }
                else
                {
                    // if no function is need to execute following expression, pass it
                    // to the "BasicArithmeticalExpression" method.
                    tmpResult = BasicArithmeticalExpression(roughExpr);
                }

                // when all the calculations have been done
                // we replace the "opening bracket with the result"
                // and removing the rest.
                tokens[open] = tmpResult.ToString(CultureInfo);
                tokens.RemoveRange(open + 1, close - open);

                if (LocalFunctions.Keys.Contains(functionName))
                {
                    // if we also executed a function, removing
                    // the function name as well.
                    tokens.RemoveAt(open - 1);
                }
            }

            // at this point, we should have replaced all brackets
            // with the appropriate values, so we can simply
            // calculate the expression. it's not so complex
            // any more!
            return BasicArithmeticalExpression(tokens);
        }

        private double BasicArithmeticalExpression(List<string> tokens)
        {
            // PERFORMING A BASIC ARITHMETICAL EXPRESSION CALCULATION
            // THIS METHOD CAN ONLY OPERATE WITH NUMBERS AND OPERATORS
            // AND WILL NOT UNDERSTAND ANYTHING BEYOND THAT.

            switch (tokens.Count)
            {
                case 1:
                    return double.Parse(tokens[0], CultureInfo);
                case 2:
                    var op = tokens[0];

                    if (op == "-" || op == "+")
                    {
                        var first = op == "+" ? "" : (tokens[1].Substring(0, 1) == "-" ? "" : "-");

                        return double.Parse(first + tokens[1], CultureInfo);
                    }

                    return Operators[op](0, double.Parse(tokens[1], CultureInfo));
                case 0:
                    return 0;
            }

            foreach (var op in Operators)
            {
                int opPlace;

                while ((opPlace = tokens.IndexOf(op.Key)) != -1)
                {
                    var rhs = double.Parse(tokens[opPlace + 1], CultureInfo);

                    if (op.Key == "-" && opPlace == 0)
                    {
                        var result = op.Value(0.0, rhs);
                        tokens[0] = result.ToString(CultureInfo);
                        tokens.RemoveRange(opPlace + 1, 1);
                    }
                    else
                    {
                        var result = op.Value(double.Parse(tokens[opPlace - 1], CultureInfo), rhs);
                        tokens[opPlace - 1] = result.ToString(CultureInfo);
                        tokens.RemoveRange(opPlace, 2);
                    }
                }
            }

            return double.Parse(tokens[0], CultureInfo);
        }

        #endregion
    }
}