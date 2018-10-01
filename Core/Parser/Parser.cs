using System;
using System.Collections.Generic;
using System.IO;

namespace Nebula.Parser
{
    public abstract class Parser
    {
        protected Tokenizer Tokenizer { get; set; }
        protected Parser(Tokenizer tokenizer)
        {
            Tokenizer = tokenizer;
        }

        protected Parser(string inputFile)
        {
            var finfo = new FileInfo(inputFile);
            Tokenizer = new Tokenizer(new InputStream(finfo));
        }

        protected Token IsString(string str)
        {
            var token = Tokenizer.Peek();
            if (token == null)
            {
                return null;
            }

            return token.Type == TokenType.String ? token : null;
        }

        protected bool IsPunc(char? c)
        {
            var token = Tokenizer.Peek();
            if (token == null)
            {
                return false;
            }
            return token.Type == TokenType.Punctuation && (c == null || token.Value == c.ToString());
        }

        protected Token IsKeyword(string kw)
        {
            var token = Tokenizer.Peek();
            if (token == null)
            {
                return null;
            }
            return token.Type == TokenType.Keyword && (kw == null || token.Value == kw) ? token : null;
        }

        protected Token IsOp(string op)
        {
            var token = Tokenizer.Peek();
            if (token == null)
            {
                return null;
            }
            if (token.Type == TokenType.Operation && (op == null || op == token.Value))
            {
                return token;
            }
            return null;
        }

        protected void SkipPunc(char c)
        {
            if (IsPunc(c))
            {
                Tokenizer.Next();
            }
            else
            {
                Tokenizer.Error("Expecting punctuation: " + c);
            }
        }

        protected void SkipKeyword(string kw)
        {
            if (IsKeyword(kw) != null)
            {
                Tokenizer.Next();
            }
            else
            {
                Tokenizer.Error("Expecting keyword: " + kw);
            }
        }

        protected void SkipOp(string op)
        {
            if (IsOp(op) != null)
            {
                Tokenizer.Next();
            }
            else
            {
                Tokenizer.Error("Expecting operator: " + op);
            }
        }

        protected void Unexpected()
        {
            Tokenizer.Error("Unexpected token: " + Tokenizer.Peek());
        }

        protected List<T> Delimited<T>(char start, char stop, char separator, Func<T> parser) where T : AstNode
        {
            var nodes = new List<T>();
            var first = true;

            SkipPunc(start);
            while (!Tokenizer.Eof())
            {
                if (IsPunc(stop))
                {
                    break;
                }
                if (first)
                {
                    first = false;
                }
                else
                {
                    SkipPunc(separator);
                }
                if (IsPunc(stop))
                {
                    break;
                }
                nodes.Add(parser());
            }
            SkipPunc(stop);
            return nodes;
        }

    }
}