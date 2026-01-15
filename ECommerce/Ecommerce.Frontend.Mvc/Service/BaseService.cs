using ECommerce.Frontend.Mvc.Dto;
using ECommerce.Frontend.Mvc.Service.IService;
using Newtonsoft.Json;
using System.Text;
using static ECommerce.Frontend.Mvc.Utility.StaticDetails;

namespace ECommerce.Frontend.Mvc.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("ECommerceAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                if (requestDto.ContentType == ContentType.MultipartFormData)
                {
                    message.Headers.Add("Accept", "*/*");
                }
                else
                {
                    message.Headers.Add("Accept", "application/json");
                }

                //token
                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", "Bearer " + token);
                }

                message.RequestUri = new Uri(requestDto.Url);

                if (requestDto.ContentType == ContentType.MultipartFormData)
                {
                    var content = new MultipartFormDataContent();

                    foreach (var prop in requestDto.Data.GetType().GetProperties())
                    {
                        var value = prop.GetValue(requestDto.Data);
                        if (value is FormFile)
                        {
                            var file = value as FormFile;
                            if (file != null)
                            {
                                var fileContent = new StreamContent(file.OpenReadStream());
                                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                                content.Add(fileContent, prop.Name, file.FileName);
                            }
                        }
                    }
                }
                else
                {
                    if (requestDto.Data != null)
                    {
                        message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                    }
                }


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
