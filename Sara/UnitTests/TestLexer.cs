using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sara;

namespace UnitTests
{
    [TestClass]
    public class TestLexer
    {
        [TestMethod]
        public void WordTable()
        {
            var table = new WordTable();
            Assert.AreEqual(11, table.KeyWords.Count);

            Assert.AreEqual("if", table["if"].Lexeme);
            Assert.AreEqual("else", table["else"].Lexeme);
            Assert.AreEqual("while", table["while"].Lexeme);
            Assert.AreEqual("do", table["do"].Lexeme);
            Assert.AreEqual("break", table["break"].Lexeme);
            Assert.AreEqual("true", table["true"].Lexeme);
            Assert.AreEqual("false", table["false"].Lexeme);
            Assert.AreEqual("int", table["int"].Lexeme);
            Assert.AreEqual("char", table["char"].Lexeme);
            Assert.AreEqual("bool", table["bool"].Lexeme);
            Assert.AreEqual("float", table["float"].Lexeme);

        }
    }
}
