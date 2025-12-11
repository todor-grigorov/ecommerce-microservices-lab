using ECommerce.Frontend.Mvc.Dto;
using ECommerce.Frontend.Mvc.Service.IService;
using ECommerce.Frontend.Mvc.Utility;

namespace ECommerce.Frontend.Mvc.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> AssignRoleAsync(AssignRoleRequestDto assignRoleRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = assignRoleRequestDto,
                Url = StaticDetails.IdentityApiBase + "/api/auth/assign-role"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = loginRequestDto,
                Url = StaticDetails.IdentityApiBase + "/api/auth/login"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = registrationRequestDto,
                Url = StaticDetails.IdentityApiBase + "/api/auth/register"
            }, withBearer: false);
        }
    }
}
