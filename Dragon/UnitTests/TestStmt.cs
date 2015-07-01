using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragon;

namespace UnitTests
{
    [TestClass]
    public class TestStmt
    {
        [TestMethod]
        public void TestStmtClass()
        {
            var stmt = new Stmt();
            Assert.AreEqual(Stmt.Null, Stmt.Enclosing);
        }

        [TestMethod]
        public void TestIf()
        {
            var ifStmt = new If(Constant.False, new Stmt());
            ifStmt.Gen(42, 99);
            //output:
            //      goto L99
            //L1:
        }
    }
}
