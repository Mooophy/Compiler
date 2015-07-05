using System.Collections.Generic;
using System.Linq;

namespace Sara
{
    public class Type : Word
    {
        public int Width;

        public Type(string typeName, char tag, int width)
            : base(typeName, tag)
        {
            this.Width = width;
        }

        public readonly static Type
            Int = new Type("int", Tag.BASIC, 4),
            Float = new Type("float", Tag.BASIC, 8),
            Char = new Type("char", Tag.BASIC, 1),
            Bool = new Type("bool", Tag.BASIC, 1);

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
}
