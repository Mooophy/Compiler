namespace Dragon
{
    public class Constant : Expr
    {
        public Constant(Token tok, Dragon.Type type)
            : base(tok,type)
        { }

        public Constant(int i)
            : base(new Num(i), Dragon.Type.Int)
        { }

        public static readonly Constant
            True = new Constant(Word.True, Dragon.Type.Bool),
            False = new Constant(Word.False, Dragon.Type.Bool);

        /// <summary>
        /// Only for Constant.True and Constant.False
        /// </summary>
        /// <param name="lineForTrue"></param>
        /// <param name="lineForFalse"></param>
        public override void Jumping(int lineForTrue, int lineForFalse)
        {
            if (this == Constant.True && lineForTrue != 0)
                this.Emit("goto L" + lineForTrue);
            else if(this == Constant.False && lineForFalse != 0)
                this.Emit("goto L" + lineForFalse);
        }
    }


    /// <summary>
    /// A syntax node with operator op and operands lhs and rhs, providing common functionality for classes Or, And and Not.
    /// </summary>
    public class Logical : Expr
    {
        public Expr Lhs { get; private set; }
        public Expr Rhs { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="op">operator</param>
        /// <param name="lhs">boolean operand</param>
        /// <param name="rhs">boolean operand</param>
        public Logical(Token op, Expr lhs, Expr rhs)
            : base(op, Logical.Check(lhs.Type, rhs.Type))
        {
            this.Lhs = lhs;
            this.Rhs = rhs;
            if (null == this.Type) this.Error("type error");
        }
        /// <summary>
        /// Check if both are Dragon.Type.Bool
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns>Dragon.Type.Bool or null</returns>
        private static Dragon.Type Check(Dragon.Type lhs, Dragon.Type rhs)
        {
            if (lhs == Dragon.Type.Bool && rhs == Dragon.Type.Bool) 
                return Dragon.Type.Bool;
            return null;
        }
        /// <summary>
        /// Overriding to generate OpCode
        /// </summary>
        /// <returns>Temp object</returns>
        public override Expr Gen()
        {
            var falseExit = this.NewLable();
            var after = this.NewLable();
            var temp = new Temp(this.Type);
            this.Jumping(0, falseExit);
            this.Emit(temp.ToString() + " = true");
            this.Emit("goto L" + after);
            this.EmitLabel(falseExit);
            this.Emit(temp.ToString() + " = false");
            this.EmitLabel(after);
            return temp;
        }
        /// <summary>
        /// Overriding
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return this.Lhs.ToString() + " " + this.Op.ToString() + " " + this.Rhs.ToString();
        }
    }


    /// <summary>
    ///  B = Lhs || Rhs
    /// </summary>
    public class Or : Logical
    {
        /// <summary>
        /// Ctor
        /// Note : this differs the textbook version by omitting the Token parameter.
        /// </summary>
        /// <param name="lhs">Expr</param>
        /// <param name="rhs">Expr</param>
        public Or(Expr lhs, Expr rhs)
            : base(Word.or, lhs, rhs)
        { }

        /// <summary>
        /// Polymorphism happens
        /// </summary>
        /// <param name="trueExit"></param>
        /// <param name="falseExit"></param>
        public override void Jumping(int trueExit, int falseExit)
        {
            var label = trueExit != 0 ? trueExit : this.NewLable();
            this.Lhs.Jumping(label, 0);             //polymorphism expected
            this.Rhs.Jumping(trueExit, falseExit);  //polymorphism expected
            if (trueExit == 0)
                this.EmitLabel(label);
        }
    }


    /// <summary>
    /// B = Lhs && Rhs 
    /// </summary>
    public class And : Logical
    {
        /// <summary>
        /// Ctor
        /// Note : this differs the textbook version by omitting the Token parameter.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public And(Expr lhs, Expr rhs)
            : base(Word.and, lhs, rhs)
        { }
        /// <summary>
        /// Polymorphism happens
        /// </summary>
        /// <param name="trueExit"></param>
        /// <param name="falseExit"></param>
        public override void Jumping(int trueExit, int falseExit)
        {
            int label = falseExit != 0 ? falseExit : this.NewLable();
            this.Lhs.Jumping(0, label);
            this.Rhs.Jumping(trueExit, falseExit);
            if (falseExit == 0)
                this.EmitLabel(label);
        }
    }

    /// <summary>
    /// B = !Rhs
    /// </summary>
    public class Not : Logical
    {
        /// <summary>
        /// Ctor
        /// Note : this differs the textbook version by omitting the Token parameter.
        /// </summary>
        /// <param name="expr"></param>
        public Not(Expr expr)
            : base(new Token('!'), expr, expr)
        { }
        /// <summary>
        /// Overriding
        /// </summary>
        /// <param name="trueExit"></param>
        /// <param name="falseExit"></param>
        public override void Jumping(int trueExit, int falseExit)
        {
            this.Rhs.Jumping(falseExit, trueExit);
        }
        /// <summary>
        /// Overriding
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Op.ToString() + " " + this.Rhs.ToString();
        }
    }


    public class Rel : Logical
    {
        public Rel(Token tok, Expr lhs, Expr rhs)
            : base(tok, lhs, rhs)
        { }

        public Dragon.Type check(Dragon.Type lft, Dragon.Type rht)
        {
            if (lft is Array || rht is Array) return null;
            else if (lft == rht) return Dragon.Type.Bool;
            else return null;
        }

        public override void Jumping(int t, int f)
        {
            Expr lft = this.Lhs.Reduce();
            Expr rht = this.Rhs.Reduce();
            string test = lft.ToString() + " " + this.Op.ToString() + " " + rht.ToString();
            this.EmitJumps(test, t, f);
        }
    }


    public class Access : Op
    {
        public Id Array;
        public Expr Index;

        public Access(Id arr, Expr idx, Type type)
            : base(new Word("[]", Tag.INDEX), type)
        {
            this.Array = arr;
            this.Index = idx;
        }

        public override Expr Gen()
        {
            return new Access(this.Array, this.Index.Reduce(), this.Type);
        }

        public override void Jumping(int t, int f)
        {
            this.EmitJumps(this.Reduce().ToString(), t, f);
        }

        public override string ToString()
        {
            return this.Array.ToString() + " [ " + this.Index.ToString() + " ]";
        }
    }
}
