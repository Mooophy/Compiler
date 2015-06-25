using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dragon
{
    class Program
    {
        static void Main(string[] args)
        {
            var lex = new Lexer(new StreamReader(@"..\..\..\TestSamples\code2.cpp"));
            var parse = new Parser(lex);
            parse.Program();
            Console.WriteLine();
        }
    }
}
