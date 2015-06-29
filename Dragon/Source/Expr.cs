using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon
{
    public class Expr : Node
    {
        public Token Op { get; set; }
        public Type Type { get; set; }

        public Expr(Token tok, Type type) //private on book
        {
            this.Op = tok;
            this.Type = type;
        }

        public virtual Expr Gen()
        {
            return this;
        }

        //Computes or "reduce" an expression down to a single address;
        //That is, it returns a constant, an identifier, or a temproary name.
        public virtual Expr Reduce()
        {
            return this;
        }

        public virtual void Jumping(int lineForTrue, int lineForFalse)
        {
            this.EmitJumps(this.ToString(), lineForTrue, lineForFalse);
        }

        public void EmitJumps(string test, int lineForTrue, int lineForFalse)
        {
            if (lineForTrue != 0 && lineForFalse != 0)
            {
                this.Emit("if " + test + " goto L" + lineForTrue);
                this.Emit("goto L" + lineForFalse);
            }
            else if (lineForTrue != 0)
            {
                this.Emit("if " + test + " goto L" + lineForTrue);
            }
            else if (lineForFalse != 0)
            {
                this.Emit("iffalse " + test + " goto L" + lineForFalse);
            }
            else
            {
                ;
            }
        }

        public override string ToString()
        {
            return this.Op.ToString();
        }
    }


    public class Id : Expr
    {
        public int Offset { get; set; }
        public Id(Word id, Dragon.Type type, int offset)
            : base(id, type)
        {
            this.Offset = offset;
        }
    }


    public class Temp : Expr
    {
        static int Count = 0;
        public int Number { get; private set; }

        public Temp(Dragon.Type type)
            : base(Word.temp, type)
        {
            this.Number = ++Temp.Count;
        }

        public override string ToString()
        {
            return "t" + this.Number;
        }
    }


    public class Op : Expr
    {
        public Op(Token op, Dragon.Type type)
            : base(op, type)
        { }

        //Computes or "reduce" an expression down to a single address;
        //That is, it returns a constant, an identifier, or a temproary name.
        public override Expr Reduce()
        {
            Expr expr = this.Gen();  //Note: the method Gen is virtual, so polymorphism will happen here.
            Temp temp = new Temp(this.Type);
            this.Emit(temp.ToString() + " = " + expr.ToString());
            return temp;
        }
    }


    public class Arith : Op
    {
        public Expr ExprLeft;
        public Expr ExprRight;

        public Arith(Token op, Expr lhs, Expr rhs)
            : base(op, null)
        {
            this.ExprLeft = lhs;
            this.ExprRight = rhs;
            this.Type = Dragon.Type.Max(this.ExprLeft.Type, this.ExprRight.Type);
            if (this.Type == null)
                this.Error("type error");
        }

        public override Expr Gen()
        {
            return new Arith(this.Op, this.ExprLeft.Reduce(), this.ExprRight.Reduce());
        }

        public override string ToString()
        {
            return this.ExprLeft.ToString() + " " + this.Op.ToString() + " " + this.ExprRight.ToString();
        }
    }


    public class Unary : Op
    {
        public Expr Expr { get; private set; }

        public Unary(Token op, Expr expr)
            : base(op, Dragon.Type.Max(Dragon.Type.Int, expr.Type))
        {
            if (this.Type == null)
                this.Error("type error");
            this.Expr = expr;
        }

        public override Expr Gen()
        {
            return new Unary(this.Op, this.Expr.Reduce());
        }

        public override string ToString()
        {
            return this.Op.ToString() + " " + this.Expr.ToString();
        }
    }
}
