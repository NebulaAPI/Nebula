using System;
using Nebula.SDK.Objects.Shared;

namespace Nebula.SDK.Objects.Server
{
    public class UserApiKey
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public DateTime Generated { get; set; }
        public bool Active { get; set; }
    }
}