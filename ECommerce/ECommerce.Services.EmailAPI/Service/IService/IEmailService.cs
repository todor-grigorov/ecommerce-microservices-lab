using ECommerce.Services.EmailAPI.Dto;

namespace ECommerce.Services.EmailAPI.Service.IService
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
    }
}
