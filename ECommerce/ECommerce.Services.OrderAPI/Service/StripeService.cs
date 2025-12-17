using ECommerce.Services.OrderAPI.Dto;
using ECommerce.Services.OrderAPI.Service.IService;
using ECommerce.Services.OrderAPI.Utility;
using Stripe;
using Stripe.Checkout;

namespace ECommerce.Services.OrderAPI.Service
{
    public class StripeService : IStripeService
    {
        public Session? CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            try
            {
                StripeConfiguration.ApiKey = StaticDetails.StripeSecretKey;

                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                var discounts = new List<SessionDiscountOptions>() {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDto.OrderHeader.CouponCode
                    }
                };


                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var lineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.99 -> 2099 cents
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName,
                            },
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(lineItem);
                }

                if (stripeRequestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = discounts;
                }

                var service = new SessionService();
                Session session = service.Create(options);

                return session;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PaymentIntent? GetPaymentIntent(string sessionId)
        {
            try
            {
                StripeConfiguration.ApiKey = StaticDetails.StripeSecretKey;
                var service = new SessionService();
                Session session = service.Get(sessionId);

                PaymentIntent paymentIntent = new PaymentIntentService().Get(session.PaymentIntentId);

                return paymentIntent;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
