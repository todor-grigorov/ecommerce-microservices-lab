using ECommerce.Services.IdentityAPI.Models;

namespace ECommerce.Services.IdentityAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser user);
    }
}
