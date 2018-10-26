namespace Nebula.SDK.Objects
{
    public enum AuthenticationMethod
    {
        OAuthToken,
        JwtBearer,
        BasicHttp,
        CustomHeader,
        NoAuthentication
    }

    public class ApiConfig
    {
        public string Host { get; set; }
        public string Prefix { get; set; }
        public AuthenticationMethod AuthMethod { get; set; }
        public string CustomHeaderKey { get; set; }
    }
}