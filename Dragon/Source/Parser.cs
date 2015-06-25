using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon
{
    public class Parser
    {
        #region Fields
        private Lexer _lexer;
        private Token _look;
        public Env Top;
        public int Used;
        #endregion

        public Parser(Lexer lex)
        {
            this._lexer = lex;
            this.Move();
            this.Top = null;
            this.Used = 0;
        }

        public void Move ()
        {
            _look = _lexer.scan();
        }

        public void Error(string msg)
        {
            //note The book here is lex.line, but Line is static member, so this might be a bug.
            throw new Exception("near line " + Lexer.Line + ": " + msg);
        }

        public void Match(int tag)
        {
            if (_look.TagValue == tag) this.Move();
            else this.Error("syntax error");
        }

        public void Program()
        {
            Stmt s = this.Block();
            int begin = s.NewLable();
            int after = s.NewLable();
            s.EmitLabel(begin);
            s.Gen(begin, after);
            s.EmitLabel(after);
        }
        
        public Stmt Block()
        {
            this.Match('{');
            Env savedEnv = this.Top;
            this.Top = new Env(this.Top);

            this.Declarations();
            var s = this.Stmts();
            this.Match('}');
            this.Top = savedEnv;
            return s;
        }

        public void Declarations()
        {
            while(_look.TagValue == Tag.BASIC)
            {
                var type = this.Type();
                var tok = _look;
                this.Match(Tag.ID);
                this.Match(';');

                var id = new Id(tok as Word, type, this.Used);
                this.Top.Add(tok, id);
                this.Used += type.Width;
            }
        }

        public Dragon.Type Type()
        {
            var type = _look as Dragon.Type;    //expect _look.tag == Tag.Basic
            this.Match(Tag.BASIC);

            return _look.TagValue != '[' ? type : this.Dimension(type);
        }

        public Dragon.Type Dimension(Dragon.Type type)
        {
            this.Match('[');
            Token tok = _look;
            this.Match(Tag.NUM);
            this.Match(']');

            if (_look.TagValue == '[')
                type = this.Dimension(type);

            return new Array(((Num)tok).Value, type);
        }

        public Stmt Stmts()
        {
            if (_look.TagValue == '}')
                return Dragon.Stmt.Null;
            else
                return new Seq(this.Stmt(), this.Stmts());
        }

        public Stmt Stmt()
        {
            Expr expr;
            Stmt s1, s2, savedStmt;
            switch(_look.TagValue)
            {
                case ':':
                    this.Move();
                    return Dragon.Stmt.Null;

                case Tag.IF:
                    this.Match(Tag.IF);
                    this.Match('(');
                    expr = this.Bool();
                    this.Match(')');
                    
                    s1 = this.Stmt();
                    if (_look.TagValue != Tag.ELSE)
                        return new If(expr, s1);

                    this.Match(Tag.ELSE);
                    s2 = this.Stmt();
                    return new Else(expr, s1, s2);

                case Tag.WHILE:
                    var whileNode = new While();
                    savedStmt = Dragon.Stmt.Enclosing;
                    Dragon.Stmt.Enclosing = whileNode;
                    this.Match(Tag.WHILE);
                    this.Match('(');
                    expr = this.Bool();
                    this.Match(')');
                    s1 = this.Stmt();
                    whileNode.Init(expr, s1);
                    Dragon.Stmt.Enclosing = savedStmt;
                    return whileNode;

                case Tag.DO:
                    var doNode = new Do();
                    savedStmt = Dragon.Stmt.Enclosing;
                    Dragon.Stmt.Enclosing = doNode;
                    this.Match(Tag.DO);
                    s1 = this.Stmt();
                    this.Match(Tag.WHILE);
                    this.Match('(');
                    expr = this.Bool();
                    this.Match(')');
                    this.Match(';');
                    doNode.Init(s1, expr);
                    Dragon.Stmt.Enclosing = savedStmt;
                    return doNode;

                case Tag.BREAK:
                    this.Match(Tag.BREAK);
                    this.Match(';');
                    return new Break();

                case '{':
                    return this.Block();

                default:
                    return this.Assign();
            }
        }

        public Stmt Assign()
        {
            Stmt stmt;
            var tok = _look;

            this.Match(Tag.ID);
            var id = Top.Get(tok);
            if (id == null)
                this.Error(tok.ToString() + " undeclared");

            if(_look.TagValue == '=')
            {
                this.Move();
                stmt = new Set(id, this.Bool());
            }
            else 
            {
                Access x = this.Offset(id);
                this.Match('=');
                stmt = new SetElem(x, this.Bool());
            }

            this.Match(';');
            return stmt;
        }    

        public Expr Bool()
        {
            Expr expr = this.Join();
            while(_look.TagValue == Tag.OR)
            {
                var tok = _look;
                this.Move();
                expr = new Or(tok, expr, this.Join());
            }
            return expr;
        }

        public Expr Join()
        {
            Expr expr = this.Equality();
            while(_look.TagValue == Tag.AND)
            {
                var tok = _look;
                this.Move();
                expr = new And(tok, expr, this.Equality());
            }
            return expr;
        }

        public Expr Equality()
        {
            Expr expr = this.Rel();
            while(_look.TagValue == Tag.EQ || _look.TagValue == Tag.NE)
            {
                var tok = _look;
                this.Move();
                expr = new Rel(tok, expr, this.Rel());
            }
            return expr;
        }

        public Expr Rel()
        {
            Expr expr = this.Expr();
            if('<' == _look.TagValue || Tag.LE == _look.TagValue || Tag.GE == _look.TagValue || '>' == _look.TagValue)
            {
                Token tok = _look;
                this.Move();
                return new Rel(tok, expr, this.Expr());
            }
            return expr;
        }

        public Expr Expr()
        {
            Expr expr = this.Term();
            while(_look.TagValue == '+' || _look.TagValue == '-')
            {
                Token tok = _look;
                this.Move();
                expr = new Arith(tok, expr, this.Term());
            }
            return expr;
        }

        public Expr Term()
        {
            Expr expr = this.Unary();
            while(_look.TagValue == '*' || _look.TagValue == '/')
            {
                Token tok = _look;
                this.Move();
                expr = new Arith(tok, expr, this.Unary());
            }
            return expr;
        }

        public Expr Unary()
        {
            if (_look.TagValue == '-')
            {
                this.Move();
                return new Unary(Word.minus, this.Unary());
            }
            else if (_look.TagValue == '!')
            {
                Token tok = _look;
                this.Move();
                return new Not(tok, this.Unary());
            }
            else
                return this.Factor();
        }

        public Expr Factor()
        {
            Expr expr = null;
            switch(_look.TagValue)
            {
                case'(':
                    this.Move();
                    expr = this.Bool();
                    this.Match(')');
                    return expr;

                case Tag.NUM:
                    expr = new Constant(_look, Dragon.Type.Int);
                    this.Move();
                    return expr;

                case Tag.REAL:
                    expr = new Constant(_look, Dragon.Type.Float);
                    this.Move();
                    return expr;

                case Tag.TRUE:
                    expr = Constant.True;
                    this.Move();
                    return expr;

                case Tag.FALSE:
                    expr = Constant.False;
                    this.Move();
                    return expr;

                default:
                    this.Error("syntax error");
                    return expr;

                case Tag.ID:
                    string s = _look.ToString();
                    Id id = this.Top.Get(_look);
                    if (id == null)
                        this.Error(_look.ToString() + " undeclared");
                    this.Move();
                    if (_look.TagValue != '[')
                        return id;
                    else
                        return this.Offset(id);
            }
        }

        public Access Offset(Id a)
        {
            Expr i, w, t1, t2, loc;
            Type type = a.Type;
            this.Match('[');
            i = this.Bool();
            this.Match(']');
            type = (type as Array).Of;
            w = new Constant(type.Width);
            t1 = new Arith(new Token('*'), i, w);
            loc = t1;
            while(_look.TagValue == '[')
            {
                this.Match('[');
                i = this.Bool();
                this.Match(']');
                type = (type as Array).Of;
                w = new Constant(type.Width);
                t1 = new Arith(new Token('*'), i, w);
                t2 = new Arith(new Token('+'), loc, t1);
                loc = t2;
            }
            return new Access(a, loc, type);
        }
    }
}
