using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sara;

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
            var foo = new Id(new Word("foo", Tag.ID), Sara.Type.Int, 0x20);
            var bar = new Constant(55);
            var set = new Set(foo, bar);

            set.Gen(10, 20);
            //output:
            //      	foo = 55
        }

        [TestMethod]
        public void TestSetElem()
        {
            var acc = new Access(new Id(new Word("arr", Tag.ID), Sara.Type.Int,0x20), new Constant(20),Sara.Type.Int);
            var setElem = new SetElem(acc, new Constant(42));
            setElem.Gen(10, 20);
            //Output:	
            //          arr [ 20 ] = 42
        }

        [TestMethod]
        public void TestSeq()
        {
            var acc = new Access(new Id(new Word("arr", Tag.ID), Sara.Type.Int, 0x20), new Constant(20), Sara.Type.Int);
            var setElem = new SetElem(acc, new Constant(42));

            var seq = new Seq(setElem, setElem);
            seq.Gen(10, 20);
            //output:
            //	        arr [ 20 ] = 42
            //    L1:	arr [ 20 ] = 42
        }

        [TestMethod]
        public void TestBreak()
        {
            Stmt.Enclosing = new Stmt();
            var brk = new Break();
            brk.Gen(10, 20);
            //output:
            //          goto L0
        }
    }
}
