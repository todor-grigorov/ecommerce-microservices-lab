using Ecommerce.Frontend.Mvc.Dto;

namespace Ecommerce.Frontend.Mvc.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto?> AssignRoleAsync(AssignRoleRequestDto assignRoleRequestDto);
    }
}
