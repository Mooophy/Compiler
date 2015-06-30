using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragon;

namespace UnitTests
{
    [TestClass]
    public class TestBooleanExpr
    {
        [TestMethod]
        public void TestConstant()
        {
            Assert.AreEqual("true", Constant.True.ToString());
            Assert.AreEqual("false", Constant.False.ToString());
            Constant.True.Jumping(42, 99);
            Constant.False.Jumping(100, 8);
            //output :
            //goto L42
            //goto L8

            var c1 = new Constant(42);
            Assert.AreEqual("42", c1.ToString());
            var c2 = new Constant(new Real(3.14f), Dragon.Type.Int);
            Assert.AreEqual("3.14", c2.ToString());
        }

        [TestMethod]
        public void TestLogical()
        {
            var logical = new Logical(Word.and, Constant.True, Constant.False);
            Assert.AreEqual("true && false", logical.ToString());
            logical.Gen();

            //output:	
            //iffalse true && false goto L1
            //        t1 = true
            //        goto L2
            //L1:	  t1 = false
            //L2:
        }

        [TestMethod]
        public void TestOr()
        {
            var or = new Or(Constant.False, Constant.False);
            Assert.AreEqual("false || false", or.ToString());
            or.Jumping(42, 99);

            //output:
            //  	  goto L99
        }
    }
}
