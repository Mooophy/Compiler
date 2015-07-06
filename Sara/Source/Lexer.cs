using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Sara
{
    public class WordTable : Dictionary<string, Word>
    {
        private readonly List<Word> _keyWords = new List<Word>
        {
            new Word("if", Tag.IF),
            new Word("else", Tag.ELSE),
            new Word("while", Tag.WHILE),
            new Word("do", Tag.DO),
            new Word("break", Tag.BREAK),

            Word.True,
            Word.False,
            Type.Int,
            Type.Char,
            Type.Bool,
            Type.Float
        };

        public List<Word> KeyWords
        {
            get { return _keyWords; }
        }

        public WordTable()
        {
            _keyWords.ForEach(w => { this[w.Lexeme] = w; });
        }
    }

    public class Lexer
    {
        private StreamReader _stream;
        public Dictionary<string, Word> Words { get; private set; }
        public long Line { get; private set; }
        public IList<Token> Result { get; private set; }

        public Lexer(StreamReader sr)
        {
            this._stream = sr;
            this.Words = new Sara.WordTable();
            this.Line = 1;
        }

        private IList<Token> Scan(StreamReader reader)
        {
            //delegates for later use
            Func<char> ch       = () => { return (char)reader.Peek(); };
            Func<bool> notEof   = () => { return reader.Peek() != -1; };
            Func<bool> isWS     = () => { return char.IsWhiteSpace(ch()); };
            Func<bool> isWord   = () => { return char.IsLetter(ch()) || '_' == ch(); };
            Action     move     = () => { reader.Read(); };
            Func<char, bool> matchNext = (char arg) => { move(); return arg == ch(); };
            var ret = new List<Token>();

            while(notEof())
            {
                //for whitespace
                for (; isWS(); move()) ;

                //for operators like && !=, etc
                switch(ch())
                {
                    case '&': 
                        ret.Add(matchNext('&') ? Word.and : new Token('&')); break;
                    case '|': 
                        ret.Add(matchNext('|') ? Word.or  : new Token('|')); break;
                    case '=':
                        ret.Add(matchNext('=') ? Word.eq  : new Token('=')); break;
                    case '!':
                        ret.Add(matchNext('=') ? Word.ne  : new Token('!')); break;
                    case '<':
                        ret.Add(matchNext('=') ? Word.le  : new Token('<')); break;
                    case '>':
                        ret.Add(matchNext('=') ? Word.ge  : new Token('>')); break;
                }

                //for identifiers and reserved words
                if(isWord())
                {
                    var sb = new StringBuilder();
                    sb.Append(ch());
                    move();
                    for (; isWord() || char.IsDigit(ch()); move()) sb.Append(ch());
                    var w = sb.ToString();

                    if (this.Words.ContainsKey(w)) this.Result.Add(Words[w]);
                }
            }
            return ret;
        }

    }
}
