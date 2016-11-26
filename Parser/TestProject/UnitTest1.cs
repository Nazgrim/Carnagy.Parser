using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var bd = new[] { "1", "2" };
            var cc = new[] { "2", "3" };
            var inter= bd.Intersect(cc);
        }
    }
}
