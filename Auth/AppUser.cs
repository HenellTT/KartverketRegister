using Microsoft.AspNetCore.Identity;
using System;

namespace KartverketRegister.Auth
{
    public class AppUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Organization { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserType { get; set; }  // maps directly to ENUM('User','Admin')

        public string Email { get; set; }
    }
}
