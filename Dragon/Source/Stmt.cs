namespace Dragon
{
    public class Stmt : Node
    {
        public int After { get; protected set; }
        /// <summary>
        /// Ctor, does nothing, the work is done in the subclasses
        /// </summary>
        public Stmt() 
        {
            this.After = 0;
        }
        /// <summary>
        /// Virtual
        /// </summary>
        /// <param name="begin">label</param>
        /// <param name="after">label</param>
        public virtual void Gen(int begin, int after) { }
        /// <summary>
        /// Representing an empty sequence
        /// </summary>
        public static Stmt Null = new Stmt();
        /// <summary>
        /// Used during parsing to keep track of the enclosing construct.
        /// </summary>
        public static Stmt Enclosing = Stmt.Null;
    }


    /// <summary>
    /// if (Expr) Stmt
    /// </summary>
    public class If : Stmt
    {
        /// <summary>
        /// Node Expr
        /// </summary>
        public Expr Expr { get; private set; }
        /// <summary>
        /// Node Stmt
        /// </summary>
        public Stmt Stmt { get; private set; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="stmt"></param>
        public If(Expr expr, Stmt stmt)
        {
            this.Expr = expr;
            this.Stmt = stmt;
            if(this.Expr.Type != Type.Bool)
                this.Expr.Error("boolean required in if");
        }
        /// <summary>
        /// Just a placeholder for the Gen methods in the subclasses
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="after"></param>
        public override void Gen (int begin, int after)
        {
            var lable = this.NewLable();    //label for the code for stmt
            this.Expr.Jumping(0, after);    //fall through on true, goto "after" on false
            this.EmitLabel(lable);
            this.Stmt.Gen(lable, after);
        }
    }


    /// <summary>
    /// if (Expr) Stmt1 else stmt2
    /// </summary>
    public class IfElse : Stmt
    {
        public Expr Expr { get; private set; }
        public Stmt Stmt1 { get; private set; }
        public Stmt Stmt2 { get; private set; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="expr">bool</param>
        /// <param name="stmt1">stmt for true</param>
        /// <param name="stmt2">stmt for false</param>
        public IfElse(Expr expr, Stmt stmt1, Stmt stmt2)
        {
            this.Expr = expr;
            this.Stmt1 = stmt1;
            this.Stmt2 = stmt2;
            if (this.Expr.Type != Dragon.Type.Bool)
                this.Expr.Error("boolean required in if");
        }
        /// <summary>
        /// Overriding
        /// </summary>
        /// <param name="beginning"></param>
        /// <param name="after"></param>
        public override void Gen(int beginning, int after)
        {
            int label1 = this.NewLable();
            int lable2 = this.NewLable();
            this.Expr.Jumping(0, lable2);

            this.EmitLabel(label1);
            this.Stmt1.Gen(label1, after);
            this.Emit("goto L" + after);

            this.EmitLabel(lable2);
            this.Stmt2.Gen(lable2, after);
        }
    }


    /// <summary>
    /// while (bool) stmt
    /// </summary>
    public class While : Stmt
    {
        public Expr Expr { get; private set; }
        public Stmt Stmt { get; private set; }

        public While()
        {
            this.Expr = null;
            this.Stmt = null;
        }

        public void Init(Expr expr, Stmt stmt)
        {
            this.Expr = expr;
            this.Stmt = stmt;
            if (this.Expr.Type != Type.Bool)
                this.Expr.Error("boolean requried in while");
        }

        public override void Gen(int begin, int after)
        {
            this.After = after;             //save label after
            this.Expr.Jumping(0, after);
            var label = this.NewLable();    //label for stmt
            this.EmitLabel(label);
            this.Stmt.Gen(label, begin);
            this.Emit("goto L " + begin);
        }
    }


    /// <summary>
    /// do stmt while (bool)
    /// </summary>
    public class Do : Stmt
    {
        public Expr Expr { get; private set; }
        public Stmt Stmt { get; private set; }

        public Do()
        {
            this.Expr = null;
            this.Stmt = null;
        }

        public void Init(Stmt stmt, Expr expr)
        {
            this.Expr = expr;
            this.Stmt = stmt;
            if (this.Expr.Type != Type.Bool)
                this.Expr.Error("boolean requried in do");
        }

        public override void Gen(int begin, int after)
        {
            this.After = after;
            var label = this.NewLable();
            this.Stmt.Gen(begin, label);
            this.EmitLabel(label);
            this.Expr.Jumping(begin, 0);
        }
    }


    /// <summary>
    /// Implementing assignment
    /// </summary>
    public class Set : Stmt
    {
        public Id Id { get; private set; }
        public Expr Expr { get; private set; }

        public Set(Id id, Expr expr)
        {
            this.Id = id;
            this.Expr = expr;
            if( null == this.Check(this.Id.Type, this.Expr.Type))
                this.Error("type error");
        }

        public Dragon.Type Check(Dragon.Type lhs, Dragon.Type rhs)
        {
            if (Dragon.Type.Numeric(lhs) && Dragon.Type.Numeric(rhs)) 
                return rhs;
            else if (lhs == Dragon.Type.Bool && rhs == Dragon.Type.Bool) 
                return rhs;
            else 
                return null;
        }

        public override void Gen(int begin, int after)
        {
            this.Emit(this.Id.ToString() + " = " + this.Expr.Gen().ToString());
        }
    }


    /// <summary>
    /// Implementing assignment to an array element
    /// </summary>
    public class SetElem : Stmt
    {
        public Id Array { get; private set; }
        public Expr Index { get; private set; }
        public Expr Expr { get; private set; }

        public SetElem(Access access, Expr expr)
        {
            this.Array = access.Array;
            this.Index = access.Index;
            this.Expr = expr;
            if (null == this.Check(access.Type, this.Expr.Type))
                this.Error("type error");
        }

        public Dragon.Type Check(Dragon.Type lhs, Dragon.Type rhs)
        {
            if (lhs is Array || rhs is Array) return null;
            else if (lhs == rhs) return rhs;
            else if (Dragon.Type.Numeric(lhs) && Dragon.Type.Numeric(rhs)) return rhs;
            else return null;
        }

        public override void Gen(int beginning, int after)
        {
            var idx = this.Index.Reduce().ToString();
            var val = this.Expr.Reduce().ToString();
            this.Emit(this.Array.ToString() + " [ " + idx + " ] = " + val);
        }
    }


    public class Seq : Stmt 
    {
        public Stmt Stmt1;
        public Stmt Stmt2;
        
        public Seq(Stmt stmt1, Stmt stmt2)
        {
            this.Stmt1 = stmt1;
            this.Stmt2 = stmt2;
        }

        public override void Gen(int beginning, int after)
        {
            if (this.Stmt1 == Stmt.Null)
            {
                this.Stmt2.Gen(beginning, after);
            }
            else if (this.Stmt2 == Stmt.Null)
            {
                this.Stmt1.Gen(beginning, after); 
            }
            else
            {
                int label = this.NewLable();
                this.Stmt1.Gen(beginning, label);
                this.EmitLabel(label);
                this.Stmt2.Gen(label, after);
            }
        }
    }

    public class Break : Stmt
    {
        public Stmt Stmt;

        public Break()
        {
            if (Stmt.Enclosing == null)
                this.Error("unenclosed break");
            this.Stmt = Stmt.Enclosing;
        }

        public override void Gen(int beginning, int after)
        {
            this.Emit("goto L" + this.Stmt.After);
        }
    }
}
