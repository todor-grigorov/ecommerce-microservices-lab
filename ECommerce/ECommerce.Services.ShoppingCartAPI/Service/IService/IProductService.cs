using ECommerce.Services.ShoppingCartAPI.Dto;

namespace ECommerce.Services.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
