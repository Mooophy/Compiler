using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sara
{
    public class Lexer
    {
        private StreamReader _stream;
        public Dictionary<string, Word> KeyWords { get; private set; }
        public long Line { get; private set; }

        private void InitKeyWords(Dictionary<string, Word> dic)
        {
            Action<Word> reserve = w => { dic.Add(w.Lexeme, w); };

            reserve(new Word("if", Tag.IF));
            reserve(new Word("else", Tag.ELSE));
            reserve(new Word("while", Tag.WHILE));
            reserve(new Word("do", Tag.DO));
            reserve(new Word("break", Tag.BREAK));
            reserve(Word.True);
            reserve(Word.False);

            //need class Type to before uncomment

            //reserve(Type.Int);
            //reserve(Type.Char);
            //reserve(Type.Bool);
            //reserve(Type.Float);
        }

        public Lexer(StreamReader sreader)
        {
            this._stream = sreader;
            this.KeyWords = new Dictionary<string, Word>();
            this.Line = 1;
        }
    }
}
