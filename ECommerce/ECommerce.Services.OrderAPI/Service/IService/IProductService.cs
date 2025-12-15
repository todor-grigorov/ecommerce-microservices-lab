using ECommerce.Services.OrderAPI.Dto;

namespace ECommerce.Services.OrderAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
