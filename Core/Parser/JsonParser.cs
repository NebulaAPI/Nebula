using System.Collections.Generic;
using System.Linq;
using SharpPad;

namespace Nebula.Parser
{
    public class JsonObject : NamedNode
    {
        public List<JsonObject> Children { get; set; }

        public dynamic Value { get; set; }

        public bool IsObject { get; set; }
        
        public bool IsArray { get; set; }
        
        public bool IsValue { get; set; }
        
        public JsonObject() : base("jobj")
        {
            Children = new List<JsonObject>();
            IsArray = false;
            IsObject = false;
            IsValue = false;
        }
    }

    public class JsonParser : Parser
    {
        public JsonParser(Tokenizer tokenizer) : base(tokenizer)
        {
            
        }

        public JsonObject Parse()
        {
            while (!Tokenizer.Eof())
            {
                if (IsObject())
                {
                    return ParseRootObject();
                }
            }
            return null;
        }

        private JsonObject ParseRootObject()
        {
            var obj = new JsonObject() { IsObject = true };

            if (IsPunc('{'))
            {
                obj.Children.AddRange(Delimited('{', '}', ',', ParseObject));
            }
            
            if (IsPunc('['))
            {
                obj.Children.AddRange(Delimited('[', ']', ',', ParseArray));
            }
            obj.Dump();

            return obj;
        }

        private JsonObject ParseObject()
        {
            // this represents the inside of an object.
            // "<some text>": "some value"
            // "<some text>": { other object }
            // "<some text>": [ an array ]
            var obj = new JsonObject();
            var token = IsString(null);
            if (token == null)
            {
                Unexpected();
            }
            obj.Name = token.Value;
            obj.IsObject = true;
            Tokenizer.Next();
            SkipPunc(':');

            // here we check what the next thing is. If its a value, set the value of this object
            // if its an object, add it as a child and run parse object again
            // if its an array, add it as a child and run parse array
            var nextToken = IsValue();
            if (nextToken != null)
            {
                obj.Value = ParseValue(nextToken);
                obj.IsObject = false;
                obj.IsValue = true;
            }

            if (IsObject())
            {
                obj.Children.AddRange(Delimited<JsonObject>('{', '}', ',', ParseObject));
            }

            if (IsArray())
            {
                obj.Children.AddRange(Delimited<JsonObject>('[', ']', ',', ParseArray));
            }
            
            return obj;
        }

        private JsonObject ParseArray()
        {
            var obj = new JsonObject();
            obj.IsArray = true;
            
            if (IsArray())
            {
                obj.Children.AddRange(Delimited<JsonObject>('[', ']', ',', ParseArray));
            }

            if (IsObject())
            {
                obj.Children.AddRange(Delimited<JsonObject>('{', '}', ',', ParseObject));
            }
            
            var token = IsValue();
            if (token != null)
            {
                var newObject = new JsonObject();
                newObject.IsValue = true;
                newObject.Value = ParseValue(token);
                obj.Children.Add(newObject);
            }
            return obj;
        }

        private dynamic ParseValue(Token valueToken)
        {
            if (IsOp("-") != null)
            {
                Tokenizer.Next();
                var nextToken = IsValue();
                if (nextToken != null)
                {
                    Tokenizer.Next();
                    return "-" + nextToken.Value;
                }

                Unexpected();
            }
            Tokenizer.Next();
            return valueToken.Value;
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
            return IsPunc('{');
        }

        private bool IsArray()
        {
            return IsPunc('[');
        }
    }
}