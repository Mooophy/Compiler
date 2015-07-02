using System;
using System.Linq;
using System.IO;

namespace Dragon
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Count() != 1)
            //{
            //    Console.WriteLine("Please specify code file");
            //    return;
            //}

            var lex = new Lexer(new StreamReader("code2.cpp"));
            var parse = new Parser(lex);
            parse.Program();
            Console.WriteLine();
        }
    }
}
