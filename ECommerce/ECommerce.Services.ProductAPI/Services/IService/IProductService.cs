namespace ECommerce.Services.ProductAPI.Services.IService
{
    public interface IProductService
    {
        (string ImageUrl, string ImageLocalPath) CreateProductImage(IFormFile productImageFile, int productId, string baseUrl);
        void DeleteProductImage(string productImageLocalPath);
    }
}
