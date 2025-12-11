using ECommerce.Frontend.Mvc.Dto;

namespace ECommerce.Frontend.Mvc.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true);
    }
}
