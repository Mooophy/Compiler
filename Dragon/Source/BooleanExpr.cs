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
            : base(op, null)
        {
            this.Lhs = lhs;
            this.Rhs = rhs;
            this.Type = this.Check(lhs.Type, rhs.Type);
            if (null == this.Type) 
                this.Error("type error");
        }
        /// <summary>
        /// Virtual
        /// Check if both are Dragon.Type.Bool
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns>Dragon.Type.Bool or null</returns>
        protected virtual Dragon.Type Check(Dragon.Type lhs, Dragon.Type rhs)
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


    /// <summary>
    /// Implementation for < <= == != >= >
    /// Note: coercions are not permitted 
    /// </summary>
    public class Rel : Logical
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="op"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public Rel(Token op, Expr lhs, Expr rhs)
            : base(op, lhs, rhs)
        { }
        /// <summary>
        /// Overriding
        /// Check the two operands have the same type and neither is Array
        /// </summary>
        /// <param name="lft"></param>
        /// <param name="rht"></param>
        /// <returns></returns>
        protected override Dragon.Type Check(Dragon.Type lft, Dragon.Type rht)
        {
            if (lft is Array || rht is Array)
                return null;
            else
                return lft == rht ? Dragon.Type.Bool : null;
        }
        /// <summary>
        /// Overriding
        /// </summary>
        /// <param name="trueExit"></param>
        /// <param name="falseExit"></param>
        public override void Jumping(int trueExit, int falseExit)
        {
            var test = this.Lhs.Reduce().ToString() + " " + this.Op.ToString() + " " + this.Rhs.Reduce().ToString();
            this.EmitJumps(test, trueExit, falseExit);
        }
    }


    public class Access : Op
    {
        public Id Array { get; private set; }
        public Expr Index { get; private set; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="idx"></param>
        /// <param name="type"></param>
        public Access(Id arr, Expr idx, Dragon.Type type)
            : base(new Word("[]", Tag.INDEX), type)
        {
            this.Array = arr;
            this.Index = idx;
        }
        /// <summary>
        /// Overriding
        /// </summary>
        /// <returns></returns>
        public override Expr Gen()
        {
            return new Access(this.Array, this.Index.Reduce(), this.Type);
        }
        /// <summary>
        /// Overriding
        /// </summary>
        /// <param name="trueExit"></param>
        /// <param name="falseExit"></param>
        public override void Jumping(int trueExit, int falseExit)
        {
            this.EmitJumps(this.Reduce().ToString(), trueExit, falseExit);
        }
        /// <summary>
        /// Overriding
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Array.ToString() + " [ " + this.Index.ToString() + " ]";
        }
    }
}
