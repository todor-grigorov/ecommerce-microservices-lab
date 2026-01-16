using AutoMapper;
using ECommerce.Services.ProdictAPI.Data;
using ECommerce.Services.ProductAPI.Dto;
using ECommerce.Services.ProductAPI.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private ResponseDto _response;
        private readonly IProductService _productService;

        public ProductController(AppDbContext dbContext, IProductService productService, IMapper mapper)
        {
            _dbContext = dbContext;
            _productService = productService;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            try
            {
                var products = _dbContext.Products.ToList();
                var productsDto = _mapper.Map<List<ProductDto>>(products);
                _response.Result = productsDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var product = _dbContext.Products.First(p => p.ProductId == id);
                var productDto = _mapper.Map<ProductDto>(product);
                _response.Result = productDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }

        [HttpGet]
        [Route("GetProductByCategory/{categoryName}")]
        [Authorize]
        public IActionResult GetProductByCategory(string categoryName)
        {
            try
            {
                var products = _dbContext.Products.Where(p => p.CategoryName == categoryName).ToList();
                var productsDto = _mapper.Map<List<ProductDto>>(products);
                _response.Result = productsDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }

        [HttpGet]
        [Route("GetProductByName/{productName}")]
        [Authorize]
        public IActionResult GetProductByName(string productName)
        {
            try
            {
                var products = _dbContext.Products.Where(p => p.Name.Contains(productName)).ToList();
                var productsDto = _mapper.Map<List<ProductDto>>(products);
                _response.Result = productsDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }

        [HttpGet]
        [Route("search/{productName}/{categoryName}")]
        [Authorize]
        public IActionResult SearchProducts(string productName, string categoryName)
        {
            try
            {
                var products = _dbContext.Products
                    .Where(p => p.Name.Contains(productName) && p.CategoryName.Contains(categoryName))
                    .ToList();
                var productsDto = _mapper.Map<List<ProductDto>>(products);
                _response.Result = productsDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public IActionResult CreateProduct(ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Models.Product>(productDto);

                _dbContext.Products.Add(product);
                _dbContext.SaveChanges();

                if (productDto.Image != null)
                {
                    var baseUrl = $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
                    var (imageUrl, imageLocalPath) = _productService.CreateProductImage(productDto.Image, product.ProductId, baseUrl);

                    product.ImageUrl = imageUrl;
                    product.ImageLocalPath = imageLocalPath;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                }

                _dbContext.Products.Update(product);
                _dbContext.SaveChanges();
                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public IActionResult UpdateProduct(ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Models.Product>(productDto);

                if (productDto.Image != null)
                {
                    if (!string.IsNullOrEmpty(product.ImageLocalPath))
                    {
                        _productService.DeleteProductImage(product.ImageLocalPath);
                    }

                    var baseUrl = $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
                    var (imageUrl, imageLocalPath) = _productService.CreateProductImage(productDto.Image, product.ProductId, baseUrl);

                    product.ImageUrl = imageUrl;
                    product.ImageLocalPath = imageLocalPath;
                }


                _dbContext.Products.Update(product);
                _dbContext.SaveChanges();
                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                var product = _dbContext.Products.First(p => p.ProductId == id);

                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    _productService.DeleteProductImage(product.ImageLocalPath);
                }

                _dbContext.Products.Remove(product);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }
    }
}
