using Microsoft.AspNetCore.Identity;

namespace MyPracticWebStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }     
    }
}
