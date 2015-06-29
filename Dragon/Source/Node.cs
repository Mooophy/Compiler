//
//  corresponds to Inter package in dragon book
//
using System;
using System.IO;

namespace Dragon
{
    public class Node
    {
        int _lexLine;
        static int _labels = 0;

        public Node() //private on book
        { 
            _lexLine = Lexer.Line; 
        }

        public void Error(string msg)
        {
            throw new Exception("near line " + _lexLine + ": " + msg);
        }

        public int NewLable()
        {
            return ++Node._labels;
        }

        public void EmitLabel(int i)
        {
            Console.Write("L" + i + ":");
        }

        public void Emit(string s)
        {
            Console.WriteLine("\t" + s);
        }
    }
}
