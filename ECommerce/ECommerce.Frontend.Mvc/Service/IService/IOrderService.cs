using ECommerce.Frontend.Mvc.Dto;

namespace ECommerce.Frontend.Mvc.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrderAsync(CartDto cartDto);
        //Task<ResponseDto?> GetCouponByIdAsync(int id);
        //Task<ResponseDto?> GetCouponAsync(string couponCode);
        //Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto);
        //Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto);
        //Task<ResponseDto?> DeleteCouponAsync(int id);
    }
}
