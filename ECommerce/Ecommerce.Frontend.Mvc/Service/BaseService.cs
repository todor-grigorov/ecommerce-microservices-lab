using Ecommerce.Frontend.Mvc.Dto;
using Ecommerce.Frontend.Mvc.Service.IService;
using Ecommerce.Frontend.Mvc.Utility;
using Newtonsoft.Json;
using System.Text;
using static Ecommerce.Frontend.Mvc.Utility.StaticDetails;

namespace Ecommerce.Frontend.Mvc.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
        {
            try
            {


                HttpClient client = _httpClientFactory.CreateClient("EcommerceAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                //token

                message.RequestUri = new Uri(requestDto.Url);

                if (requestDto.Data != null)
                {
                    //message.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestDto.Data));
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                    //message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                }

                HttpResponseMessage? apiResponse = null;

                message.Method = requestDto.ApiType switch
                {
                    ApiType.GET => HttpMethod.Get,
                    ApiType.POST => HttpMethod.Post,
                    ApiType.PUT => HttpMethod.Put,
                    ApiType.DELETE => HttpMethod.Delete,
                    _ => HttpMethod.Get
                };

                apiResponse = await client.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.NoContent:
                        return new ResponseDto() { IsSuccess = false, Message = "Not Found" };
                    case System.Net.HttpStatusCode.Forbidden:
                        return new ResponseDto() { IsSuccess = false, Message = "Access Denied" };
                    case System.Net.HttpStatusCode.Unauthorized:
                        return new ResponseDto() { IsSuccess = false, Message = "Unauthorized" };
                    case System.Net.HttpStatusCode.InternalServerError:
                        return new ResponseDto() { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return responseDto;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    IsSuccess = false,
                    Message = ex.Message.ToString(),
                };

                return dto;
            }
        }
    }
}
