using System;
using System.Collections.Generic;
using System.IO;

namespace Nebula.Parser
{
    public class Parser
    {
        private Tokenizer Tokenizer { get; set; }
        public Parser(Tokenizer tokenizer)
        {
            Tokenizer = tokenizer;
        }

        public Parser(string inputFile)
        {
            Tokenizer = new Tokenizer(new InputStream(File.ReadAllText(inputFile)));
        }

        private bool IsPunc(char? c)
        {
            var token = Tokenizer.Peek();
            if (token == null)
            {
                return false;
            }
            return token.Type == TokenType.Punctuation && (c == null || token.Value == c.ToString());
        }

        private Token IsKeyword(string kw)
        {
            var token = Tokenizer.Peek();
            if (token == null)
            {
                return null;
            }
            return token.Type == TokenType.Keyword && (kw == null || token.Value == kw) ? token : null;
        }

        private Token IsOp(string op)
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

        private Token IsReturnTypeOp(string op)
        {
            var token = Tokenizer.Peek();
            if (token == null)
            {
                return null;
            }
            if (token.Type == TokenType.ReturnValue && (op == null || op == token.Value))
            {
                return token;
            }
            return null;
        }

        private Token IsHttpOp(string op)
        {
            var token = Tokenizer.Peek();
            if (token == null)
            {
                return null;
            }
            if (
                (token.Type == TokenType.GetFunction ||
                 token.Type == TokenType.PostFunction ||
                 token.Type == TokenType.PutFunction ||
                 token.Type == TokenType.DeleteFunction) &&
                 (op == null || op == token.Value)
                )
            {
                return token;
            }
            return null;
        }

        private void SkipPunc(char c)
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

        private void SkipKeyword(string kw)
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

        private void SkipOp(string op)
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

        private void Unexpected()
        {
            Tokenizer.Error("Unexpected token: " + Tokenizer.Peek());
        }

        private List<T> Delimited<T>(char start, char stop, char separator, Func<T> parser) where T : AstNode
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

        private AstNode ParseApi()
        {
            var apiNodes = new List<AstNode>();
            
            SkipKeyword("api");
            var token = Tokenizer.Next();
            var apiName = token.Value;
            SkipPunc('{');
            while (!IsPunc('}'))
            {
                token = IsKeyword(null);
                if (token != null)
                {
                    switch (token.Value)
                    {
                        case "config": 
                            apiNodes.Add(ParseConfig()); 
                            break;
                        case "func": 
                            apiNodes.Add(ParseFunction()); 
                            break;
                        default:
                            Unexpected(); 
                            break;
                    }
                }
            }
            SkipPunc('}');
            return new ApiNode(apiName, apiNodes);
        }

        private KeyValueNode ParseKeyValue()
        {
            var keyToken = Tokenizer.Next();
            SkipOp("=");
            var valueToken = Tokenizer.Next();
            return new KeyValueNode(keyToken.Value, valueToken.Value);
        }

        private AstNode ParseConfig()
        {
            SkipKeyword("config");
            var keyValuePairs = Delimited('{', '}', ',', ParseKeyValue);
            return new ConfigNode(keyValuePairs);
        }

        private ArgumentNode ParseFunctionArgument()
        {
            var argumentName = Tokenizer.Next();
            SkipPunc(':');
            var argumentType = ParseDataType();
            return new ArgumentNode(argumentName.Value, argumentType);
        }

        private DataTypeNode ParseDataType()
        {
            var typeName = Tokenizer.Next();
            if (IsPunc('['))
            {
                SkipPunc('[');
                var genericType = Tokenizer.Next();
                if (IsPunc(']'))
                {
                    SkipPunc(']');
                    return new DataTypeNode(typeName.Value, true, genericType.Value);
                }
                else
                {
                    Unexpected();
                    return null;
                }
            }
            return new DataTypeNode(typeName.Value, false, null);
        }

        private AstNode ParseFunction()
        {
            SkipKeyword("func");
            var functionName = Tokenizer.Next().Value;
            var arguments = Delimited('(', ')', ',', ParseFunctionArgument);
            var httpOp = IsHttpOp(null);
            if (httpOp != null)
            {
                Tokenizer.Next();
                var url = Tokenizer.Next();
                if (IsReturnTypeOp(null) != null)
                {
                    Tokenizer.Next();
                    var returnType = ParseDataType();
                    return new FunctionNode(functionName, arguments, httpOp.Type, url.Value, returnType);
                }
                else
                {
                    Unexpected();
                    return null;
                }
            }
            else
            {
                Unexpected();
                return null;
            }
        }

        private AstNode ParseEntity()
        {
            SkipKeyword("entity");
            var entityName = Tokenizer.Next();
            var fields = Delimited('{', '}', ',', ParseFunctionArgument);
            return new EntityNode(entityName.Value, fields);
        }

        private AstNode ParseObject()
        {
            var token = IsKeyword(null);
            if (token != null)
            {
                switch (token.Value)
                {
                    case "api": return ParseApi();
                    case "entity": return ParseEntity();
                }
            }
            Unexpected();
            return null;
        }

        public ModuleNode Parse(string moduleName)
        {
            var nodes = new List<AstNode>();
            while (!Tokenizer.Eof())
            {
                var obj = ParseObject();
                if (obj != null)
                {
                    nodes.Add(obj);
                }
            }
            
            return new ModuleNode(moduleName, nodes);
        }
    }
}