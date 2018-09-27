using System.Collections.Generic;
using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Interfaces;
using Nebula.Parser;
using System.Linq;
using System;
using Nebula.Models;

namespace Nebula.Compiler.Objects.Csharp
{
    public abstract class CsharpClass<T> : AbstractClass<T> where T : MainObjectNode
    {
        public CsharpClass(AbstractNamespace ns, T root, CsharpCompiler compiler) 
            : base(ns, root, compiler)
        {
            
        }

        public CsharpClass()
            : base()
        {

        }

        protected GenericClass BuildAuthenticator()
        {
            var props = new List<GenericProperty>();
            var body = new List<string>();

            Action<string> prepareAuthorizationBody = (authMethod) => {
                props.Add(new GenericProperty {
                        Name = "AccessToken", 
                        DataTypeString = "string"
                    });
                body.AddRange(new [] {
                    $"request.AddHeader(\"Authorization\", $\"{authMethod} {{AccessToken}}\"" 
                });
            };

            switch (Config.AuthMethod)
            {
                case AuthenticationMethod.BasicHttp:
                case AuthenticationMethod.NoAuthentication:
                    return null;
                case AuthenticationMethod.CustomHeader:
                    props.Add(new GenericProperty {
                        Name = "CustomHeader",
                        DataTypeString = "string"
                    });
                    body.AddRange(new [] {
                        $"request.AddHeader(\"{Config.CustomHeaderKey}\", $\"{{CustomHeader}}\"" 
                    });
                    break;
                case AuthenticationMethod.JwtBearer:
                    prepareAuthorizationBody("bearer");
                    break;
                case AuthenticationMethod.OAuthToken:
                    prepareAuthorizationBody("token");
                    break;
            }

            var funcs = new List<GenericFunction>
            {
                new GenericFunction
                {
                    Name = "Authenticate",
                    ReturnType = "void",
                    Arguments = new List<GenericVariableDefinition>
                    {
                        new GenericVariableDefinition("client", "IRestClient"),
                        new GenericVariableDefinition("request", "IRestRequest")
                    },
                    Body = body
                }
            };
            
            return new GenericClass
            {
                AccessModifier = Visibility.Private,
                Name = "Authenticator",
                Inheritence = new List<GenericClass> {
                    new GenericClass { Name = "IAuthenticator"}
                },
                Properties = props,
                Functions = funcs
            };
        }

        protected GenericConstructor BuildConstructor()
        {
            var args = new List<GenericVariableDefinition>();
            switch (Config.AuthMethod)
            {
                case AuthenticationMethod.BasicHttp:
                    args.Add(new GenericVariableDefinition("username", "string"));
                    args.Add(new GenericVariableDefinition("password", "string"));
                    break;
                case AuthenticationMethod.CustomHeader:
                    args.Add(new GenericVariableDefinition("authValue", "string"));
                    break;
                case AuthenticationMethod.JwtBearer:
                case AuthenticationMethod.OAuthToken:
                    args.Add(new GenericVariableDefinition("token", "string"));
                    break;
            }
            
            return new GenericConstructor
            {
                Arguments = args,
                Body = new List<string> {

                }
            };
        }
    }
}