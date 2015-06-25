using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragon;

namespace UnitTests
{
    [TestClass]
    public class TestSymbol
    {
        [TestMethod]
        public void TestType()
        {
            Assert.AreEqual("int", Dragon.Type.Int.Lexeme);
            Assert.AreEqual(Tag.BASIC, Dragon.Type.Int.TagValue);
            Assert.AreEqual(4, Dragon.Type.Int.Width);

            Assert.AreEqual("float", Dragon.Type.Float.Lexeme);
            Assert.AreEqual(Tag.BASIC, Dragon.Type.Float.TagValue);
            Assert.AreEqual(8, Dragon.Type.Float.Width);

            Assert.AreEqual("char", Dragon.Type.Char.Lexeme);
            Assert.AreEqual(Tag.BASIC, Dragon.Type.Char.TagValue);
            Assert.AreEqual(1, Dragon.Type.Char.Width);

            Assert.AreEqual("bool", Dragon.Type.Bool.Lexeme);
            Assert.AreEqual(Tag.BASIC, Dragon.Type.Bool.TagValue);
            Assert.AreEqual(1, Dragon.Type.Bool.Width);

            Assert.IsTrue(Dragon.Type.Numeric(Dragon.Type.Int));
            Assert.IsTrue(Dragon.Type.Numeric(Dragon.Type.Float));
            Assert.IsTrue(Dragon.Type.Numeric(Dragon.Type.Char));
            Assert.IsFalse(Dragon.Type.Numeric(Dragon.Type.Bool));

            var list = new Dragon.Type("list", Tag.BASIC, 4);
            Assert.AreEqual("list", list.Lexeme);
            Assert.AreEqual(Tag.BASIC, list.TagValue);
            Assert.AreEqual(4, list.Width);

            Assert.AreEqual(Dragon.Type.Float, Dragon.Type.Max(Dragon.Type.Float, Dragon.Type.Int));
            Assert.AreEqual(Dragon.Type.Int, Dragon.Type.Max(Dragon.Type.Char, Dragon.Type.Int));
            Assert.IsNull(Dragon.Type.Max(Dragon.Type.Bool, Dragon.Type.Float));
        }

        [TestMethod]
        public void TestEnv()
        {
            var global = new Env(null);
            var main = new Env(global);

            var tok = new Word("some_var", Tag.ID);
            var id = new Id(tok, Dragon.Type.Int, 0xff);
            global.Add(tok, id);
            
            Assert.ReferenceEquals(id, main.Get(tok));
            Assert.IsNull(global.Get(new Token(Tag.ID)));
        }

        [TestMethod]
        public void TestArray()
        {
            var arr = new Dragon.Array(42, Dragon.Type.Float);

            Assert.AreEqual(Dragon.Type.Float, arr.Of);
            Assert.AreEqual(42, arr.Size);
            Assert.AreEqual("[42]float", arr.ToString());
        }
    }
}
