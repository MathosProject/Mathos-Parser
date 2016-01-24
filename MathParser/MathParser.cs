/* 
 * Copyright (C) 2012-2016 Mathos Project,
 * All rights reserved.
 * 
 * Please see the license file in the project folder,
 * or, go to https://github.com/MathosProject/Mathos-Parser/blob/master/LICENSE.
 * 
 * Please feel free to ask me directly at my email!
 *  artem@artemlos.net
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Mathos.Parser
{
    /// <summary>
    /// This is a mathematical expression parser that allows you to parser a string value,
    /// perform the required calculations, and return a value in form of a decimal.
    /// </summary>
    public class MathParser
    {
        /// <summary>
        /// This constructor will add some basic operators, functions, and variables
        /// to the parser. Please note that you are able to change that using
        /// boolean flags
        /// </summary>
        /// <param name="loadPreDefinedFunctions">This will load "abs", "cos", "cosh", "arccos", "sin", "sinh", "arcsin", "tan", "tanh", "arctan", "sqrt", "rem", "round"</param>
        /// <param name="loadPreDefinedOperators">This will load "%", "*", ":", "/", "+", "-", ">", "&lt;", "="</param>
        /// <param name="loadPreDefinedVariables">This will load "pi", "pi2", "pi05", "pi025", "pi0125", "pitograd", "piofgrad", "e", "phi", "major", "minor"</param>
        public MathParser(bool loadPreDefinedFunctions = true, bool loadPreDefinedOperators = true, bool loadPreDefinedVariables = true)
        {
            if (loadPreDefinedOperators)
            {
                // by default, we will load basic arithmetic operators.
                // please note, its possible to do it either inside the constructor,
                // or outside the class. the lowest value will be executed first!
                OperatorList.Add("%"); // modulo
                OperatorList.Add("^"); // to the power of
                OperatorList.Add(":"); // division 1
                OperatorList.Add("/"); // division 2
                OperatorList.Add("*"); // multiplication
                OperatorList.Add("-"); // subtraction
                OperatorList.Add("+"); // addition

                OperatorList.Add(">"); // greater than
                OperatorList.Add("<"); // less than
                OperatorList.Add("="); // are equal


                // when an operator is executed, the parser needs to know how.
                // this is how you can add your own operators. note, the order
                // in this list does not matter.
                OperatorAction.Add("%", (numberA, numberB) => numberA%numberB);
                OperatorAction.Add("^", (numberA, numberB) => (decimal) Math.Pow((double) numberA, (double) numberB));
                OperatorAction.Add(":", (numberA, numberB) => numberA/numberB);
                OperatorAction.Add("/", (numberA, numberB) => numberA/numberB);
                OperatorAction.Add("*", (numberA, numberB) => numberA*numberB);
                OperatorAction.Add("+", (numberA, numberB) => numberA + numberB);
                OperatorAction.Add("-", (numberA, numberB) => numberA - numberB);

                OperatorAction.Add(">", (numberA, numberB) => numberA > numberB ? 1 : 0);
                OperatorAction.Add("<", (numberA, numberB) => numberA < numberB ? 1 : 0);
                OperatorAction.Add("=", (numberA, numberB) => numberA == numberB ? 1 : 0);
            }


            if (loadPreDefinedFunctions)
            {
                // these are the basic functions you might be able to use.
                // as with operators, localFunctions might be adjusted, i.e.
                // you can add or remove a function.
                // please open the "MathosTest" project, and find MathParser.cs
                // in "CustomFunction" you will see three ways of adding 
                // a new function to this variable!
                // EACH FUNCTION MAY ONLY TAKE ONE PARAMETER, AND RETURN ONE
                // VALUE. THESE VALUES SHOULD BE IN "DECIMAL FORMAT"!
                LocalFunctions.Add("abs", x => (decimal) Math.Abs((double) x[0]));

                LocalFunctions.Add("cos", x => (decimal) Math.Cos((double) x[0]));
                LocalFunctions.Add("cosh", x => (decimal) Math.Cosh((double) x[0]));
                LocalFunctions.Add("arccos", x => (decimal) Math.Acos((double) x[0]));

                LocalFunctions.Add("sin", x => (decimal) Math.Sin((double) x[0]));
                LocalFunctions.Add("sinh", x => (decimal) Math.Sinh((double) x[0]));
                LocalFunctions.Add("arcsin", x => (decimal) Math.Asin((double) x[0]));

                LocalFunctions.Add("tan", x => (decimal) Math.Tan((double) x[0]));
                LocalFunctions.Add("tanh", x => (decimal) Math.Tanh((double) x[0]));
                LocalFunctions.Add("arctan", x => (decimal) Math.Atan((double) x[0]));
                //LocalFunctions.Add("arctan2", x => (decimal)Math.Atan2((double)x[0], (double)x[1]));

                LocalFunctions.Add("sqrt", x => (decimal) Math.Sqrt((double) x[0]));
                LocalFunctions.Add("rem", x => (decimal) Math.IEEERemainder((double) x[0], (double) x[1]));
                LocalFunctions.Add("root", x => (decimal) Math.Pow((double) x[0], 1.0/(double) x[1]));

                LocalFunctions.Add("pow", x => (decimal) Math.Pow((double) x[0], (double) x[1]));

                LocalFunctions.Add("exp", x => (decimal) Math.Exp((double) x[0]));
                //LocalFunctions.Add("log", x => (decimal)Math.Log((double)x[0]));
                //LocalFunctions.Add("log10", x => (decimal)Math.Log10((double)x[0]));

                LocalFunctions.Add("log", delegate(decimal[] input)
                {
                    // input[0] is the number
                    // input[1] is the base

                    switch (input.Length)
                    {
                        case 1:
                            return (decimal) Math.Log((double) input[0]);
                        case 2:
                            return (decimal) Math.Log((double) input[0], (double) input[1]);
                        default:
                            return 0; // false
                    }
                });

                LocalFunctions.Add("round", x => (decimal) Math.Round((double) x[0]));
                LocalFunctions.Add("truncate",
                    x => (decimal) (x[0] < 0.0m ? -Math.Floor(-(double) x[0]) : Math.Floor((double) x[0])));
                LocalFunctions.Add("floor", x => (decimal) Math.Floor((double) x[0]));
                LocalFunctions.Add("ceiling", x => (decimal) Math.Ceiling((double) x[0]));
                LocalFunctions.Add("sign", x => (decimal) Math.Sign((double) x[0]));
            }

            if (loadPreDefinedVariables)
            {
                // local variables such as pi can also be added into the parser.
                LocalVariables.Add("pi", (decimal) 3.14159265358979323846264338327950288); // the simplest variable!
                LocalVariables.Add("pi2", (decimal) 6.28318530717958647692528676655900576);
                LocalVariables.Add("pi05", (decimal) 1.57079632679489661923132169163975144);
                LocalVariables.Add("pi025", (decimal) 0.78539816339744830961566084581987572);
                LocalVariables.Add("pi0125", (decimal) 0.39269908169872415480783042290993786);
                LocalVariables.Add("pitograd", (decimal) 57.2957795130823208767981548141051704);
                LocalVariables.Add("piofgrad", (decimal) 0.01745329251994329576923690768488612);

                LocalVariables.Add("e", (decimal) 2.71828182845904523536028747135266249);
                LocalVariables.Add("phi", (decimal) 1.61803398874989484820458683436563811);
                LocalVariables.Add("major", (decimal) 0.61803398874989484820458683436563811);
                LocalVariables.Add("minor", (decimal) 0.38196601125010515179541316563436189);
            }
        }

        #region Properties

        /// <summary>
        /// All operators should be inside this property.
        /// The first operator is executed first, et cetera.
        /// An operator may only be ONE character.
        /// </summary>
        public List<string> OperatorList { get; set; } = new List<string>();

        /// <summary>
        /// When adding a variable in the OperatorList property, you need to assign how that operator should work.
        /// </summary>
        public Dictionary<string, Func<decimal, decimal, decimal>> OperatorAction { get; set; } =
            new Dictionary<string, Func<decimal, decimal, decimal>>();

        /// <summary>
        /// All functions that you want to define should be inside this property.
        /// </summary>
        public Dictionary<string, Func<decimal[], decimal>> LocalFunctions { get; set; } =
            new Dictionary<string, Func<decimal[], decimal>>();

        /// <summary>
        /// All variables that you want to define should be inside this property.
        /// </summary>
        public Dictionary<string, decimal> LocalVariables { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// When converting the result from the Parse method or ProgrammaticallyParse method ToString(),
        /// please use this cultur info.
        /// </summary>
        public CultureInfo CultureInfo { get; } = CultureInfo.InvariantCulture;

        #endregion

        #region Public Methods

        /// <summary>
        /// Enter the math expression in form of a string.
        /// </summary>
        /// <param name="mathExpression"></param>
        /// <returns></returns>
        public decimal Parse(string mathExpression)
        {
            return MathParserLogic(Scanner(mathExpression));
        }

        /// <summary>
        /// Enter the math expression in form of a list of tokens.
        /// </summary>
        /// <param name="mathExpression"></param>
        /// <returns></returns>
        public decimal Parse(ReadOnlyCollection<string> mathExpression)
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
        /// <param name="mathExpression"></param>
        /// <param name="correctExpression"></param>
        /// <param name="identifyComments"></param>
        /// <returns></returns>
        public decimal ProgrammaticallyParse(string mathExpression, bool correctExpression = true, bool identifyComments = true)
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
            decimal varValue;

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
                    LocalVariables[varName] = varValue;
                else
                    LocalVariables.Add(varName, varValue);

                return varValue;
            }

            if (!mathExpression.Contains(":="))
                return Parse(mathExpression);

            //mathExpression = mathExpression.Replace(" ", ""); // remove white space
            varName = mathExpression.Substring(0, mathExpression.IndexOf(":=", StringComparison.Ordinal));
            mathExpression = mathExpression.Replace(varName + ":=", "");

            varValue = Parse(mathExpression);
            varName = varName.Replace(" ", "");

            if (LocalVariables.ContainsKey(varName))
                LocalVariables[varName] = varValue;
            else
                LocalVariables.Add(varName, varValue);

            return varValue;
        }

        /// <summary>
        /// This will convert a string expression into a list of tokens that can be later executed by Parse or ProgrammaticallyParse methods.
        /// </summary>
        /// <param name="mathExpression"></param>
        /// <returns>A ReadOnlyCollection</returns>
        public ReadOnlyCollection<string> GetTokens(string mathExpression)
        {
            return Scanner(mathExpression).AsReadOnly();
        }

        #endregion

        #region Core

        /// <summary>
        /// This will correct sqrt() and arctan() written in different ways only.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string Correction(string input)
        {
            // Word corrections

            input = System.Text.RegularExpressions.Regex.Replace(input, "\\b(sqr|sqrt)\\b", "sqrt",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            input = System.Text.RegularExpressions.Regex.Replace(input, "\\b(atan2|arctan2)\\b", "arctan2",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //... and more

            return input;
        }
        
        /// <summary>
        /// Scanning the <paramref name="expr"/> and convert it into tokens.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private List<string> Scanner(string expr)
        {
            var tokens = new List<string>();
            var vector = "";

            expr = expr.Replace("+-", "-");
            expr = expr.Replace("-+", "-");
            expr = expr.Replace("--", "+");

            for (var i = 0; i < expr.Length; i++)
            {
                var ch = expr[i];
                
                if (char.IsWhiteSpace(ch))
                {
                    // should be used to remove whitespace.
                }
                else if (char.IsLetter(ch))
                {
                    if (i != 0 && (char.IsDigit(expr[i - 1]) || char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
                    {
                        tokens.Add("*");
                    }

                    vector += ch;

                    // here is it is possible to choose whether you want variables that only contain letters with or without digits.
                    while ((i + 1) < expr.Length && char.IsLetterOrDigit(expr[i + 1]))
                    {
                        i++;
                        vector += expr[i];
                    }

                    tokens.Add(vector);
                    vector = "";
                }
                else if (char.IsDigit(ch))
                {
                    vector += ch;

                    while ((i + 1) < expr.Length && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
                    {
                        i++;
                        vector += expr[i];
                    }

                    tokens.Add(vector);
                    vector = "";
                }
                else if ((i + 1) < expr.Length && (ch == '-' || ch == '+') && char.IsDigit(expr[i + 1]) &&
                         (i == 0 || OperatorList.IndexOf(expr[i - 1].ToString(CultureInfo.InvariantCulture)) != -1 ||
                          ((i - 1) > 0 && expr[i - 1] == '(')))
                {
                    // if the above is true, then, the token for that negative number will be "-1", not "-","1".
                    // to sum up, the above will be true if the minus sign is in front of the number, but
                    // at the beginning, for example, -1+2, or, when it is inside the brakets (-1).
                    // NOTE: this works for + sign as well!
                    vector += ch;

                    while ((i + 1) < expr.Length && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
                    {
                        i++;
                        vector += expr[i];
                    }

                    tokens.Add(vector);
                    vector = "";
                }
                else if (ch == '(')
                {
                    if (i != 0 && (char.IsDigit(expr[i - 1]) || char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
                    {
                        // if we remove this line(below), we would be able to have numbers in function names. however, then we can't parse 3(2+2).
                        tokens.Add("*");
                        tokens.Add("(");
                    }
                    else
                        tokens.Add("(");
                }
                else
                    tokens.Add(ch.ToString());
            }

            return tokens;
        }

        private decimal MathParserLogic(List<string> tokens)
        {
            // CALCULATING THE EXPRESSIONS INSIDE THE BRACKETS
            // IF NEEDED, EXECUTE A FUNCTION

            // Variables replacement
            for (var i = 0; i < tokens.Count; i++)
            {
                if (LocalVariables.Keys.Contains(tokens[i]))
                    tokens[i] = LocalVariables[tokens[i]].ToString(CultureInfo);
            }

            while (tokens.IndexOf("(") != -1)
            {
                // getting data between "(" and ")"
                var open = tokens.LastIndexOf("(");
                var close = tokens.IndexOf(")", open); // in case open is -1, i.e. no "(" // , open == 0 ? 0 : open - 1

                if (open >= close)
                {
                    throw new ArithmeticException("No closing bracket/parenthesis! tkn: " +
                                                  open.ToString(CultureInfo.InvariantCulture));
                }

                var roughExpr = new List<string>();

                for (var i = open + 1; i < close; i++)
                    roughExpr.Add(tokens[i]);

                decimal tmpResult;

                var functioName = tokens[open == 0 ? 0 : open - 1];
                var args = new decimal[0];

                if (LocalFunctions.Keys.Contains(functioName))
                {
                    if (roughExpr.Contains(","))
                    {
                        // converting all arguments into a decimal array
                        for (var i = 0; i < roughExpr.Count; i++)
                        {
                            var defaultExpr = new List<string>();
                            var firstCommaOrEndOfExpression = (roughExpr.IndexOf(",", i) != -1)
                                ? roughExpr.IndexOf(",", i)
                                : roughExpr.Count;

                            while (i < firstCommaOrEndOfExpression)
                            {
                                defaultExpr.Add(roughExpr[i]);
                                i++;
                            }

                            // changing the size of the array of arguments
                            Array.Resize(ref args, args.Length + 1);

                            args[args.Length - 1] = (defaultExpr.Count == 0)
                                ? 0
                                : BasicArithmeticalExpression(defaultExpr);
                        }

                        // finnaly, passing the arguments to the given function
                        tmpResult = decimal.Parse(LocalFunctions[functioName](args).ToString(CultureInfo), CultureInfo);
                    }
                    else
                    {
                        // but if we only have one argument, then we pass it directly to the function
                        tmpResult =
                            decimal.Parse(
                                LocalFunctions[functioName](new[] {BasicArithmeticalExpression(roughExpr)})
                                    .ToString(CultureInfo), CultureInfo);
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

                if (LocalFunctions.Keys.Contains(functioName))
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

        private decimal BasicArithmeticalExpression(List<string> tokens)
        {
            // PERFORMING A BASIC ARITHMETICAL EXPRESSION CALCULATION
            // THIS METHOD CAN ONLY OPERATE WITH NUMBERS AND OPERATORS
            // AND WILL NOT UNDERSTAND ANYTHING BEYOND THAT.

            switch (tokens.Count)
            {
                case 1:
                    return decimal.Parse(tokens[0], CultureInfo);
                case 2:
                    var op = tokens[0];

                    if (op == "-" || op == "+")
                    {
                        return
                            decimal.Parse((op == "+" ? "" : (tokens[1].Substring(0, 1) == "-" ? "" : "-")) + tokens[1],
                                CultureInfo);
                    }

                    return OperatorAction[op](0, decimal.Parse(tokens[1], CultureInfo));
                case 0:
                    return 0;
            }

            foreach (var op in OperatorList)
            {
                while (tokens.IndexOf(op) != -1)
                {
                    var opPlace = tokens.IndexOf(op);

                    var numberA = Convert.ToDecimal(tokens[opPlace - 1], CultureInfo);
                    var numberB = Convert.ToDecimal(tokens[opPlace + 1], CultureInfo);

                    var result = OperatorAction[op](numberA, numberB);

                    tokens[opPlace - 1] = result.ToString(CultureInfo);
                    tokens.RemoveRange(opPlace, 2);
                }
            }

            return Convert.ToDecimal(tokens[0], CultureInfo);
        }

        #endregion
    }
}