using Ecommerce.Frontend.Mvc.Dto;
using Ecommerce.Frontend.Mvc.Service.IService;
using Ecommerce.Frontend.Mvc.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web.Providers.Entities;

namespace Ecommerce.Frontend.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto();

            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            ResponseDto responseDto = await _authService.LoginAsync(obj);

            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));

                await SignInUser(loginResponseDto);
                _tokenProvider.SetToken(loginResponseDto.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", responseDto.Message);

                return View(obj);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem { Text = StaticDetails.RoleAdmin, Value = StaticDetails.RoleAdmin },
                new SelectListItem { Text = StaticDetails.RoleCustomer, Value = StaticDetails.RoleCustomer },
            };

            ViewBag.RoleList = roleList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto obj)
        {
            ResponseDto result = await _authService.RegisterAsync(obj);
            ResponseDto assignRole;

            if (result != null && result.IsSuccess)
            {
                if (obj.Role == StaticDetails.RoleAdmin)
                {
                    assignRole = await _authService.AssignRoleAsync(new AssignRoleRequestDto
                    {
                        Email = obj.Email,
                        Role = StaticDetails.RoleAdmin
                    });
                }
                else
                {
                    assignRole = await _authService.AssignRoleAsync(new AssignRoleRequestDto
                    {
                        Email = obj.Email,
                        Role = StaticDetails.RoleCustomer
                    });
                }

                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration Successful";

                    return RedirectToAction(nameof(Login));
                }
            }

            var roleList = new List<SelectListItem>()
            {
                new SelectListItem { Text = StaticDetails.RoleAdmin, Value = StaticDetails.RoleAdmin },
                new SelectListItem { Text = StaticDetails.RoleCustomer, Value = StaticDetails.RoleCustomer },
            };

            ViewBag.RoleList = roleList;

            return View(obj);
        }

        public IActionResult Logout()
        {
            return View();
        }

        private async Task SignInUser(LoginResponseDto loginRequestDto)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(loginRequestDto.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));



            var  principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
