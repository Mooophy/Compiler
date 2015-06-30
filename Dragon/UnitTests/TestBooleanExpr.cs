using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragon;

namespace UnitTests
{
    [TestClass]
    public class TestBooleanExpr
    {
        [TestMethod]
        public void TestConstantStaticMembers()
        {
            Assert.AreEqual("true", Constant.True.ToString());
            Assert.AreEqual("false", Constant.False.ToString());

            Constant.True.Jumping(42, 99);
            Constant.False.Jumping(100, 8);

            //output :
            //goto L42
            //goto L8
        }

        [TestMethod]
        public void TestConstantCtors()
        {
            var c1 = new Constant(42);
            Assert.AreEqual("42", c1.ToString());
            var c2 = new Constant(new Real(3.14f), Dragon.Type.Int);
            Assert.AreEqual("3.14", c2.ToString());
        }
    }
}
