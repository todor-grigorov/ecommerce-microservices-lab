using Ecommerce.Frontend.Mvc.Dto;
using Ecommerce.Frontend.Mvc.Service.IService;
using Ecommerce.Frontend.Mvc.Utility;

namespace Ecommerce.Frontend.Mvc.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;

        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = productDto,
                Url = StaticDetails.ProductApiBase + "/api/product"
            });
        }

        public async Task<ResponseDto?> DeleteProductAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.DELETE,
                Url = StaticDetails.ProductApiBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDto?> GetAllProductsAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.ProductApiBase + "/api/product"
            });
        }

        public async Task<ResponseDto?> GetProductByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.ProductApiBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Data = productDto,
                Url = StaticDetails.ProductApiBase + "/api/product"
            });
        }
    }
}
