using Microsoft.AspNetCore.Identity;

namespace EducationPortalAPI.Models
{
    public class User : IdentityUser
    {
        public int ID { get; set; }
        public string? Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
