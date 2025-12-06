using Microsoft.AspNetCore.Identity;

namespace ECommerce.Services.IdentityAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
