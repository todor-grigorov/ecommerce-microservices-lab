using ECommerce.Services.ProductAPI.Services.IService;

namespace ECommerce.Services.ProductAPI.Services
{
    public class ProductService : IProductService
    {
        public (string ImageUrl, string ImageLocalPath) CreateProductImage(IFormFile productImageFile, int productId, string baseUrl)
        {
            string fileName = productId + Path.GetExtension(productImageFile.FileName);
            string filePath = @"wwwroot\ProductImages\" + fileName;
            var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);

            using (var stream = new FileStream(filePathDirectory, FileMode.Create))
            {
                productImageFile.CopyTo(stream);
            }

            var imageUrl = baseUrl + "/ProductImages/" + fileName;
            var imageLocalPath = filePath;

            return (imageUrl, imageLocalPath);
        }

        public void DeleteProductImage(string productImageLocalPath)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), productImageLocalPath);
            FileInfo file = new FileInfo(filePath);

            if (file.Exists)
            {
                file.Delete();
            }
        }
    }
}
