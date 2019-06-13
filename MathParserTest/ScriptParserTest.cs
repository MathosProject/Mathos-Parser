using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mathos.Parser.Test
{
    [TestClass]
    public class ScriptParserTest
    {
        private ScriptParser parser;

        [TestInitialize]
        public void Initialize()
        {
            parser = new ScriptParser();
        }

        [TestMethod]
        public void ListScriptOutput()
        {
            string[] lines = new string[]
            {
                "let y = 5 * pi",
                "let y = floor(y)",
                "y"
            };

            var result15 = parser.ExecuteLines(lines);

            Assert.AreEqual(15, result15);
        }

        [TestMethod]
        public void MultilineScriptOutput()
        {
            string script3 =
                "2 * 5" + Environment.NewLine +
                "8 + 2" + Environment.NewLine +
                "56/4" + Environment.NewLine +
                "9 / 3";

            string script5 = 
                "let x = 5" + Environment.NewLine + 
                "x";

            var result3 = parser.ExecuteMultiline(script3);
            var result5 = parser.ExecuteMultiline(script5);

            Assert.AreEqual(3, result3);
            Assert.AreEqual(5, result5);
        }

        [TestMethod]
        public void NoEmptyLineOutput()
        {
            string script =
                "let x = 5" + Environment.NewLine +
                "x" + Environment.NewLine +
                " \t" + Environment.NewLine; // <- space + tab

            var result = parser.ExecuteMultiline(script);

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void IfOutput()
        {
            string script1 =
                "let x = 5" + Environment.NewLine +
                "0" + Environment.NewLine +
                "if (x >= 4)" + Environment.NewLine +
                "1" + Environment.NewLine +
                "end if";

            string script0 =
                "0" + Environment.NewLine +
                "if (x < 4)" + Environment.NewLine +
                "1" + Environment.NewLine +
                "end if";

            var result1 = parser.ExecuteMultiline(script1);
            var result0 = parser.ExecuteMultiline(script0);

            Assert.AreEqual(1, result1);
            Assert.AreEqual(0, result0);
        }

        [TestMethod]
        public void ElseIfOutput()
        {
            string script2 =
                "let x = 5" + Environment.NewLine +
                "0" + Environment.NewLine +
                "if (x >= 6)" + Environment.NewLine +
                "1" + Environment.NewLine +
                "else if (x >= 3)" + Environment.NewLine +
                "2" + Environment.NewLine +
                "end if";

            string script1 =
                "0" + Environment.NewLine +
                "if (x >= 4)" + Environment.NewLine +
                "1" + Environment.NewLine +
                "else if (x >= 3)" + Environment.NewLine +
                "2" + Environment.NewLine +
                "else if (x >= 2)" + Environment.NewLine +
                "3" + Environment.NewLine +
                "end if";

            var result2 = parser.ExecuteMultiline(script2);
            var result1 = parser.ExecuteMultiline(script1);

            Assert.AreEqual(2, result2);
            Assert.AreEqual(1, result1);
        }

        [TestMethod]
        public void ElseOutput()
        {
            string script2 =
                "let x = 5" + Environment.NewLine +
                "0" + Environment.NewLine +
                "if (x >= 6)" + Environment.NewLine +
                "1" + Environment.NewLine +
                "else" + Environment.NewLine +
                "2" + Environment.NewLine +
                "end if";

            string script1 =
                "0" + Environment.NewLine +
                "if (x >= 4)" + Environment.NewLine +
                "1" + Environment.NewLine +
                "else if (x >= 3)" + Environment.NewLine +
                "2" + Environment.NewLine +
                "else" + Environment.NewLine +
                "3" + Environment.NewLine +
                "end if";

            var result2 = parser.ExecuteMultiline(script2);
            var result1 = parser.ExecuteMultiline(script1);

            Assert.AreEqual(2, result2);
            Assert.AreEqual(1, result1);
        }

        [TestMethod]
        public void NestedIfs()
        {
            string[] lines0 = new string[]
            {
                "let abc = 123",
                "0",
                "if abc < 100",
                    "1",
                    "if abc > 100",
                        "2",
                    "end if",
                "end if"
            };

            string[] lines4 = new string[]
            {
                "let abc = 123",
                "4",
                "if abc < 100",
                    "1",
                    "if abc > 100",
                        "2",
                    "else",
                        "3",
                    "end if",
                "end if"
            };

            string[] lines2 = new string[]
            {
                "let abc = 123",
                "5",
                "if abc < 100",
                    "1",
                "else",
                    "if abc > 100",
                        "2",
                    "end if",
                "end if"
            };

            var result0 = parser.ExecuteLines(lines0);
            var result4 = parser.ExecuteLines(lines4);
            var result2 = parser.ExecuteLines(lines2);

            Assert.AreEqual(0, result0);
            Assert.AreEqual(4, result4);
            Assert.AreEqual(2, result2);
        }

        [TestMethod]
        [ExpectedException(typeof(ScriptParserException))]
        public void ExceptionLinenumbers()
        {
            string[] lines = new string[]
            {
                "let abc = 123",
                "5",
                "if foo < 100", //line 3 contains undefined variable 
                    "1",
                "else",
                    "if abc > 100",
                        "2",
                    "end if",
                "end if"
            };
            try
            {
                parser.ExecuteLines(lines);
            }
            catch (ScriptParserException e)
            {
                Assert.IsTrue(e.Message.ToLowerInvariant().Contains("foo") && e.Message.ToLowerInvariant().Contains("line 3"));
                throw e;
            }
        }
    }
}
