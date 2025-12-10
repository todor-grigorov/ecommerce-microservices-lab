using ECommerce.Services.ShoppingCartAPI.Dto;

namespace ECommerce.Services.ShoppingCartAPI.Service.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCoupon(string couponCode);
    }
}
