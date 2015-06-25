using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Dragon
{
    public class Lexer
    {
        private StreamReader _reader;
        private char _curr; // i.e. peek in dragon book
        public bool EofReached { get; private set; }
        public static int Line { get; private set; }
        private Dictionary<string, Word> _words;

        private void Reserve(Word w)
        {
            _words.Add(w.Lexeme, w); 
        }

        public Lexer(StreamReader r)
        {
            Lexer.Line = 1;
            this._reader = r;
            this._curr = ' ';
            this._words = new Dictionary<string, Word>();

            this.Reserve(new Word("if", Tag.IF));
            this.Reserve(new Word("else", Tag.ELSE));
            this.Reserve(new Word("while", Tag.WHILE));
            this.Reserve(new Word("do", Tag.DO));
            this.Reserve(new Word("break", Tag.BREAK));
            this.Reserve(Word.True);
            this.Reserve(Word.False);
            this.Reserve(Type.Int);
            this.Reserve(Type.Char);
            this.Reserve(Type.Bool);
            this.Reserve(Type.Float);
        }

        private bool ReadChar()
        {
            try 
            {
                if (-1 == this._reader.Peek())
                {
                    this.EofReached = true;
                    return false;
                }
                else
                {
                    this._curr = (char)this._reader.Read();
                    return true;
                }
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e.Message); 
            }

            return true;
        }

        private bool ReadChar(char ch)
        {
            if (this.EofReached) return false;
            this.ReadChar();
            if (_curr != ch) return false;
            this._curr = ' ';
            return true;
        }

        public Token scan()
        {
            //for white spaces
            for (; !this.EofReached; this.ReadChar())
            {
                if (_curr == ' ' || _curr == '\t')
                { 
                    continue; 
                }
                else if (_curr == '\r')
                {
                    this.ReadChar();    //eat \r    
                    ++Line; 
                }
                else break;
            }

            if (this.EofReached) return null;

            //for operators like && !=, etc
            switch (_curr)
            {
                case '&':
                    return this.ReadChar('&') ? Word.and : new Token('&');
                case '|':
                    return this.ReadChar('|') ? Word.or : new Token('|');
                case '=':
                    return this.ReadChar('=') ? Word.eq : new Token('=');
                case '!':
                    return this.ReadChar('=') ? Word.ne : new Token('!');
                case '<':
                    return this.ReadChar('=') ? Word.le : new Token('<');
                case '>':
                    return this.ReadChar('=') ? Word.ge : new Token('>');
            }

            //for numbers
            if (char.IsDigit(_curr))
            {
                int v = 0;
                do
                {
                    v = 10 * v + (int)(_curr - '0');
                    this.ReadChar();
                } while (char.IsDigit(_curr));
                if (_curr != '.') return new Num(v);

                float f = v, d = 10;
                for (; ; )
                {
                    this.ReadChar();
                    if (!char.IsDigit(_curr)) break;
                    f += (int)(_curr - 48) / d;
                    d *= 10;
                }
                return new Real(f);
            }

            //for identifiers
            if (char.IsLetter(_curr))
            {
                var b = new StringBuilder();
                do
                {
                    b.Append(_curr); 
                    this.ReadChar();
                } while (char.IsLetterOrDigit(_curr));
                var s = b.ToString();
                if (_words.ContainsKey(s)) return _words[s];
                else return _words[s] = new Word(s, Tag.ID);
            }

            //for the rest 
            var tok = new Token(_curr);
            if (!this.EofReached) _curr = ' ';
            return tok;
        }
    }
}
