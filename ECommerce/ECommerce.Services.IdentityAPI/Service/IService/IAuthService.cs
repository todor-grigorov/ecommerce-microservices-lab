using ECommerce.Services.IdentityAPI.Dto;

namespace ECommerce.Services.IdentityAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginRequestDto> Login(LoginRequestDto loginRequestDto);
    }
}
