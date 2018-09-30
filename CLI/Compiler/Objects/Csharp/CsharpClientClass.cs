using System;
using System.Collections.Generic;
using System.Linq;
using Nebula.Compiler.Abstracts;
using Nebula.Models;
using Nebula.Parser;

namespace Nebula.Compiler.Objects.Csharp
{
    public class CsharpClientClass : CsharpClass<ApiNode>
    {
        public CsharpClientClass() 
        {
            
        }

        public override void Init()
        {
            Config = Compiler.ApiConfig[RootNode];

            Functions.AddRange(RootNode.SearchByType<FunctionNode>().Select(f => new AbstractFunction(f)));

            Properties.Add(new GenericProperty("Client", "RestClient"));

            Constructor = BuildConstructor();

            var authenticator = BuildAuthenticator();
            if (authenticator != null)
            {
                TopOfClassExtra.Add(authenticator);
            }
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
            var body = new List<string>();
            body.Add($"Client = new RestClient(\"{Config.Host}\");");
            switch (Config.AuthMethod)
            {
                case AuthenticationMethod.BasicHttp:
                    args.Add(new GenericVariableDefinition("username", "string"));
                    args.Add(new GenericVariableDefinition("password", "string"));
                    body.Add("Client.Authenticator = new HttpBasicAuthenticator(username, password);");
                    break;
                case AuthenticationMethod.CustomHeader:
                    args.Add(new GenericVariableDefinition("authValue", "string"));
                    body.Add("Client.Authenticator = new Authenticator { CustomHeader = authenticationValue };");
                    break;
                case AuthenticationMethod.JwtBearer:
                case AuthenticationMethod.OAuthToken:
                    args.Add(new GenericVariableDefinition("token", "string"));
                    body.Add("Client.Authenticator = new Authenticator { AccessToken = token };");
                    break;
            }
            
            return new GenericConstructor
            {
                Arguments = args,
                Name = $"{RootNode.Name}Client",
                Body = body
            };
        }
    }
}