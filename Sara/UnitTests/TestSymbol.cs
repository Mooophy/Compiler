using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sara;

namespace UnitTests
{
    [TestClass]
    public class TestSymbol
    {
        [TestMethod]
        public void TestType()
        {
            Assert.AreEqual("int", Sara.Type.Int.Lexeme);
            Assert.AreEqual(Tag.BASIC, Sara.Type.Int.TagValue);
            Assert.AreEqual(4, Sara.Type.Int.Width);

            Assert.AreEqual("float", Sara.Type.Float.Lexeme);
            Assert.AreEqual(Tag.BASIC, Sara.Type.Float.TagValue);
            Assert.AreEqual(8, Sara.Type.Float.Width);

            Assert.AreEqual("char", Sara.Type.Char.Lexeme);
            Assert.AreEqual(Tag.BASIC, Sara.Type.Char.TagValue);
            Assert.AreEqual(1, Sara.Type.Char.Width);

            Assert.AreEqual("bool", Sara.Type.Bool.Lexeme);
            Assert.AreEqual(Tag.BASIC, Sara.Type.Bool.TagValue);
            Assert.AreEqual(1, Sara.Type.Bool.Width);

            Assert.IsTrue(Sara.Type.Numeric(Sara.Type.Int));
            Assert.IsTrue(Sara.Type.Numeric(Sara.Type.Float));
            Assert.IsTrue(Sara.Type.Numeric(Sara.Type.Char));
            Assert.IsFalse(Sara.Type.Numeric(Sara.Type.Bool));

            var list = new Sara.Type("list", Tag.BASIC, 4);
            Assert.AreEqual("list", list.Lexeme);
            Assert.AreEqual(Tag.BASIC, list.TagValue);
            Assert.AreEqual(4, list.Width);

            Assert.AreEqual(Sara.Type.Float, Sara.Type.Max(Sara.Type.Float, Sara.Type.Int));
            Assert.AreEqual(Sara.Type.Int, Sara.Type.Max(Sara.Type.Char, Sara.Type.Int));
            Assert.IsNull(Sara.Type.Max(Sara.Type.Bool, Sara.Type.Float));
        }

        [TestMethod]
        public void TestEnv()
        {
            var global = new Env(null);
            var main = new Env(global);

            var tok = new Word("some_var", Tag.ID);
            var id = new Id(tok, Sara.Type.Int, 0xff);
            global.AddIdentifier(tok, id);
            
            Assert.ReferenceEquals(id, main.Get(tok));
            Assert.IsNull(global.Get(new Token(Tag.ID)));
        }

        [TestMethod]
        public void TestArray()
        {
            var arr = new Sara.Array(42, Sara.Type.Float);

            Assert.AreEqual(Sara.Type.Float, arr.Of);
            Assert.AreEqual(42, arr.Size);
            Assert.AreEqual("[42] float", arr.ToString());
            Assert.AreEqual(8 * 42, arr.Width);
        }
    }
}
