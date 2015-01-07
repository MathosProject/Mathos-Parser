using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class BugFixes
    {
        [TestMethod]
        public void UnaryOperators()
        {
            var mp = new Mathos.Parser.MathParser();
            var result = mp.Parse("-sin(5)");
        }

        [TestMethod]
        public void Doubles()
        {

            // the reason why decimal was chosen instead is because doubles are expressed as:
            // 3.11111111111111E+29
            // considering this, it should be possible to work with doubles instead.

            double a = 311111111111111111111112222222d;

            string b = a.ToString();

            double c = double.Parse(b);

        }
    }
}
