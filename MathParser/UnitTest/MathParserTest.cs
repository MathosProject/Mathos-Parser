using System;
using System.Collections.Generic;
using System.Diagnostics;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using Assert = NUnit.Framework.Assert;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Mathos.Parser.Test
{
    [TestClass]
    public class MathParserTest
    {
        [TestMethod]
        public void BasicArithmetics()
        {
            var parser = new MathParser();
            
            Assert.AreEqual(7, parser.Parse("5+2"));
            Assert.AreEqual(11, parser.Parse("5+2*3"));
        }
        
        [TestMethod]
        public void AdvancedArithmetics()
        {
            var parser = new MathParser();
            
            Assert.AreEqual(20, parser.Parse("(2+3)(3+1)"));
        }

        [TestMethod]
        public void ConditionStatements()
        {
            var parser = new MathParser();

            Assert.AreEqual(1, parser.Parse("2 + 3 = 1 + 4"));
            Assert.AreEqual(1, parser.Parse("3 + 2 > (2 - 1)"));

            Assert.AreEqual(0, parser.Parse("2 + 2 = 22"));
            Assert.AreEqual(0, parser.Parse("10 > 100"));
        }

        [TestMethod]
        public void ProgramicallyAddVariables()
        {
            /* 
             * when parsing an expression that requires,
             * for instance, a variable name declaration
             * or change, use ProgramaticallyParse().
             */
            var parser = new MathParser();
            
            parser.ProgrammaticallyParse("let a = 2pi");
            Assert.AreEqual(parser.LocalVariables["pi"] * 2, parser.Parse("a"), 0.00000000000001);
            
            parser.ProgrammaticallyParse("b := 20");
            Assert.AreEqual(20, parser.Parse("b"));
            
            parser.ProgrammaticallyParse("let c be 25");
            Assert.AreEqual(25, parser.Parse("c"));
        }

        private double NumberTimesTwo(double[] x)
        {
            return x[0] * 2;
        }

        [TestMethod]
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
            var parser = new MathParser();
            
            parser.LocalFunctions.Add("numberTimesTwo", NumberTimesTwo);
            Assert.AreEqual(6, parser.Parse("numberTimesTwo(3)"));
            
            parser.LocalFunctions.Add("square", x => x[0] * x[0]);
            Assert.AreEqual(16, parser.Parse("square(4)"));
            
            parser.LocalFunctions.Add("cube", delegate(double[] x)
            {
                return x[0] * x[0] * x[0];
            });

            Assert.AreEqual(8, parser.Parse("cube(2)"));
        }
        
        [TestMethod]
        public void CustomFunctionsWithSeveralArguments()
        {
            /*
             * This example demonstrates the "anonymous method" way of adding
             * a function that can take more than one agument.
             */

            var parser = new MathParser(false);
            
            parser.LocalFunctions.Add("log", delegate(double[] input)
            {
                // input[0] is the number
                // input[1] is the base

                if (input.Length == 1)
                    return Math.Log(input[0]);
                if (input.Length == 2)
                    return Math.Log(input[0], input[1]);

                return 0;
            });

            Assert.AreEqual(0.693147181, parser.Parse("log(2)"), 0.000000001);
            Assert.AreEqual(0.63093, parser.Parse("log(2,3)"), 0.000001);
        }
        
        [TestMethod]
        public void NegativeNumbers()
        {
            var parser = new MathParser();
            
            Assert.AreEqual(0, parser.Parse("-1+1"));
            Assert.AreEqual(1, parser.Parse("--1"));
            Assert.AreEqual(-2, parser.Parse("-2"));
            Assert.AreEqual(-2, parser.Parse("(-2)"));
            // Assert.AreEqual(2, parser.Parse("-(-2)")); TODO: Fixme
            Assert.AreEqual(4, parser.Parse("(-2)(-2)"));
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
            var parser = new MathParser
            {
                OperatorList = new List<string> {"$", "%", "*", ":", "/", "+", "-", ">", "<", "="}
            };
            
            parser.OperatorAction.Add("$", (numA, numB) => numA*2 + numB*3);
            
            Assert.AreEqual(3 * 2 + 3 * 2, parser.Parse("3$2"));
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

            GC.Collect();

            timer.Start();
            
            parser.Parse("5+2");
            parser.Parse("5+2*3*1+2((1-2)(2-3))");
            parser.Parse("5+2*3*1+2((1-2)(2-3))*-1");

            timer.Stop();

            Debug.WriteLine("Parse Time: " + timer.ElapsedMilliseconds + "ms");
        }

        [TestMethod]
        public void BuiltInFunctions()
        {
            var parser = new MathParser();
            
            Assert.AreEqual(21, parser.Parse("round(21.333333333333)"));
            Assert.AreEqual(1, parser.Parse("pow(2,0)"));
        }

        [TestMethod]
#if !NUNIT
        [ExpectedException(typeof(ArithmeticException))]
#endif
        public void ExceptionCatching()
        {
            var parser = new MathParser();

#if NUNIT
            TestDelegate expectException = () =>
            {
                parser.Parse("(-1");
                parser.Parse("rem(20,1,,,,)");
            };

            Assert.Throws(typeof(ArithmeticException), expectException);
#else
            parser.Parse("(-1");
            parser.Parse("rem(20,1,,,,)");
#endif
        }

        [TestMethod]
        public void StrangeStuff()
        {
            var parser = new MathParser {OperatorList = new List<string>() {"times", "*", "dividedby", "/", "plus", "+", "minus", "-"}};

            parser.OperatorAction.Add("times", (x, y) => x * y);
            parser.OperatorAction.Add("dividedby", (x, y) => x / y);
            parser.OperatorAction.Add("plus", (x, y) => x + y);
            parser.OperatorAction.Add("minus", (x, y) => x - y);

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
