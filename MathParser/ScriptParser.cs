using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mathos.Parser
{
    public class ScriptParser
    {
        public Dictionary<string, double> LocalVariables { get { return mathParser.LocalVariables; } }
        public Dictionary<string, Func<double, double, double>> Operators { get { return mathParser.Operators; } }
        public Dictionary<string, Func<double[], double>> LocalFunctions { get { return mathParser.LocalFunctions; } }

        private MathParser mathParser;
        private BooleanParser booleanParser;
        public ScriptParser(bool loadPreDefinedFunctions = true, bool loadPreDefinedOperators = true, bool loadPreDefinedVariables = true, CultureInfo cultureInfo = null)
        {
            mathParser = new MathParser(loadPreDefinedFunctions, loadPreDefinedOperators, loadPreDefinedVariables, cultureInfo);
            booleanParser = new BooleanParser(mathParser);
        }

        /// <summary>
        /// Executes a string containg multiple lines as a script, and returns the last calculated number
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public double ExecuteMultiline(string script)
        {
            string[] lines = script.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            return ExecuteLines(lines);
        }

        /// <summary>
        /// Executes a list of strings as a script, line by line, and returns the last calculated number
        /// </summary>
        /// <param name="linesEnumerable"></param>
        /// <returns></returns>
        public double ExecuteLines(IEnumerable<string> linesEnumerable)
        {
            var lines = linesEnumerable.ToArray();

            double lastOutput = 0;
            Stack<IfChainState> chainStates = new Stack<IfChainState>();
            int lineNumber = 0;
            try
            {
                while (lineNumber < lines.Length)
                {
                    var line = lines[lineNumber].Trim().ToLowerInvariant();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        lineNumber++;
                        continue;
                    }
                    IfChainState currentState = IfChainState.Executing;
                    if (chainStates.Count > 0)
                    {
                        currentState = chainStates.Peek();
                    }
                    if (line.StartsWith("if"))
                    {
                        if (currentState == IfChainState.Executing)
                        {
                            string condition = line.Substring(line.IndexOf("if") + 2).Trim();
                            var result = booleanParser.ProgrammaticallyParse(condition);
                            bool executeIf = booleanParser.ToBoolean(result);
                            chainStates.Push(executeIf ? IfChainState.Executing : IfChainState.NotExecuted);
                        }
                        else
                        {
                            // The if statement this if statement is in is not executing, so we push executed on the stack to make sure the contents of this don't get executed, and any following if else/else statements don't get executed either
                            chainStates.Push(IfChainState.Executed);
                        }
                    }
                    else if (line.StartsWith("else if") || line.StartsWith("elif"))
                    {
                        var oldState = chainStates.Pop();
                        if (oldState == IfChainState.NotExecuted)
                        {
                            string condition = line.Substring(line.IndexOf("if") + 2).Trim();
                            var result = booleanParser.ProgrammaticallyParse(condition);
                            bool executeIf = booleanParser.ToBoolean(result);
                            chainStates.Push(executeIf ? IfChainState.Executing : IfChainState.NotExecuted);
                        }
                        else
                        {
                            chainStates.Push(IfChainState.Executed);
                        }
                    }
                    else if (line.StartsWith("else"))
                    {
                        var oldState = chainStates.Pop();
                        chainStates.Push(oldState == IfChainState.NotExecuted ? IfChainState.Executing : IfChainState.Executed);
                    }
                    else if (line == "end if" || line == "endif")
                    {
                        chainStates.Pop();
                    }
                    else
                    {
                        if (currentState == IfChainState.Executing)
                        {
                            lastOutput = mathParser.ProgrammaticallyParse(line);
                        }
                    }
                    lineNumber++;
                }
            }
            catch (Exception e)
            {
                throw new ScriptParserException(lineNumber + 1, e);
            }
            return lastOutput;
        }

        private enum IfChainState
        {
            // No if/else if in this chain has been executed yet, so the following else/else if can be executed
            NotExecuted = 0,
            // The current if/else if/else in this chain is executing, so we execute any code found, and any following else/else if cannot be executed
            Executing = 1,
            // The current if/else if/else is not executing, but a previous one did, so following else/else if cannot be executed
            Executed = 2
        }
    }
}