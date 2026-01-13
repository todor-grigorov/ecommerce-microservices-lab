using ECommerce.Frontend.Mvc.Dto;
using ECommerce.Frontend.Mvc.Service.IService;
using ECommerce.Frontend.Mvc.Utility;

namespace ECommerce.Frontend.Mvc.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;

        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CreateOrderAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = cartDto,
                Url = StaticDetails.OrderApiBase + "/api/order/CreateOrder"
            });
        }

        public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = stripeRequestDto,
                Url = StaticDetails.OrderApiBase + "/api/order/CreateStripeSession"
            });
        }

        public async Task<ResponseDto?> GetAllOrders(string? userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.OrderApiBase + "/api/order/GetOrders" + userId,
            });
        }

        public async Task<ResponseDto?> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.OrderApiBase + "/api/order/GetOrder",
            });
        }

        public async Task<ResponseDto?> UpdateOrderStatus(int orderId, string status)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = status,
                Url = StaticDetails.OrderApiBase + "/api/order/UpdateOrderStatus" + orderId,
            });
        }

        public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = orderHeaderId,
                Url = StaticDetails.OrderApiBase + "/api/order/ValidateStripeSession"
            });
        }
    }
}
