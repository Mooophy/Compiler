namespace Sara
{
    /// <summary>
    /// Constants for tokens
    /// </summary>
    public class Tag
    {
        public const char
            AND     = (char)256,
            BASIC   = (char)257,
            BREAK   = (char)258,
            DO      = (char)259,
            ELSE    = (char)260,
            EQ      = (char)261,
            FALSE   = (char)262,
            GE      = (char)263,
            ID      = (char)264,
            IF      = (char)265,
            INDEX   = (char)266,    //used in syntax tree
            LE      = (char)267,    //used in syntax tree
            MINUS   = (char)268,    
            NE      = (char)269,
            NUM     = (char)270,
            OR      = (char)271,
            REAL    = (char)272,
            TEMP    = (char)273,    //used in syntax tree
            TRUE    = (char)274,
            WHILE   = (char)275;
    }

    /// <summary>
    /// Representing a lexeme that indicates its purpose of parsing
    /// </summary>
    public class Token
    {
        public char TagValue { get; private set; }

        public Token(char tag)
        {
            this.TagValue = tag;
        }

        public override string ToString()
        {
            return this.TagValue.ToString();
        }
    }


    /// <summary>
    /// Integer
    /// </summary>
    public class Num : Token
    {
        public int Value { get; private set; }

        public Num(int val)
            : base(Tag.NUM)
        {
            this.Value = val;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }


    /// <summary>
    /// Manages lexemes for reserved words, identifiers and composite tokens like &&
    /// </summary>
    public class Word : Token
    {
        public string Lexeme { get; private set; }

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
            and     = new Word("&&", Tag.AND),
            or      = new Word("||", Tag.OR),
            eq      = new Word("==", Tag.EQ),
            ne      = new Word("!=", Tag.NE),
            le      = new Word("<=", Tag.LE),
            ge      = new Word(">=", Tag.GE),
            minus   = new Word("minus", Tag.MINUS),
            True    = new Word("true", Tag.TRUE),
            False   = new Word("false", Tag.FALSE),
            temp    = new Word("t", Tag.TEMP);
    }


    /// <summary>
    /// Floating point numbers
    /// </summary>
    public class Real : Token
    {
        public float Value { get; private set; }

        public Real(float val)
            : base(Tag.REAL)
        {
            this.Value = val;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
