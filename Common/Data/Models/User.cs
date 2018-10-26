using System;

namespace Nebula.Common.Data.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public DateTime Joined { get; set; }
        public DateTime LastLogin { get; set; }
    }
}