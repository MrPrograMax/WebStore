using Microsoft.AspNetCore.Identity;

namespace MyPracticWebStore_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }     
    }
}
