namespace ECommerce.Services.RewardsApi.Dto
{
    public record RewardsMessageDto
    {
        public string UserId { get; set; }
        public int RewardsActivity { get; set; }
        public int OrderId { get; set; }
    }
}
