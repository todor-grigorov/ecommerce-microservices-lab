using ECommerce.Services.CouponAPI.Dto;
using ECommerce.Services.IdentityAPI.Dto;
using ECommerce.Services.IdentityAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Services.IdentityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _response;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
            _response = new ResponseDto();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto dto)
        {
            var errorMessages = await _authService.Register(dto);

            if (!string.IsNullOrEmpty(errorMessages))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessages;
                return BadRequest(_response);
            }
            else
            {
                _response.Message = "User registered successfully.";
            }

            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var loginResponse = await _authService.Login(dto);

            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect.";
                return BadRequest(_response);
            }

            _response.Result = loginResponse;

            return Ok(_response);
        }

    }
}
