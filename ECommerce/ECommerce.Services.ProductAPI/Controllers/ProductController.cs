using AutoMapper;
using ECommerce.Services.ProdictAPI.Data;
using ECommerce.Services.ProductAPI.Dto;
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

        public ProductController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet]
        [Authorize]
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
        [Authorize]
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
        public IActionResult CreateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Models.Product>(productDto);

                _dbContext.Products.Add(product);
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
        public IActionResult UpdateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Models.Product>(productDto);
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
