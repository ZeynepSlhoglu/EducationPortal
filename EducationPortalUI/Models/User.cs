using Microsoft.AspNetCore.Identity;

namespace EducationPortalUI.Models

{
    public class User 
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool? InstructorStatus { get; set; }
        public string? InstructorInfo { get; set; }
    }
}
