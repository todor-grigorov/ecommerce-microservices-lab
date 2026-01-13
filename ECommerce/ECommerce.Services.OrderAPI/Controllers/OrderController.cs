using AutoMapper;
using ECommerce.Services.OrderAPI.Data;
using ECommerce.Services.OrderAPI.Dto;
using ECommerce.Services.OrderAPI.Models;
using ECommerce.Services.OrderAPI.Service.IService;
using ECommerce.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace ECommerce.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        protected ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IProductService _productService;
        private readonly IStripeService _stripeService;
        private readonly IServiceBus _bus;

        public OrderController(AppDbContext dbContext, IConfiguration configuration, IMapper mapper, IProductService productService, IStripeService stripeService, IServiceBus bus)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _mapper = mapper;
            _productService = productService;
            _response = new ResponseDto();
            _stripeService = stripeService;
            _bus = bus;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public async Task<ResponseDto?> Get(string? userId)
        {
            try
            {
                IEnumerable<OrderHeader> orderHeaders;
                if (User.IsInRole(StaticDetails.RoleAdmin))
                {
                    orderHeaders = _dbContext.OrderHeaders.Include(o => o.OrderDetails).OrderByDescending(o => o.OrderHeaderId);
                }
                else
                {
                    orderHeaders = _dbContext.OrderHeaders.Include(o => o.OrderDetails).Where(o => o.UserId == userId).OrderByDescending(o => o.OrderHeaderId);
                }




                _response.Result = _mapper.Map<IEnumerable<OrderHeaderDto>>(orderHeaders);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetOrder/{orderId}")]
        public async Task<ResponseDto> Get(int orderId)
        {
            try
            {
                OrderHeader? orderHeader = _dbContext.OrderHeaders.Include(o => o.OrderDetails).FirstOrDefault(o => o.OrderHeaderId == orderId);

                if (orderHeader == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Order not found.";
                }

                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(orderHeader);
                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
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

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {
                var stripeSession = _stripeService.CreateStripeSession(stripeRequestDto);
                stripeRequestDto.StripeSessionUrl = stripeSession.Url;

                OrderHeader orderHeader = await _dbContext.OrderHeaders.FindAsync(stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = stripeSession.Id;
                await _dbContext.SaveChangesAsync();

                _response.Result = stripeRequestDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = await _dbContext.OrderHeaders.FindAsync(orderHeaderId);
                var paymentIntent = _stripeService.GetPaymentIntent(orderHeader.StripeSessionId);

                if (paymentIntent.Status.ToLower() == "succeeded")
                {
                    orderHeader.Status = StaticDetails.Status_Approved;
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    await _dbContext.SaveChangesAsync();

                    RewardsDto rewardsDto = new()
                    {
                        UserId = orderHeader.UserId,
                        Email = orderHeader.Email,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        OrderId = orderHeader.OrderHeaderId
                    };

                    var topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await _bus.PublishMessageAsync(rewardsDto, topicName);
                }

                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPut("UpdateOrderStatus/{orderId}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string status)
        {
            try
            {
                OrderHeader orderHeader = _dbContext.OrderHeaders.FirstOrDefault(o => o.OrderHeaderId == orderId);
                if (orderHeader == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Order not found.";
                    return _response;
                }

                if (status == StaticDetails.Status_Cancelled)
                {
                    var options = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderHeader.PaymentIntentId!
                    };

                    var service = new RefundService();
                    Refund refund = service.Create(options);
                }

                orderHeader.Status = status;
                _dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
    }
}
