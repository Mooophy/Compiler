using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon
{
    public class Stmt : Node
    {
        public Stmt() { }
        public static Stmt Null = new Stmt();

        public virtual void Gen(int beginning, int after) { }
        public int After = 0;
        public static Stmt Enclosing = Stmt.Null;
    }


    public class If : Stmt
    {
        public Expr Expr;
        public Stmt Stmt;

        public If(Expr expr, Stmt stmt)
        {
            this.Expr = expr;
            this.Stmt = stmt;
            if(this.Expr.Type != Type.Bool)
                this.Expr.Error("boolean required in if");
        }

        public override void Gen (int beginning, int after)
        {
            int lable = this.NewLable();    //label for the code of "for"
            this.Expr.Jumping(0, after);    //fall through on true, goto "after" on false
            this.EmitLabel(lable);
            this.Stmt.Gen(lable, after);
        }
    }


    public class Else : Stmt
    {
        public Expr Expr;
        public Stmt Stmt1, Stmt2;

        public Else(Expr expr, Stmt stmt1, Stmt stmt2)
        {
            this.Expr = expr;
            this.Stmt1 = stmt1;
            this.Stmt2 = stmt2;
            if (this.Expr.Type != Dragon.Type.Bool)
                this.Expr.Error("boolean required in if");
        }

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


    public class While : Stmt
    {
        public Expr Expr;
        public Stmt Stmt;

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

        public override void Gen(int beginning, int after)
        {
            this.After = after;             //save after
            this.Expr.Jumping(0, after);
            int label = this.NewLable();    //label for stmt
            this.EmitLabel(label);
            this.Stmt.Gen(label, beginning);
            this.Emit("goto L " + beginning);
        }
    }


    public class Do : Stmt
    {
        public Expr Expr;
        public Stmt Stmt;

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

        public override void Gen(int beginning, int after)
        {
            this.After = after;
            int label = this.NewLable();
            this.Stmt.Gen(beginning, label);
            this.EmitLabel(label);
            this.Expr.Jumping(beginning, 0);
        }
    }


    //for assignment
    public class Set : Stmt
    {
        public Id Id;
        public Expr Expr;

        public Set(Id id, Expr expr)
        {
            this.Id = id;
            this.Expr = expr;
            if( null == this.Check(this.Id.Type, this.Expr.Type))
                this.Error("type error");
        }

        public Type Check(Type lhs, Type rhs)
        {
            if (Dragon.Type.Numeric(lhs) && Dragon.Type.Numeric(rhs)) return rhs;//why rhs?
            else if (lhs == Dragon.Type.Bool && rhs == Dragon.Type.Bool) return rhs;
            else return null;
        }

        public override void Gen(int beginning, int after)
        {
            this.Emit(this.Id.ToString() + " = " + this.Expr.Gen().ToString());
        }
    }


    //assignment for an array element
    public class SetElem : Stmt
    {
        public Id Array;
        public Expr Index;
        public Expr Expr;

        public SetElem(Access access, Expr expr)
        {
            this.Array = access.Array;
            this.Index = access.Index;
            this.Expr = expr;
            if (null == this.Check(access.Type, this.Expr.Type))
                this.Error("type error");
        }

        public Type Check(Type lhs, Type rhs)
        {
            if (lhs is Array || rhs is Array) return null;
            else if (lhs == rhs) return rhs;
            else if (Dragon.Type.Numeric(lhs) && Dragon.Type.Numeric(rhs)) return rhs;
            else return null;
        }

        public override void Gen(int beginning, int after)
        {
            string s1 = this.Index.Reduce().ToString();
            string s2 = this.Expr.Reduce().ToString();
            this.Emit(this.Array.ToString() + " [ " + s1 + " ] = " + s2);
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
