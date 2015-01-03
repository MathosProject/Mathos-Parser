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
    }
}
