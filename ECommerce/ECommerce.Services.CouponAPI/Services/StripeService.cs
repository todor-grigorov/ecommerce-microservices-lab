using ECommerce.Services.CouponAPI.Services.IService;
using ECommerce.Services.CouponAPI.Utility;
using Stripe;

namespace ECommerce.Services.CouponAPI.Services
{
    public class StripeService : IStripeService
    {
        public async Task<Coupon?> CreateCoupon(long amountOff, string name, string currency, string id)
        {
            try
            {
                StripeConfiguration.ApiKey = StaticDetails.StripeSecretKey;

                var options = new CouponCreateOptions
                {
                    AmountOff = amountOff,
                    Name = name,
                    Currency = currency,
                    Id = id
                };
                var service = new CouponService();
                Coupon coupon = service.Create(options);

                return coupon;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task<bool> DeleteCoupon(string id)
        {
            var service = new CouponService();
            var isDeleted = service.Delete(id).Deleted;

            if (isDeleted != null && isDeleted == true)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
