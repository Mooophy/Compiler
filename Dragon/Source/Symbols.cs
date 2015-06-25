using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon
{
    public class Env
    {
        private Dictionary<Token, Id> _table;
        protected Env _prev;

        public Env(Env prev)
        {
            _table = new Dictionary<Token, Id>();
            _prev = prev;
        }

        public void Add(Token tok, Id id)
        {
            _table.Add(tok, id);
        }

        public Id Get(Token tok)
        {
            for(var env = this; env != null; env = env._prev)
                if (_table.Keys.Contains(tok)) return _table[tok];
            return null;
        }
    }


    public class Type : Word
    {
        public int Width;

        public Type(string typeName, char tag, int width) 
            : base(typeName, tag) 
        { 
            this.Width = width; 
        }

        public readonly static Type
            Int     =   new Type("int",     Tag.BASIC, 4),
            Float   =   new Type("float",   Tag.BASIC, 8),
            Char    =   new Type("char",    Tag.BASIC, 1),
            Bool    =   new Type("bool",    Tag.BASIC, 1);

        public static bool Numeric(Type type) 
        {
            return type == Type.Char || type == Type.Int || type == Type.Float; 
        }

        public static Type Max(Type lhs, Type rhs)
        {
            if (!Type.Numeric(lhs) || !Type.Numeric(rhs))
                return null;
            else if (lhs == Type.Float || rhs == Type.Float)
                return Type.Float;
            else if (lhs == Type.Int || rhs == Type.Int)
                return Type.Int;
            else
                return Type.Char;
        }
    }


    public class Array : Type
    {
        public Type Of;
        public int Size;
        
        public Array(int sz, Type type)
            : base("[]",Tag.INDEX, sz * type.Width)
        {
            this.Size = sz;
            this.Of = type;
        }

        public override string ToString()
        {
            return "[" + this.Size + "]" + this.Of.ToString();
        }
    }
}
