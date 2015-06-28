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
        public void TestLexer1()
        {
            string path = @"..\..\..\TestSamples\code1.cpp";
            Assert.IsTrue(File.Exists(path));
            var reader = new StreamReader(path);

            var lex = new Lexer(reader);
            var tokens = new List<Token>();
            while (!lex.EofReached)
                tokens.Add(lex.Scan());
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
                    tokens.Add(lex.Scan());
                Assert.AreEqual(8, Lexer.Line);
            }
            
            Assert.AreEqual(35, tokens.Count);
            Assert.IsNull(tokens[34]);// the last one is null
            var expect = new List<string> 
            { 
                "{", 
                "int", "i", ";",
                "float", "[", "100", "]", "a", ";",
                "while", "(", "true", ")",
                "{",
                "do", "i", "=", "i", "+", "1", ";",
                "while","(","a","[","i", "]", "<", "42",")",";",
                "}",
                "}"
            };
            for (int i = 0; i != tokens.Count - 1; ++i)
                Assert.AreEqual(expect[i], tokens[i].ToString());
        }
    }
}
