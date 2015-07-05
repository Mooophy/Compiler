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

        public Lexer(StreamReader sr)
        {
            this._stream = sr;
            this.Words = new Sara.WordTable();
            this.Line = 1;
        }
    }
}
