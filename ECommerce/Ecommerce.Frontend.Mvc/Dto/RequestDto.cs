using static ECommerce.Frontend.Mvc.Utility.StaticDetails;

namespace ECommerce.Frontend.Mvc.Dto
{
    public record RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object? Data { get; set; }
        public string AccessToken { get; set; }
    }
}
