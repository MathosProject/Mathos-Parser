using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Mathos.Parser
{
    public class BooleanParser
    {
        public CultureInfo CultureInfo { get { return mathParser.CultureInfo; } }
        private MathParser mathParser;

        public BooleanParser(MathParser parser)
        {
            this.mathParser = parser;
        }

        /// <summary>
        /// Converts the given double to a boolean based on truthy logic, so non-zero values return 1 and zero returns false
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ToBoolean(double value)
        {
            return !Equals(value, 0);
        }

        /// <summary>
        /// Converts the given boolean to a truthy double, zo true gives 1 and false gives 0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double ToDouble(bool value)
        {
            return value ? 1 : 0;
        }

        /// <summary>
        /// Converts the give double to a truthy value, so any non-zero value returns 1, and zeroes return zero
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double ToTruthy(double value)
        {
            return Equals(value, 0) ? 0 : 1;
        }

        private bool Equals(double a, double b)
        {
            return Math.Abs(a - b) < 0.00000001;
        }

        public double ProgrammaticallyParse(string condition)
        {
            return ToTruthy(ProgrammaticallyParseToNumber(condition));
        }

        private double ProgrammaticallyParseToNumber(string condition)
        {
            condition = condition.Replace("&&", "&");
            condition = condition.Replace("||", "|");
            condition = Regex.Replace(condition, @"\btrue\b", "1", RegexOptions.IgnoreCase);
            condition = Regex.Replace(condition, @"\bfalse\b", "0", RegexOptions.IgnoreCase);
            condition = condition.Replace("||", "|");
            condition = Regex.Replace(condition, @"\band\b", "&", RegexOptions.IgnoreCase);
            condition = Regex.Replace(condition, @"\bor\b", "|", RegexOptions.IgnoreCase);
            condition = condition.Trim();

            if (condition.Contains("("))
            {
                int open = condition.IndexOf("(");
                int close = open + 1;

                char[] myArray = condition.ToCharArray();
                Stack<char> myStack = new Stack<char>();
                for (int i = open; i < myArray.Length; i++)
                {
                    if (myArray[i] == '(')
                    {
                        myStack.Push(myArray[i]);
                    }
                    if (myArray[i] == ')')
                    {
                        myStack.Pop();
                    }
                    if (myStack.Count == 0)
                    {
                        close = i;
                        break;
                    }
                    if (i == myArray.Length - 1)
                    {
                        throw new Exception("Brackets don't match");
                    }
                }

                string innerCondition = condition.Substring(open + 1, (close) - (open + 1));
                // check to see if the brackets aren't part of a operator call, like abs()
                if (open == 0 || (!char.IsLetterOrDigit(condition[open - 1])))
                {
                    condition = condition.Replace("(" + innerCondition + ")", ProgrammaticallyParseToNumber(innerCondition) + "");
                }
            }
            string[] conditions = condition.Split('&', '|');
            foreach (string c in conditions)
            {
                condition = condition.Replace(c, mathParser.ProgrammaticallyParse(c).ToString(CultureInfo));
            }

            var tokens = Lexer(condition);
            return BasicBooleanExpression(tokens);
        }

        private List<string> Lexer(string expr)
        {
            var token = "";
            var tokens = new List<string>();
            for (var i = 0; i < expr.Length; i++)
            {
                var ch = expr[i];

                if (char.IsWhiteSpace(ch))
                {
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
                if (ch == '-')
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
                tokens.Add(ch.ToString());
            }

            return tokens;
        }

        /// <summary>
        /// Executes a basic boolean expression. Note that AND goes before OR in the order of operations, so true AND false OR true equals true
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private double BasicBooleanExpression(List<string> tokens)
        {
            // PERFORMING A BASIC BOOLEAN EXPRESSION CALCULATION
            // THIS METHOD CAN ONLY OPERATE WITH NUMBERS AND LOGIC OPERATORS
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
                    throw new Exception("Can't parse tokens as boolean expression: " + tokens[0] + " " + tokens[1]);
                case 0:
                    return 0;
            }

            int andIndex = tokens.IndexOf("&");
            while (andIndex > 0)
            {
                var left = double.Parse(tokens[andIndex - 1], CultureInfo);
                var right = double.Parse(tokens[andIndex + 1], CultureInfo);
                var result = ToBoolean(left) && ToBoolean(right);
                tokens[andIndex - 1] = result ? "1" : "0";
                tokens.RemoveAt(andIndex);
                tokens.RemoveAt(andIndex);
                andIndex = tokens.IndexOf("&");
            }

            int orIndex = tokens.IndexOf("|");
            while (orIndex > 0)
            {
                var left = double.Parse(tokens[orIndex - 1], CultureInfo);
                var right = double.Parse(tokens[orIndex + 1], CultureInfo);
                var result = ToBoolean(left) || ToBoolean(right);
                tokens[orIndex - 1] = result ? "1" : "0";
                tokens.RemoveAt(orIndex);
                tokens.RemoveAt(orIndex);
                orIndex = tokens.IndexOf("|");
            }

            return double.Parse(tokens[0], CultureInfo);
        }
    }
}
