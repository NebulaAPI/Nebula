using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nebula.SDK.Objects;

namespace Nebula.Core.Parser
{
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
        private InputStream _stream;
        private Token _currentToken;
        private List<string> _keywords;

        public Tokenizer(InputStream stream)
        {
            _stream = stream;
            _keywords = new List<string>
            {
                "entity", "api", "func", "exception", "export", "use", "config", "true", "false"
            };

        }

        private Token GenerateToken(TokenType type, string value)
        {
            var token = new Token(type, value);

            token.Line = _stream.Line;
            token.StartPos = _stream.Col - value.Length;
            token.Length = value.Length;
            token.LineText = _stream.Lines.Length >= token.Line ? _stream.Lines[token.Line] : "";

            return token;
        }

        private bool IsKeyword(string s)
        {
            return _keywords.Contains(s);
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
            while (!_stream.Eof() && predicate(_stream.Peek()))
            {
                str += _stream.Next();
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
            return GenerateToken(TokenType.Number, number);
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
            _stream.Next();
            var ch = _stream.Peek();
            if (IsDigit(ch))
            {
                return ReadNumber();
            }
            return ReadNext();
        }

        private string ReadEscaped(char end)
        {
            var escaped = false;
            var str = "";
            _stream.Next();
            while (!_stream.Eof())
            {
                var ch = _stream.Next();
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
            _stream.Next();
        }

        private Token ReadNext()
        {
            ReadWhile(IsWhitespace);
            if (_stream.Eof())
            {
                return null;
            }

            var ch = _stream.Peek();
            // if (ch == '-')
            // {
            //     return ReadNegativeNumber();
            // }

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
                return GenerateToken(TokenType.Punctuation, _stream.Next().ToString());
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
                    case ">@": return GenerateToken(TokenType.PatchFunction, opChar);
                    case "->": return GenerateToken(TokenType.ReturnValue, opChar);
                    default: return GenerateToken(TokenType.Operation, opChar);
                }
            }
            _stream.Error("Can't handle character: " + ch);
            return null;
        }

        public Token Peek()
        {
            if (_currentToken != null)
            {
                return _currentToken;
            }
            _currentToken = ReadNext();
            return _currentToken;
        }

        public Token Next()
        {
            var tok = _currentToken;
            _currentToken = null;
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
            _stream.Error(msg);
        }

        public void Reset()
        {
            _currentToken = null;
            _stream.Reset();
        }

    }
}