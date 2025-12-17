using Stripe;

namespace ECommerce.Services.CouponAPI.Services.IService
{
    public interface IStripeService
    {
        Task<Coupon?> CreateCoupon(long amountOff, string name, string currency, string id);
        Task<bool> DeleteCoupon(string id);
    }
}
