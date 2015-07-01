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

        [TestMethod]
        public void TestIfElse()
        {
            var ifElse = new IfElse(new Rel(new Token('>'), new Constant(42), new Constant(99)), new Stmt(), new Stmt());
            ifElse.Gen(10, 100);
            //output:
            //      iffalse 42 > 99 goto L2
            //L1:	goto L100
            //L2:
        }

        [TestMethod]
        public void TestWhile()
        {
            var while_ = new While();
            while_.Init(new Rel(new Token('>'), new Constant(42), new Constant(99)), new Stmt());
            while_.Gen(10, 88);
            //output:
            //      iffalse 42 > 99 goto L88
            //L1:	goto L 10
        }

        [TestMethod]
        public void TestDo()
        {
            var do_ = new Do();
            do_.Init(new Stmt(), new Rel(new Token('>'), new Constant(42), new Constant(99)));
            do_.Gen(10, 20);
            //output:
            //L1:	if 42 > 99 goto L10
        }

        [TestMethod]
        public void TestSet()
        {
            var foo = new Id(new Word("foo", Tag.ID), Dragon.Type.Int, 0x20);
            var bar = new Constant(55);
            var set = new Set(foo, bar);

            set.Gen(10, 20);
            //output:
            //      	foo = 55
        }
    }
}
