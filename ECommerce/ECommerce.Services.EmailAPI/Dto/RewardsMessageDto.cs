namespace ECommerce.Services.EmailAPI.Dto
{
    public record RewardsMessageDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public int RewardsActivity { get; set; }
        public int OrderId { get; set; }
    }
}
