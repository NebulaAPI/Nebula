using System.Collections.Generic;
using Nebula.SDK.Objects;

namespace Nebula.Core.Parser
{
    public class NebulaParser : Parser
    {
        public NebulaParser(Tokenizer tokenizer) : base(tokenizer)
        {

        }

        public NebulaParser(string input) : base(input)
        {
            
        }

        public ModuleNode Parse(string moduleName)
        {
            var nodes = new List<AstNode>();
            while (!_tokenizer.Eof())
            {
                var obj = ParseObject();
                if (obj != null)
                {
                    nodes.Add(obj);
                }
            }
            
            return new ModuleNode(moduleName, nodes);
        }
        
        private AstNode ParseApi()
        {
            var apiNodes = new List<AstNode>();
            
            SkipKeyword("api");
            var token = _tokenizer.Next();
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
            var keyToken = _tokenizer.Next();
            SkipOp("=");
            var valueToken = _tokenizer.Next();
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
            var argumentName = _tokenizer.Next();
            SkipPunc(':');
            var argumentType = ParseDataType();
            return new ArgumentNode(argumentName.Value, argumentType);
        }

        private DataTypeNode ParseDataType()
        {
            var typeName = _tokenizer.Next();
            if (IsPunc('['))
            {
                SkipPunc('[');
                var genericType = _tokenizer.Next();
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

        private Token IsReturnTypeOp(string op)
        {
            var token = _tokenizer.Peek();
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
            var token = _tokenizer.Peek();
            if (token == null)
            {
                return null;
            }
            if (
                (token.Type == TokenType.GetFunction ||
                 token.Type == TokenType.PostFunction ||
                 token.Type == TokenType.PutFunction ||
                 token.Type == TokenType.DeleteFunction ||
                 token.Type == TokenType.PatchFunction) &&
                 (op == null || op == token.Value)
                )
            {
                return token;
            }
            return null;
        }

        private AstNode ParseFunction()
        {
            SkipKeyword("func");
            var functionName = _tokenizer.Next().Value;
            var arguments = Delimited('(', ')', ',', ParseFunctionArgument);
            var httpOp = IsHttpOp(null);
            if (httpOp != null)
            {
                _tokenizer.Next();
                var url = _tokenizer.Next();
                if (IsReturnTypeOp(null) != null)
                {
                    _tokenizer.Next();
                    var returnType = ParseDataType();
                    var docs = Delimited<KeyValueNode>('{', '}', ',', ParseKeyValue);
                    return new FunctionNode(functionName, arguments, httpOp.Type, url.Value, returnType, docs);
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
            var entityName = _tokenizer.Next();
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
    }
}