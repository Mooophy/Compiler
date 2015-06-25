using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dragon;

namespace UnitTests
{
    [TestClass]
    public class TestLexer
    {
        [TestMethod]
        public void TestTag()
        {
            Assert.AreEqual(256, Tag.AND);
            Assert.AreEqual(257, Tag.BASIC);
            Assert.AreEqual(258, Tag.BREAK);
            Assert.AreEqual(259, Tag.DO);
            Assert.AreEqual(260, Tag.ELSE);
            Assert.AreEqual(261, Tag.EQ);
            Assert.AreEqual(262, Tag.FALSE);
            Assert.AreEqual(263, Tag.GE);
            Assert.AreEqual(264, Tag.ID);
            Assert.AreEqual(265, Tag.IF);
            Assert.AreEqual(266, Tag.INDEX);
            Assert.AreEqual(267, Tag.LE);
            Assert.AreEqual(268, Tag.MINUS);
            Assert.AreEqual(269, Tag.NE);
            Assert.AreEqual(270, Tag.NUM);
            Assert.AreEqual(271, Tag.OR);
            Assert.AreEqual(272, Tag.REAL);
            Assert.AreEqual(273, Tag.TEMP);
            Assert.AreEqual(274, Tag.TRUE);
            Assert.AreEqual(275, Tag.WHILE);
        }

        [TestMethod]
        public void TestToken()
        {
            var tok_c = new Token('c');
            Assert.AreEqual('c', tok_c.TagValue);
            Assert.AreEqual("c", tok_c.ToString());

            var tok_9 = new Token('9');
            Assert.AreEqual('9', tok_9.TagValue);
            Assert.AreEqual("9", tok_9.ToString());
        }

        [TestMethod]
        public void TestNum()
        {
            var num_0 = new Num(0);
            Assert.AreEqual(Tag.NUM, num_0.TagValue);
            Assert.AreEqual(0, num_0.Value);

            var num_42 = new Num(42);
            Assert.AreEqual(Tag.NUM, num_42.TagValue);
            Assert.AreEqual(42, num_42.Value);

            var num_10000 = new Num(10000);
            Assert.AreEqual(Tag.NUM, num_10000.TagValue);
            Assert.AreEqual(10000, num_10000.Value);
        }

        [TestMethod]
        public void TestWord()
        {
            Assert.AreEqual(Tag.AND, Word.and.TagValue); 
            Assert.AreEqual("&&", Word.and.Lexeme);
            
            Assert.AreEqual(Tag.EQ, Word.eq.TagValue);
            Assert.AreEqual("==", Word.eq.Lexeme);
            
            Assert.AreEqual(Tag.FALSE, Word.False.TagValue); 
            Assert.AreEqual("false", Word.False.Lexeme);
            
            Assert.AreEqual(Tag.GE, Word.ge.TagValue);
            Assert.AreEqual(">=", Word.ge.Lexeme);
            
            Assert.AreEqual(Tag.LE, Word.le.TagValue);
            Assert.AreEqual("<=", Word.le.Lexeme);
            
            Assert.AreEqual(Tag.MINUS, Word.minus.TagValue);
            Assert.AreEqual("minus", Word.minus.Lexeme);
            
            Assert.AreEqual(Tag.NE, Word.ne.TagValue);
            Assert.AreEqual("!=", Word.ne.Lexeme);
            
            Assert.AreEqual(Tag.OR, Word.or.TagValue); 
            Assert.AreEqual("||", Word.or.Lexeme);
            
            Assert.AreEqual(Tag.TEMP, Word.temp.TagValue);
            Assert.AreEqual("t", Word.temp.Lexeme);
            
            Assert.AreEqual(Tag.TRUE, Word.True.TagValue);
            Assert.AreEqual("true", Word.True.Lexeme);

            var w = new Word("foo", Tag.ID);
            Assert.AreEqual("foo", w.Lexeme);
            Assert.AreEqual("foo", w.ToString());
            Assert.AreEqual(Tag.ID, w.TagValue);
        }

        [TestMethod]
        public void TestReal()
        {
            var r = new Real(42.00f);
            Assert.AreEqual(Tag.REAL, r.TagValue);
            Assert.AreEqual("42", r.ToString());
        }

        [TestMethod]
        public void TestLexer1()
        {
            string path = @"..\..\..\TestSamples\code1.cpp";
            Assert.IsTrue(File.Exists(path));
            var reader = new StreamReader(path);

            var lex = new Lexer(reader);
            var tokens = new List<Token>();
            while (!lex.EofReached)
                tokens.Add(lex.scan());
            reader.Close();

            Assert.AreEqual(2, Lexer.Line);
            Assert.AreEqual(3, tokens.Count);
            Assert.IsNull(tokens[2]);// the last one is null
        }

        [TestMethod]
        public void TestLexer2()
        {
            string path = @"..\..\..\TestSamples\code2.cpp";
            Assert.IsTrue(File.Exists(path));

            var tokens = new List<Token>();
            using( var reader = new StreamReader(path))
            {
                var lex = new Lexer(reader);
                while (!lex.EofReached)
                    tokens.Add(lex.scan());
                Assert.AreEqual(7, Lexer.Line);
            }
            
            Assert.AreEqual(22, tokens.Count);
            Assert.IsNull(tokens[21]);// the last one is null
            var expect = new List<string> 
            { 
                "{", "int", "i", "=", "42",";", "if", "(", "i", ">", "0", ")","{", "i", "=", "i", "+", "55", ";", "}", "}" 
            };
            for (int i = 0; i != tokens.Count - 1; ++i)
                Assert.AreEqual(expect[i], tokens[i].ToString());
        }
    }
}
