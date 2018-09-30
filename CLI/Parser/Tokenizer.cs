using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nebula.Parser
{
    public enum TokenType
    {
        Number,
        String,
        Boolean,
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
        public int Line { get; set; }
        public int StartPos { get; set; }
        public int Length { get; set; }
        public string LineText { get; set; }

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
                "entity", "api", "func", "exception", "export", "use", "config", "true", "false"
            };

        }

        private Token GenerateToken(TokenType type, string value)
        {
            var token = new Token(type, value);

            token.Line = Stream.Line;
            token.StartPos = Stream.Col - value.Length;
            token.Length = value.Length;
            token.LineText = Stream.Lines.Length >= token.Line ? Stream.Lines[token.Line] : "";

            return token;
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

        private Token ReadNumber(bool negative = false)
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
            return GenerateToken(TokenType.Number, (negative ? "-" : "") + number);
        }

        private Token ReadIdent()
        {
            var id = ReadWhile(IsId);
            if (IsKeyword(id))
            {
                if (id == "true" || id == "false")
                {
                    return GenerateToken(TokenType.Boolean, id);
                }
                return GenerateToken(TokenType.Keyword, id);
            }

            return GenerateToken(TokenType.Variable, id);
        }

        private Token ReadNegativeNumber()
        {
            Stream.Next();
            var ch = Stream.Peek();
            if (IsDigit(ch))
            {
                return ReadNumber(true);
            }
            return ReadNext();
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
            return GenerateToken(TokenType.String, ReadEscaped('"'));
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
            if (ch == '-')
            {
                return ReadNegativeNumber();
            }

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
                return GenerateToken(TokenType.Punctuation, Stream.Next().ToString());
            }

            if (IsOpChar(ch))
            {
                var opChar = ReadWhile(IsOpChar);
                switch (opChar)
                {
                    case "<<": return GenerateToken(TokenType.GetFunction, opChar);
                    case ">>": return GenerateToken(TokenType.PostFunction, opChar);
                    case ">|": return GenerateToken(TokenType.PutFunction, opChar);
                    case "><": return GenerateToken(TokenType.DeleteFunction, opChar);
                    case "->": return GenerateToken(TokenType.ReturnValue, opChar);
                    default: return GenerateToken(TokenType.Operation, opChar);
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