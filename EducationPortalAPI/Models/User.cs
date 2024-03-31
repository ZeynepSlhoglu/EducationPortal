using Microsoft.AspNetCore.Identity;

namespace EducationPortalAPI.Models
{
    public class User : IdentityUser
    {
        public bool? InstructorStatus { get; set; }
        public string? InstructorInfo { get; set; }
    }
}
