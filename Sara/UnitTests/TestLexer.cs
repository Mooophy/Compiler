using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sara;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using E = System.Linq.Enumerable;

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

        [TestMethod]
        public void LexerWithCode1()
        {
            string code1 = @"..\..\..\TestSamples\code1.cpp";
            Assert.IsTrue(File.Exists(code1));

            Lexer lex = null;
            using(var sr = new StreamReader(code1))
            {
                lex = new Lexer(sr);
            }

            var expect = new List<string>
            {
                "{",
                "int", "i", ";",
                "}"
            };

            Assert.AreEqual(expect.Count, lex.Result.Count);
            foreach (var i in E.Range(0, expect.Count))
                Assert.AreEqual(expect[i], lex.Result[i].ToString());
        }
    }
}
