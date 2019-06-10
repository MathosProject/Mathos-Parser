using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mathos.Parser.Test
{
    [TestClass]
    public class BooleanParserTest
    {
        private BooleanParser parser;
        private MathParser mathParser;

        [TestInitialize]
        public void Initialize()
        {
            mathParser = new MathParser();
            parser = new BooleanParser(mathParser);
        }

        [TestMethod]
        public void TruthyConvert()
        {
            Assert.AreEqual(parser.ProgrammaticallyParse("0"), 0);
            Assert.AreEqual(parser.ProgrammaticallyParse("1"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("2346"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("-1"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("1-1"), 0);
            Assert.AreEqual(parser.ProgrammaticallyParse("0.5"), 1);
        }

        [TestMethod]
        public void BasicBoolean()
        {
            Assert.AreEqual(parser.ProgrammaticallyParse("0 || 0"), 0);
            Assert.AreEqual(parser.ProgrammaticallyParse("0 || 1"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("1 || 0"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("1 || 1"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("0 && 0"), 0);
            Assert.AreEqual(parser.ProgrammaticallyParse("0 && 1"), 0);
            Assert.AreEqual(parser.ProgrammaticallyParse("1 && 0"), 0);
            Assert.AreEqual(parser.ProgrammaticallyParse("1 && 1"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("1 && 0 || 1"), 1);
        }

        [TestMethod]
        public void ArithmicBoolean()
        {
            Assert.AreEqual(parser.ProgrammaticallyParse("0 <= 0.5"), 1);
            // 1 == 1
            Assert.AreEqual(parser.ProgrammaticallyParse("2 ^0 == (0.5 * 12) / 6 || 0"), 1, "Math operations in brackets not executed succesfully, possibly from converting doubles to truthy too early.");
            // 0 && 0
            Assert.AreEqual(parser.ProgrammaticallyParse("(2 ^ 0) - 1 && 3 - 5 + 2"), 0);

            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    Assert.AreEqual(parser.ProgrammaticallyParse(i + " % 2 == 0"), 1);
                }
                if (i % 2 == 1)
                {
                    Assert.AreEqual(parser.ProgrammaticallyParse(i + " % 2 == 0"), 0);
                }
            }
        }

        [TestMethod]
        public void BooleanOrderOfOperations()
        {
            Assert.AreEqual(parser.ProgrammaticallyParse("1 || 1 && 0"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("0 && 1 || 1"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("0 && (1 || 1)"), 0);
            Assert.AreEqual(parser.ProgrammaticallyParse("(1 || 1) && 0"), 0);
            Assert.AreEqual(parser.ProgrammaticallyParse("1 && 0 || 1"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("1 && 1 || 0"), 1);
        }

        [TestMethod]
        public void BooleanVariables()
        {
            // x = 12
            mathParser.ProgrammaticallyParse("let x = (5^2 - 1) / 2");

            Assert.AreEqual(parser.ProgrammaticallyParse("let y = 0"), 0);

            Assert.AreEqual(parser.ProgrammaticallyParse("x == 12"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("x % 2 == 0 && x % 3 == 0 && x % 4 == 0"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("x && y"), 0);
            Assert.AreEqual(parser.ProgrammaticallyParse("x || y"), 1);
        }

        [TestMethod]
        public void BooleanOperations()
        {
            // x = 12
            mathParser.ProgrammaticallyParse("let x = (5^2 - 1) / 2");

            Assert.AreEqual(parser.ProgrammaticallyParse("x == abs(-12)"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("x / 4 >= floor(pi)"), 1);
            Assert.AreEqual(parser.ProgrammaticallyParse("x / 3 < ceil(pi)"), 0);
        }
    }
}
