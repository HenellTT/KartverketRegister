using Microsoft.AspNetCore.Identity;
using System;

namespace KartverketRegister.Auth
{
    //representerer en bruker i systemet
    public class AppUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Organization { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserType { get; set; }  // maps directly to ENUM('User','Admin', 'Employee')

        public string Email { get; set; }
    }
}