using Microsoft.AspNetCore.Identity;

namespace OnlineMobileServices.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        
    }
}