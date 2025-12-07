using Ecommerce.Frontend.Mvc.Dto;

namespace Ecommerce.Frontend.Mvc.Service.IService
{
    public interface IBaseService
    {
       Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true);
    }
}
