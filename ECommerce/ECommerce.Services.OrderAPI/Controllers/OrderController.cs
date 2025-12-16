using AutoMapper;
using ECommerce.Services.OrderAPI.Data;
using ECommerce.Services.OrderAPI.Dto;
using ECommerce.Services.OrderAPI.Models;
using ECommerce.Services.OrderAPI.Service.IService;
using ECommerce.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
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


        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now.ToUniversalTime();
                orderHeaderDto.Status = StaticDetails.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated = _dbContext.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _dbContext.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                _response.Result = orderHeaderDto;
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
