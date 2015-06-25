//
//  corresponds to Inter package in dragon book
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dragon
{
    public class Node
    {
        int _lexLine;
        static int _labels = 0;

        public Node() //private on book
        { 
            _lexLine = Lexer.Line; 
        }

        public void Error(string msg)
        {
            throw new Exception("near line " + _lexLine + ": " + msg);
        }

        public int NewLable()
        {
            return ++Node._labels;
        }

        public void EmitLabel(int i)
        {
            Console.Write("L" + i + ":");
        }

        public void Emit(string s)
        {
            Console.Write("\t" + s);
        }
    }


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

        public virtual Expr Reduce()
        {
            return this;
        }

        public virtual void Jumping(int t, int f)
        {
            this.EmitJumps(this.ToString(), t, f);
        }

        public void EmitJumps(string test, int lineForTrue, int lineForFalse)
        {
            if(lineForTrue != 0 && lineForFalse != 0)
            {
                this.Emit("if " + test + " goto L" + lineForTrue); 
                this.Emit("goto L" + lineForFalse);
            }
            else if(lineForTrue != 0)
            {
                this.Emit("if " + test + " goto L" + lineForTrue);
            }
            else if(lineForFalse != 0)
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
        int _number;

        public Temp(Type type)
            : base(Word.temp, type)
        {
            this._number = ++Temp.Count;
        }

        public override string ToString()
        {
            return "t" + _number;
        }
    }
    

    // not tested yet
    public class Op : Expr
    {
        public Op(Token tok, Type type)
            : base(tok, type)
        { }

        public override Expr Reduce()
        {
            Expr expr = this.Gen();
            Temp temp = new Temp(this.Type);
            this.Emit(temp.ToString() + " = " + expr.ToString());
            return temp;
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


    public class Arith : Op
    {
        public Expr ExprLeft;
        public Expr ExprRight;

        public Arith(Token tok, Expr lhs, Expr rhs)
            : base(tok, null)
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
        public Expr Expr;
        
        public Unary(Token tok, Expr expr)
            : base(tok, null)
        {
            this.Expr = expr;
            this.Type = Dragon.Type.Max(Dragon.Type.Int, this.Expr.Type);
            if (this.Type == null)
                this.Error("type error");
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
