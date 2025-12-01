namespace Ecommerce.Frontend.Mvc.Dto
{
    public record RequestDto
    {
        public string ApiType { get; set; } = "GET";
        public string Url { get; set; }
        public object? Data { get; set; }
        public string AccessToken { get; set; }
    }
}
