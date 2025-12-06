namespace ECommerce.Services.IdentityAPI.Dto
{
    public class RegistrationRequestDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
