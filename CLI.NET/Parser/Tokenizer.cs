using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CLI.NET.Parser
{
    public enum TokenType
    {
        Number,
        String,
        Keyword,
        Variable,
        Punctuation,
        Operation,
        GetFunction,
        PostFunction,
        PutFunction,
        DeleteFunction,
        ReturnValue
    }
    
    public class Token
    {
        public string Value { get; private set; }
        public TokenType Type { get; private set; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type}: {Value}";
        }
    }
    
    public class Tokenizer
    {
        private InputStream Stream { get; set; }
        private Token CurrentToken { get; set; }
        private List<string> Keywords { get; set; }
        public Tokenizer(InputStream stream)
        {
            Stream = stream;
            Keywords = new List<string>
            {
                "entity", "api", "func", "exception", "export", "use", "config"
            };

        }

        private bool IsKeyword(string s)
        {
            return Keywords.Contains(s);
        }

        private bool IsDigit(char c)
        {
            return new Regex("[0-9]").IsMatch(c.ToString());
        }

        private bool IsIdStart(char c)
        {
            return new Regex("[a-z]", RegexOptions.IgnoreCase).IsMatch(c.ToString());
        }

        private bool IsId(char c)
        {
            return IsIdStart(c) || "?!<=0123456789".IndexOf(c) >= 0;
        }

        private bool IsOpChar(char c)
        {
            return "+-*/%=&|<>!.".IndexOf(c) >= 0;
        }

        private bool IsPunc(char c)
        {
            return ",;(){}[]:".IndexOf(c) >= 0;
        }

        private bool IsWhitespace(char c)
        {
            return " \t\n\r".IndexOf(c) >= 0;
        }

        private string ReadWhile(Func<char, bool> predicate)
        {
            var str = "";
            while (!Stream.Eof() && predicate(Stream.Peek()))
            {
                str += Stream.Next();
            }
            return str;
        }

        private Token ReadNumber()
        {
            var hasDot = false;
            var number = ReadWhile((ch) => {
                if (ch == '.')
                {
                    if (hasDot)
                    {
                        return false;
                    }
                    hasDot = true;
                    return true;
                }
                return IsDigit(ch);
            });
            return new Token(TokenType.Number, number);
        }

        private Token ReadIdent()
        {
            var id = ReadWhile(IsId);
            if (IsKeyword(id))
            {
                return new Token(TokenType.Keyword, id);
            }

            return new Token(TokenType.Variable, id);
        }

        private string ReadEscaped(char end)
        {
            var escaped = false;
            var str = "";
            Stream.Next();
            while (!Stream.Eof())
            {
                var ch = Stream.Next();
                if (escaped)
                {
                    str += ch;
                    escaped = false;
                }
                else if (ch == '\\')
                {
                    escaped = true;
                }
                else if (ch == end)
                {
                    break;
                }
                else
                {
                    str += ch;
                }
            }
            return str;
        }

        private Token ReadString()
        {
            return new Token(TokenType.String, ReadEscaped('"'));
        }

        private void SkipComment()
        {
            ReadWhile((ch) => {
                return ch != '\n' && ch != '\r';
            });
            Stream.Next();
        }

        private Token ReadNext()
        {
            ReadWhile(IsWhitespace);
            if (Stream.Eof())
            {
                return null;
            }

            var ch = Stream.Peek();
            if (ch == '#')
            {
                SkipComment();
                return ReadNext();
            }

            if (ch == '"')
            {
                return ReadString();
            }

            if (IsDigit(ch))
            {
                return ReadNumber();
            }

            if (IsIdStart(ch))
            {
                return ReadIdent();
            }

            if (IsPunc(ch))
            {
                return new Token(TokenType.Punctuation, Stream.Next().ToString());
            }

            if (IsOpChar(ch))
            {
                var opChar = ReadWhile(IsOpChar);
                switch (opChar)
                {
                    case "<<": return new Token(TokenType.GetFunction, opChar);
                    case ">>": return new Token(TokenType.PostFunction, opChar);
                    case ">|": return new Token(TokenType.PutFunction, opChar);
                    case "><": return new Token(TokenType.DeleteFunction, opChar);
                    case "->": return new Token(TokenType.ReturnValue, opChar);
                    default: return new Token(TokenType.Operation, opChar);
                }
            }
            Stream.Error("Can't handle character: " + ch);
            return null;
        }

        public Token Peek()
        {
            if (CurrentToken != null)
            {
                return CurrentToken;
            }
            CurrentToken = ReadNext();
            return CurrentToken;
        }

        public Token Next()
        {
            var tok = CurrentToken;
            CurrentToken = null;
            if (tok != null)
            {
                return tok;
            }
            return ReadNext();
        }

        public bool Eof()
        {
            return Peek() == null;
        }

        public void Error(string msg)
        {
            Stream.Error(msg);
        }

        public void Reset()
        {
            CurrentToken = null;
            Stream.Reset();
        }

    }
}