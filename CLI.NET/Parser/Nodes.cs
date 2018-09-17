using System.Collections.Generic;

namespace CLI.NET.Parser
{
    public class AstNode
    {
        public string Type { get; set; }

        public bool Exported { get; set; }

        public AstNode(string type)
        {
            Type = type;
            Exported = false;
        }
    }

    public class NumberNode : AstNode
    {
        public int Value { get; set; }
        public NumberNode(int value) : base("num")
        {
            Value = value;
        }
    }

    public class StringNode : AstNode
    {
        public string Value { get; set; }
        public StringNode(string value) : base("str")
        {
            Value = value;
        }
    }

    public class BooleanNode : AstNode
    {
        public bool Value { get; set; }
        public BooleanNode(bool value) : base("bool")
        {
            Value = value;
        }
    }

    public class VarNode : AstNode
    {
        public object Value { get; set; }
        public VarNode(object value) : base("var")
        {
            Value = value;
        }
    }

    public class LambdaNode<T> : AstNode where T : AstNode
    {
        public List<T> Variables { get; set; }
        public AstNode Body { get; set; }
        public string Name { get; set; }
        public LambdaNode(List<T> vars, AstNode body, string name) : base("lambda")
        {
            Variables = vars;
            Body = body;
            Name = name;
        }
    }

    public class CallNode<T> : AstNode where T : AstNode
    {
        public AstNode Function { get; set; }
        public List<T> Arguments { get; set; }
        public CallNode(AstNode func, List<T> args) : base("call")
        {
            Function = func;
            Arguments = args;
        }
    }

    public class IfNode : AstNode
    {
        public AstNode Condition { get; set; }
        public AstNode Then { get; set; }
        public AstNode Otherwise { get; set; }
        public IfNode(AstNode condition, AstNode then, AstNode otherwise) : base("if")
        {
            Condition = condition;
            Then = then;
            Otherwise = otherwise;
        }
    }

    public class AssignNode : AstNode
    {
        public string Op { get; set; }
        public AstNode Left { get; set; }
        public AstNode Right { get; set; }
        public AssignNode(string op, AstNode left, AstNode right) : base("assign")
        {
            Op = op;
            Left = left;
            Right = right;
        }
    }

    public class ApiNode : AstNode
    {
        public string Name { get; set; }
        public List<AstNode> Api { get; set; }
        public ApiNode(string name, List<AstNode> api) : base("api")
        {
            Name = name;
            Api = api;
        }
    }

    public class UseNode : AstNode
    {
        public string Path { get; set; }
        public AstNode Module { get; set; }
        public UseNode(string path, AstNode module) : base("use")
        {
            Path = path;
            Module = module;
        }
    }

    public class ModuleNode : AstNode
    {
        public string Name { get; set; }
        public List<AstNode> Elements { get; set; }
        public ModuleNode(string name, List<AstNode> elements) : base("module")
        {
            Name = name;
            Elements = elements;
        }
    }

    public class ArgumentNode : AstNode
    {
        public string Name { get; set; }
        public DataTypeNode ArgType { get; set; }
        public ArgumentNode(string name, DataTypeNode type) : base("argument")
        {
            Name = name;
            ArgType = type;
        }
    }

    public class FunctionNode : AstNode
    {
        public string Name { get; set; }
        public List<ArgumentNode> Args { get; set; }
        public TokenType Method { get; set; }
        public string Url { get; set; }
        public DataTypeNode ReturnType { get; set; }
        public FunctionNode(string name, List<ArgumentNode> args, TokenType method, string url, DataTypeNode returnType) : base("function")
        {
            Name = name;
            Args = args;
            Method = method;
            Url = url;
            ReturnType = returnType;
        }
    }

    public class FunctionsNode : AstNode
    {
        public List<FunctionNode> Functions { get; set; }
        public FunctionsNode(List<FunctionNode> functions) : base("functions")
        {
            Functions = functions;
        }
    }

    public class EntityNode : AstNode
    {
        public string Name { get; set; }
        public List<ArgumentNode> Fields { get; set; }
        public EntityNode(string name, List<ArgumentNode> fields) : base("entity")
        {
            Name = name;
            Fields = fields;
        }
    }

    public class KeyValueNode : AstNode
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public KeyValueNode(string key, string value) : base("kv")
        {
            Key = key;
            Value = value;
        }
    }

    public class ConfigNode : AstNode
    {
        public List<KeyValueNode> Nodes { get; set; }
        public ConfigNode(List<KeyValueNode> nodes) : base("config")
        {
            Nodes = nodes;
        }
    }

    public class ProjectNode : AstNode
    {
        public List<ModuleNode> Modules { get; set; }
        public ProjectNode(List<ModuleNode> modules) : base("project")
        {
            Modules = modules;
        }
    }

    public class DataTypeNode : AstNode
    {
        public string Name { get; set; }
        public bool Generic { get; set; }
        public string GenericType { get; set; }
        public DataTypeNode(string name, bool generic, string genericType) : base("dt")
        {
            Name = name;
            Generic = generic;
            GenericType = genericType;
        }
    }
}