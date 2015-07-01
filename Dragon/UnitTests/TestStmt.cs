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
    }
}
