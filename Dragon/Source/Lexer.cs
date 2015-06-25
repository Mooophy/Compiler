using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dragon
{
    public class Tag
    {
        public const char
            AND     =   (char)256,
            BASIC   =   (char)257,
            BREAK   =   (char)258,
            DO      =   (char)259,
            ELSE    =   (char)260,
            EQ      =   (char)261,
            FALSE   =   (char)262,
            GE      =   (char)263,
            ID      =   (char)264,
            IF      =   (char)265,
            INDEX   =   (char)266,
            LE      =   (char)267,
            MINUS   =   (char)268,
            NE      =   (char)269,
            NUM     =   (char)270,
            OR      =   (char)271,
            REAL    =   (char)272,
            TEMP    =   (char)273,
            TRUE    =   (char)274,
            WHILE   =   (char)275;
    }


    public class Token
    {
        public readonly char TagValue;

        public Token(char t) 
        { 
            this.TagValue = t; 
        }

        public override string ToString() 
        {
            return this.TagValue.ToString();
        }
    }


    public class Num : Token
    {
        public readonly int Value;

        public Num(int v)
            : base(Tag.NUM) 
        { 
            this.Value = v; 
        }

        public override string ToString() 
        { 
            return this.Value.ToString(); 
        }
    }


    public class Word : Token
    {
        public readonly string Lexeme;

        public Word(string lexeme, char tag) 
            : base(tag) 
        { 
            this.Lexeme = lexeme; 
        }

        public override string ToString() 
        {
            return this.Lexeme; 
        }

        public readonly static Word
            and     =   new Word("&&", Tag.AND),
            or      =   new Word("||", Tag.OR),
            eq      =   new Word("==", Tag.EQ),
            ne      =   new Word("!=", Tag.NE),
            le      =   new Word("<=", Tag.LE),
            ge      =   new Word(">=", Tag.GE),
            minus   =   new Word("minus", Tag.MINUS),
            True    =   new Word("true", Tag.TRUE),
            False   =   new Word("false", Tag.FALSE),
            temp    =   new Word("t", Tag.TEMP);
    }


    public class Real : Token
    {
        public readonly float Value;

        public Real(float v) 
            : base(Tag.REAL) 
        {
            this.Value = v; 
        }

        public override string ToString() 
        { 
            return this.Value.ToString(); 
        }
    }


    public class Lexer
    {
        StreamReader _reader;
        char _curr; // i.e. peek in dragon book
        public bool EofReached { get; private set; }
        public static int Line { get; private set; }
        Dictionary<string, Word> _words;

        void reserve(Word w)
        {
            _words.Add(w.Lexeme, w); 
        }

        public Lexer(StreamReader r)
        {
            Lexer.Line = 1;
            this._reader = r;
            this._curr = ' ';
            this._words = new Dictionary<string, Word>();

            reserve(new Word("if",      Tag.IF));
            reserve(new Word("else",    Tag.ELSE));
            reserve(new Word("while",   Tag.WHILE));
            reserve(new Word("do",      Tag.DO));
            reserve(new Word("break",   Tag.BREAK));
            reserve(Word.True);
            reserve(Word.False);
            reserve(Type.Int);
            reserve(Type.Char);
            reserve(Type.Bool);
            reserve(Type.Float);
        }

        bool ReadChar()
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

        bool ReadChar(char ch)
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
