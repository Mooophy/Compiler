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
            if (args.Count() != 1)
            {
                Console.WriteLine("Please specify code file");
                return;
            }

            var lex = new Lexer(new StreamReader(args[0]));
            var parse = new Parser(lex);
            parse.Program();
            Console.WriteLine();
        }
    }
}
