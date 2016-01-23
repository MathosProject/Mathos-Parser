using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Mathos.Parser;

namespace MathosTest
{
    [TestClass]
    public class MathParserTest
    {

        [TestMethod]
        public void BasicArithmetics()
        {
            MathParser parser = new MathParser();

            decimal resultA = parser.Parse("5+2");
            Assert.IsTrue(resultA == 7);

            decimal resultB = parser.Parse("5+2*3");
            Assert.IsTrue(resultB == 11);
        }

        [TestMethod]
        public void AdvancedArithmetics()
        {
            MathParser parser = new MathParser();
            decimal resultA = parser.Parse("(2+3)(3+1)");
            Assert.IsTrue(resultA == 20);

        }

        [TestMethod]
        public void ConditionStatements()
        {
            MathParser parser = new MathParser();

            decimal resultA = parser.Parse("2+3=1+4");
            Assert.IsTrue(resultA == 1);

            decimal resultB = parser.Parse("3+2>(2-1)");
            Assert.IsTrue(resultB == 1);


        }

        [TestMethod]
        public void ProgrmaticallyAddVariables()
        {
            /* 
             * when parsing an expression that requires 
             * for instance a variable name declaration 
             * or change, use ProgramaticallyParse().
             */
            MathParser parser = new MathParser();

            // first way, using let varname = value
            decimal resultA = parser.ProgrammaticallyParse("let a = 2pi");
            Assert.IsTrue(parser.Parse("a") == (decimal)Math.PI * 2);

            // second way, using varname :=  value
            decimal resultC = parser.ProgrammaticallyParse("b := 20");
            Assert.IsTrue(parser.Parse("b") == 20);

            // third way, using let varname be value
            decimal resultD = parser.ProgrammaticallyParse("let c be 25");
            Assert.IsTrue(resultD == 25);
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

        [TestMethod]
        public void CustomFunctionsWithSeverelArguments()
        {
            /*
             * This example demonstrates the "anonymous method" way of adding
             * a function that can take more than one agument.
             */

            MathParser parser = new MathParser(loadPreDefinedFunctions: false);

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
        }


        [TestMethod]
        public void NegativeNumbers()
        {
            MathParser parser = new MathParser();

            decimal resultA = parser.Parse("-1+1");
            Assert.IsTrue(resultA == 0);

            decimal resultB = parser.Parse("--1");
            Assert.IsTrue(resultB == 1);

            decimal resultC = parser.Parse("(-2)");
            Assert.IsTrue(resultC == -2);

            decimal resultD = parser.Parse("(-2)(-2)");
            Assert.IsTrue(resultD == 4);
        }

        [TestMethod]
        public void Trigemoetry()
        {
            MathParser parser = new MathParser();
            Assert.IsTrue(parser.Parse("cos(32) + 3") == (decimal)Math.Cos(32) + 3);
        }

        [TestMethod]
        public void CustomizeOperators()
        {
            // declaring the parser
            MathParser parser = new MathParser();

            //customize the operator list
            parser.OperatorList = new List<string>() { "$", "%", "*", ":", "/", "+", "-", ">", "<", "=" };

            // adding "dollar operator" to the OperatorAction list
            parser.OperatorAction.Add("$", delegate(decimal numA, decimal numB)
            {
                return numA * 2 + numB * 3;
            });

            // parsing and comparing
            Assert.IsTrue(parser.Parse("3$2") == 3 * 2 + 3 * 2);
        }

        [TestMethod]
        public void DecimalOperations()
        {
            //MathParser parser = new MathParser(new CultureInfo("sv-SE")); // uses "," as a decimal separator
            //decimal resultA = parser.Parse("0,245 + 0,3");
            //decimal resultB = parser.Parse("-0,245 + 0,3");
            //Assert.IsTrue(resultB == decimal.Parse("0,055", new CultureInfo("sv-SE")));

            MathParser parserDefault = new MathParser(); // or new MathParser(new CultureInfo("en-US"))
            decimal resultC = parserDefault.Parse("0.245 + 0.3");
            decimal resultD = parserDefault.Parse("-0.245 + 0.3");
            Assert.IsTrue(resultD == decimal.Parse("0.055", parserDefault.CultureInfo));
        }

        [TestMethod]
        public void DifferentCultures()
        {
            MathParser parser = new MathParser();

        }
        void funcs(decimal[] arg)
        {
        }

        [TestMethod]
        public void ExecutionTime()
        {
            Stopwatch _timer = new Stopwatch();
            _timer.Start();
            MathParser parser = new MathParser();

            decimal result = parser.Parse("5+2");
            decimal result2 = parser.Parse("5+2*3*1+2((1-2)(2-3))");
            decimal result3 = parser.Parse("5+2*3*1+2((1-2)(2-3))*-1");
            _timer.Stop();

            Debug.WriteLine("time to parser with MathParser: " + _timer.ElapsedMilliseconds);

        }

        [TestMethod]
        public void BuiltInFunctions()
        {
            MathParser parser = new MathParser(loadPreDefinedFunctions: true);
            decimal result = parser.Parse("round(21.333333333333)");
            Assert.AreEqual(result, 21);

            decimal result2 = parser.Parse("pow(2,0)");
            Assert.AreEqual(result2, 1);
        }

        [TestMethod]
        public void ExceptionCatching()
        {
            MathParser parser = new MathParser();
            try
            {
                decimal result = parser.Parse("(-1");
                Assert.Fail(); // fail if the above expression hasn't thrown an exception
            }
            catch (ArithmeticException) { }

            decimal tr = parser.Parse("rem(20,1,,,,)");
        }

        [TestMethod]
        public void StrangeStuff()
        {

            MathParser parser = new MathParser(loadPreDefinedOperators: true);

            parser.OperatorList = new List<string>() { "times", "*", "dividedby", "/", "plus", "+", "minus", "-" };
            parser.OperatorAction.Add("times", (x, y) => x * y);
            parser.OperatorAction.Add("dividedby", (x, y) => x / y);
            parser.OperatorAction.Add("plus", (x, y) => x + y);
            parser.OperatorAction.Add("minus", (x, y) => x - y);

            Debug.WriteLine(parser.Parse("5 plus 3 dividedby 2 times 3").ToString(parser.CultureInfo));
        }

        [TestMethod]
        public void TestLongExpression()
        {
            MathParser parser = new MathParser();

            decimal t = parser.Parse("4^2-2*3^2 +4");
        }

        [TestMethod]
        public void TestSpeedRegExVsStringReplce()
        {
            MathParser parser = new MathParser();
            decimal t = parser.Parse("(3+2)(1+-2)(1--2)(1-+8)");
        }


        [TestMethod]
        public void CultureIndepndenceCommaBug()
        {
            // this fixes the bug in MathParserLogic by adding CULTURE_INFO conversion
            // to _tokens[i] = LocalVariables[_tokens[i]].ToString(CULTURE_INFO);
            MathParser parser = new MathParser();

            parser.LocalVariables.Add("x", 1.5M);

            decimal a = parser.Parse("x+3");

            Assert.AreEqual(a, 4.5M);
        }


        [TestMethod]
        public void SpeedTests()
        {
            var parser = new MathParser();
            parser.LocalVariables.Add("x",10);


            var list = parser.GetTokens("(3x+2)");

            //list.Add("(");
            //list.Add("3");
            //list.Add("*");
            //list.Add("x");
            //list.Add("+");
            //list.Add("2");
            //list.Add(")");

           // list = 

            double time = BenchmarkUtil.Benchmark(() => parser.Parse("(3x+2)"), 25000);
            double time2 = BenchmarkUtil.Benchmark(() => parser.Parse(list), 25000);

            Assert.IsTrue(time >= time2);


        }

        [TestMethod]
        public void DetailedSpeedTestWithOptimization()
        {
            var mp = new MathParser();

            mp.LocalVariables.Add("x", 5);

            var expr = "(3x+2)(2(2x+1))";

            int itr = 3000;
            double creationTimeAndTokenization = BenchmarkUtil.Benchmark( () => mp.GetTokens(expr) ,1);

            var tokens = mp.GetTokens(expr);

            double parsingTime = BenchmarkUtil.Benchmark(() => mp.Parse(tokens), itr);


            double totalTime = creationTimeAndTokenization + parsingTime;


            //var mp = new MathParser();

            //mp.LocalVariables.Add("x", 5);

            //var expr = "(3x+2)(2(2x+1))";

            //int itr = 50;

            double parsingTime2 = BenchmarkUtil.Benchmark(() => mp.Parse(expr), itr);


            double totalTime2 = parsingTime2;

            
        }

        [TestMethod]
        public void CultureTest()
        {
            var mp = new MathParser();

            var result = mp.Parse("3,21");
        }

        [TestMethod]
        public void DetailedSpeedTestWithOutOptimization()
        {
            var mp = new MathParser();

            mp.LocalVariables.Add("x", 5);

            var expr = "(3x+2)(2(2x+1))";

            int itr = 50;

            double parsingTime = BenchmarkUtil.Benchmark(() => mp.Parse(expr), itr);


            double totalTime = parsingTime;


        }


        [TestMethod]
        public void CommaPIBug()
        {
            var mp = new MathParser();

            var result = mp.Parse("pi");

            Assert.AreEqual(result, mp.LocalVariables["pi"]);
            
        }
        public class BenchmarkUtil
        {
            public static double Benchmark(Action action,
                int iterations)
            {
                double time = 0;
                int innerCount = 5;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                for (int i = 0; i < innerCount; i++)
                {
                    action.Invoke();
                }

                var watch = Stopwatch.StartNew();

                for (int i = 0; i < iterations; i++)
                {
                    action.Invoke();
                    time += Convert.ToDouble(watch.ElapsedMilliseconds) /
                        Convert.ToDouble(iterations);
                }

                return time;
            }
        }

    }
}
