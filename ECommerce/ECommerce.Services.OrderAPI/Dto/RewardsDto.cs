namespace ECommerce.Services.OrderAPI.Dto
{
    public record RewardsDto
    {
        public string UserId { get; set; }
        public int RewardsActivity { get; set; }
        public int OrderId { get; set; }
    };
}
