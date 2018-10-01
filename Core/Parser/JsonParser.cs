using System.Collections.Generic;
using System.Linq;
using SharpPad;

namespace Nebula.Parser
{
    public class JsonObject : AstNode
    {
        public Dictionary<string, JsonObject> Objects { get; set; }

        public Dictionary<string, List<JsonArray>> Arrays { get; set; }

        public Dictionary<string, JsonValue> Values { get; set; }
        
        public JsonObject() : base("jobj")
        {
            Objects = new Dictionary<string, JsonObject>();
            Arrays = new Dictionary<string, List<JsonArray>>();
            Values = new Dictionary<string, JsonValue>();
        }

    }

    public class JsonArray : AstNode
    {
        public List<JsonValue> Values { get; set; }

        public List<JsonObject> Objects { get; set; }

        public List<JsonArray> Arrays { get; set; }
        
        public JsonArray() : base("jarr")
        {
            Values = new List<JsonValue>();
            Objects = new List<JsonObject>();
            Arrays = new List<JsonArray>();
        }
    }

    public class JsonValue : AstNode
    {
        public JsonValue(dynamic val) : base("jval")
        {
            Value = val;
        }

        public dynamic Value { get; set; }
    }
    
    public class JsonParser : Parser
    {
        public JsonParser(Tokenizer tokenizer) : base(tokenizer)
        {
            Tokenizer = tokenizer;
        }

        public AstNode Parse()
        {
            while (!Tokenizer.Eof())
            {
                if (IsPunc('{'))
                {
                    var root = new JsonObject();
                    var objs = Delimited<JsonObject>('{', '}', ',', ParseObject);
                    root.Objects.Add("", objs.FirstOrDefault());
                    root.Dump();
                    return root;
                }

                if (IsPunc('['))
                {
                    var root = new JsonArray();
                    var arrays = Delimited<JsonArray>('[', ']', ',', ParseArray);
                    root.Arrays.Add(arrays.FirstOrDefault());
                    return root;
                }
            }
            return null;
        }

        private Token IsValue()
        {
            var token = Tokenizer.Peek();
            if (token == null)
            {
                return null;
            }
            return token.Type == TokenType.String 
                || token.Type == TokenType.Number 
                || token.Type == TokenType.Keyword
                || token.Type == TokenType.Boolean
                || (token.Type == TokenType.Operation && token.Value == "-")
                ? token : null;
        }

        private bool IsObject()
        {
            var token = Tokenizer.Peek();
            if (token == null)
            {
                return false;
            }
            return token.Type == TokenType.Punctuation && token.Value == "{";
        }

        private bool IsArray()
        {
            var token = Tokenizer.Peek();
            if (token == null)
            {
                return false;
            }
            return token.Type == TokenType.Punctuation && token.Value == "[";
        }

        private JsonArray ParseArray()
        {
            var array = new JsonArray();
            if (IsArray())
            {
                array.Arrays.AddRange(Delimited<JsonArray>('[', ']', ',', ParseArray));
            }
            var token = IsValue();
            if (token != null)
            {
                array.Values.Add(ParseValue(token));
                Tokenizer.Next();
            }
            return array;
        }

        private JsonValue ParseValue(Token valueToken)
        {
            var token = IsOp("-");
            if (token != null)
            {
                Tokenizer.Next();
                var nextToken = IsValue();
                if (nextToken != null)
                {
                    return new JsonValue("-" + nextToken.Value);
                }
            }
            return new JsonValue(valueToken.Value);
        }

        private JsonObject ParseObject()
        {
            var obj = new JsonObject();
            var token = IsString(null);
            if (token != null)
            {
                Tokenizer.Next();
                SkipPunc(':');
                if (IsObject())
                {
                    //var o = Delimited<JsonObject>('{', '}', ',', ParseObject);
                    SkipPunc('{');
                    obj.Objects.Add(token.Value, ParseObject());
                }

                if (IsArray())
                {
                    var elements = Delimited<JsonArray>('[', ']', ',', ParseArray);
                    obj.Arrays.Add(token.Value, elements);
                }
                var subtoken = IsValue();
                if (subtoken != null)
                {
                    obj.Values.Add(token.Value, ParseValue(subtoken));
                    Tokenizer.Next();
                }
            }            
            else
            {
                Unexpected();
            }
            return obj;
        }
    }
}