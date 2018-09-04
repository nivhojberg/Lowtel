using Microsoft.AspNetCore.Identity;

namespace Lowtel.Controllers

{
    internal class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

    }
}