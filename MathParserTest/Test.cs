using System;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mathos.Parser.Test
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void BasicArithmetic()
        {
            var parser = new MathParser();
            
            Assert.AreEqual(7, parser.Parse("5 + 2"));
            Assert.AreEqual(11, parser.Parse("5 + 2 * 3"));
            Assert.AreEqual(17, parser.Parse("27 - 3 * 3 + 1 - 4 / 2"));
        }
        
        [TestMethod]
        public void AdvancedArithmetic()
        {
            var parser = new MathParser();
            
            Assert.AreEqual(30, parser.Parse("3(7+3)"));
            Assert.AreEqual(20, parser.Parse("(2+3)(3+1)"));
        }

        [TestMethod]
        public void DivideByZero()
        {
            var parser = new MathParser();
            
            Assert.AreEqual(double.PositiveInfinity, parser.Parse("5 / 0"));
            Assert.AreEqual(double.NegativeInfinity, parser.Parse("(-30) / 0"));
            Assert.AreEqual(double.NaN, parser.Parse("0 / 0"));

            Assert.AreEqual(double.PositiveInfinity, parser.Parse("5 : 0"));
            Assert.AreEqual(double.NegativeInfinity, parser.Parse("(-30) : 0"));
            Assert.AreEqual(double.NaN, parser.Parse("0 : 0"));
        }

        [TestMethod]
        public void ConditionalStatements()
        {
            var parser = new MathParser();

            Assert.AreEqual(1, parser.Parse("2 + 3 = 1 + 4"));
            Assert.AreEqual(1, parser.Parse("3 + 2 > 2 - 1"));
            Assert.AreEqual(1, parser.Parse("(2+3)(3+1) < 50 - 20"));

            Assert.AreEqual(0, parser.Parse("2 + 2 = 22"));
            Assert.AreEqual(0, parser.Parse("(2+3)(3+1) > 50 - 20"));
            Assert.AreEqual(0, parser.Parse("100 < 10"));

            Assert.AreEqual(1, parser.Parse("2.5 <= 3"));
            Assert.AreEqual(1, parser.Parse("(2+3)(3+1) <= 50 - 20"));

            Assert.AreEqual(0, parser.Parse("100 <= 10"));
            Assert.AreEqual(0, parser.Parse("(2+3)(3+1) >= 50 - 20"));
        }

        [TestMethod]
        public void ProgramicallyAddVariables()
        {
            var parser = new MathParser();
            
            parser.ProgrammaticallyParse("let a = 2pi");
            Assert.AreEqual(parser.LocalVariables["pi"] * 2, parser.Parse("a"), 0.00000000000001);
            
            parser.ProgrammaticallyParse("b := 20");
            Assert.AreEqual(20, parser.Parse("b"));
            
            parser.ProgrammaticallyParse("let c be 25 + 2(2+3)");
            Assert.AreEqual(35, parser.Parse("c"));

            parser.VariableDeclarator = "dim";
            parser.ProgrammaticallyParse("dim d = 5 ^3");
            Assert.AreEqual(125, parser.Parse("d"));
        }

        [TestMethod]
        public void CustomFunctions()
        {
            var parser = new MathParser();
            
            parser.LocalFunctions.Add("timesTwo", inputs => inputs[0] * 2);
            Assert.AreEqual(6, parser.Parse("timesTwo(3)"));
            Assert.AreEqual(42, parser.Parse("timesTwo((2+3)(3+1) + 1)"));

            parser.LocalFunctions.Add("square", inputs => inputs[0] * inputs[0]);
            Assert.AreEqual(16, parser.Parse("square(4)"));
            
            parser.LocalFunctions.Add("cube", inputs => inputs[0] * inputs[0] * inputs[0]);
            Assert.AreEqual(8, parser.Parse("cube(2)"));
        }
        
        [TestMethod]
        public void CustomFunctionsWithSeveralArguments()
        {
            var parser = new MathParser(false);
            
            parser.LocalFunctions.Add("log", delegate(double[] input)
            {
                switch (input.Length)
                {
                    case 1:
                        return Math.Log10(input[0]);
                    case 2:
                        return Math.Log(input[0], input[1]);
                    default:
                        return 0;
                }
            });

            Assert.AreEqual(0.301029996, parser.Parse("log(2)"), 0.000000001);
            Assert.AreEqual(0.630929754, parser.Parse("log(2,3)"), 0.000000001);
        }

        [TestMethod]
        [ExpectedException(typeof(MathParserException))]
        public void UndefinedVariableException()
        {
            var parser = new MathParser();

            try
            {
                parser.ProgrammaticallyParse("unknownvar * 5");
            }
            catch (Exception e)
            {
                // Tests to see if the message the exception gives is clear enough
                Assert.IsTrue(e.Message.ToLowerInvariant().Contains("variable") && e.Message.ToLowerInvariant().Contains("unknownvar"));
                throw e;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MathParserException))]
        public void UndefinedOperatorException()
        {
            var parser = new MathParser();

            try
            {
                parser.ProgrammaticallyParse("unknownoperator(5)");
            }
            catch (Exception e)
            {
                // Tests to see if the message the exception gives is clear enough
                Assert.IsTrue(e.Message.ToLowerInvariant().Contains("operator") && e.Message.ToLowerInvariant().Contains("unknownoperator"));
                throw e;
            }
        }

        [TestMethod]
        public void NegativeNumbers()
        {
            var parser = new MathParser();
            
            Assert.AreEqual(0, parser.Parse("-1+1"));
            Assert.AreEqual(1, parser.Parse("--1"));
            Assert.AreEqual(-2, parser.Parse("-2"));
            Assert.AreEqual(-2, parser.Parse("(-2)"));
            // Assert.AreEqual(2, parser.Parse("-(-2)")); TODO: Fix
            Assert.AreEqual(4, parser.Parse("(-2)(-2)"));
            Assert.AreEqual(-3, parser.Parse("-(3+2+1+6)/4"));

            parser.LocalVariables.Add("x", 50);

            Assert.AreEqual(-100, parser.Parse("-x - x"));
            Assert.AreEqual(-75, parser.Parse("-x * 1.5"));
        }

        [TestMethod]
        public void Trigonometry()
        {
            var parser = new MathParser();

            Assert.AreEqual(Math.Cos(32) + 3, parser.Parse("cos(32) + 3"));
        }

        [TestMethod]
        public void CustomizeOperators()
        {
            var parser = new MathParser();
            
            parser.Operators.Add("$", (a, b) => a * 2 + b * 3);
            
            Assert.AreEqual(3 * 2 + 3 * 2, parser.Parse("3 $ 2"));
        }

        [TestMethod]
        public void DoubleOperations()
        {
            var parserDefault = new MathParser();
            
            Assert.AreEqual(double.Parse("0.055", parserDefault.CultureInfo), parserDefault.Parse("-0.245 + 0.3"));
        }
        
        [TestMethod]
        public void ExecutionTime()
        {
            var timer = new Stopwatch();
            var parser = new MathParser();

            parser.Parse("5+2*3*1+2((1-2)(2-3))*-1"); // Warm-up

            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            timer.Start();
            
            parser.Parse("5+2");
            parser.Parse("5+2*3*1+2((1-2)(2-3))");
            parser.Parse("5+2*3*1+2((1-2)(2-3))*-1");

            timer.Stop();

            Debug.WriteLine("Parse Time: " + timer.Elapsed.TotalMilliseconds + "ms");
        }

        [TestMethod]
        public void BuiltInFunctions()
        {
            var parser = new MathParser();
            
            Assert.AreEqual(21, parser.Parse("round(21.333333333333)"));
            Assert.AreEqual(1, parser.Parse("pow(2,0)"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void ExceptionCatching()
        {
            var parser = new MathParser();

            parser.Parse("(-1");
            parser.Parse("rem(20,1,,,,)");
        }

        [TestMethod]
        public void StrangeStuff()
        {
            var parser = new MathParser();

            parser.Operators.Add("times", (x, y) => x * y);
            parser.Operators.Add("dividedby", (x, y) => x / y);
            parser.Operators.Add("plus", (x, y) => x + y);
            parser.Operators.Add("minus", (x, y) => x - y);

            Debug.WriteLine(parser.Parse("5 plus 3 dividedby 2 times 3").ToString(parser.CultureInfo));
        }

        [TestMethod]
        public void TestLongExpression()
        {
            var parser = new MathParser();

            Assert.AreEqual(2, parser.Parse("4^2-2*3^2+4"));
        }
        
        [TestMethod]
        public void SpeedTests()
        {
            var parser = new MathParser();

            parser.LocalVariables.Add("x",10);

            var list = parser.GetTokens("(3x+2)");
            var time = BenchmarkUtil.Benchmark(() => parser.Parse("(3x+2)"), 25000);
            var time2 = BenchmarkUtil.Benchmark(() => parser.Parse(list), 25000);

            Assert.IsTrue(time >= time2);
        }

        [TestMethod]
        public void DetailedSpeedTestWithOptimization()
        {
            var parser = new MathParser();

            parser.LocalVariables.Add("x", 5);

            var expr = "(3x+2)(2(2x+1))";

            const int itr = 3000;
            var creationTimeAndTokenization = BenchmarkUtil.Benchmark( () => parser.GetTokens(expr) ,1);
            var tokens = parser.GetTokens(expr);

            var parsingTime = BenchmarkUtil.Benchmark(() => parser.Parse(tokens), itr);
            var totalTime = creationTimeAndTokenization + parsingTime;

            Console.WriteLine("Parsing Time: " + parsingTime);
            Console.WriteLine("Total Time: " + totalTime);

            var parsingTime2 = BenchmarkUtil.Benchmark(() => parser.Parse(expr), itr);

            Console.WriteLine("Parsing Time 2: " + parsingTime2);
            Console.WriteLine("Total Time: " + parsingTime2);
        }

        [TestMethod]
        public void DetailedSpeedTestWithoutOptimization()
        {
            var parser = new MathParser();

            parser.LocalVariables.Add("x", 5);

            var expr = "(3x+2)(2(2x+1))";
            const int itr = 50;

            var parsingTime = BenchmarkUtil.Benchmark(() => parser.Parse(expr), itr);
            
            Console.WriteLine("Parsing Time: " + parsingTime);
            Console.WriteLine("Total Time: " + parsingTime);
        }
        
        [TestMethod]
        public void CommaPiBug()
        {
            var parser = new MathParser();
            var result = parser.Parse("pi");
            
            Assert.AreEqual(result, parser.LocalVariables["pi"], 0.00000000000001);
        }

        [TestMethod]
        public void NumberNotations()
        {
            var parser = new MathParser();
            
            Assert.AreEqual(0.0005, parser.Parse("5 * 10^-4"));
        }

        [TestMethod]
        public void NoLeadingZero()
        {
            var parser = new MathParser();

            Assert.AreEqual(0.5, parser.Parse(".5"));
            Assert.AreEqual(0.5, parser.Parse(".25 + .25"));
            Assert.AreEqual(2.0, parser.Parse("1.5 + .5"));
            Assert.AreEqual(-0.25, parser.Parse(".25 + (-.5)"));
            Assert.AreEqual(0.25, parser.Parse(".5(.5)"));
        }

        public class BenchmarkUtil
        {
            public static double Benchmark(Action action, int iterations)
            {
                double time = 0;
                const int innerCount = 5;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                for (var i = 0; i < innerCount; i++)
                    action.Invoke();

                var watch = Stopwatch.StartNew();

                for (var i = 0; i < iterations; i++)
                {
                    action.Invoke();

                    time += Convert.ToDouble(watch.ElapsedMilliseconds) / Convert.ToDouble(iterations);
                }

                return time;
            }
        }
    }
}
