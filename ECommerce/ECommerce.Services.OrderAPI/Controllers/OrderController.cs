using AutoMapper;
using ECommerce.Services.OrderAPI.Data;
using ECommerce.Services.OrderAPI.Dto;
using ECommerce.Services.OrderAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        protected ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _dbContext;
        private readonly IProductService _productService;

        public OrderController(AppDbContext dbContext, IMapper mapper, IProductService productService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _productService = productService;
            _response = new ResponseDto();
        }
    }
}
