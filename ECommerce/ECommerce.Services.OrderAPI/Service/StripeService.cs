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
            Session session;
            try
            {
                StripeConfiguration.ApiKey = StaticDetails.StripeSecretKey;

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
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


                var service = new SessionService();
                session = service.Create(options);
            }
            catch (Exception)
            {
                throw;
            }

            return session;
        }
    }
}
