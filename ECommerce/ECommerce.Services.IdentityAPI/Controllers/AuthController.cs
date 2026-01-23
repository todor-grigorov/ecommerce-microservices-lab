using ECommerce.Services.CouponAPI.Dto;
using ECommerce.Services.IdentityAPI.Dto;
using ECommerce.Services.IdentityAPI.RabbitMQSender;
using ECommerce.Services.IdentityAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Services.IdentityAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        //private readonly IServiceBus _serviceBus;
        private readonly IRabbitMQAuthMessageSender _messageBus;
        private readonly string? _registerUserQueue;
        protected ResponseDto _response;

        public AuthController(IConfiguration configuration, IAuthService authService, IRabbitMQAuthMessageSender messageBus)
        {
            _configuration = configuration;
            _authService = authService;
            _messageBus = messageBus;
            _response = new ResponseDto();
            _registerUserQueue = configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto dto)
        {
            if (string.IsNullOrEmpty(_registerUserQueue))
            {
                _response.IsSuccess = false;
                _response.Message = "Service Bus queue/topic name is not configured.";
                return BadRequest(_response);
            }

            try
            {
                var errorMessages = await _authService.Register(dto);

                if (!string.IsNullOrEmpty(errorMessages))
                {
                    _response.IsSuccess = false;
                    _response.Message = errorMessages;
                    return BadRequest(_response);
                }

                _response.Message = "User registered successfully.";
                await _messageBus.SendMessageAsync(dto.Email, _registerUserQueue);

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                return BadRequest(_response);
            }

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

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequestDto dto)
        {
            var assignRoleSuccessful = await _authService.AssignRole(dto.Email, dto.Role.ToUpper());

            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encountered";
                return BadRequest(_response);
            }

            return Ok(_response);
        }
    }
}
