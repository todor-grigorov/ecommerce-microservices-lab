using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Frontend.Mvc.Dto
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
