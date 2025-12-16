using ECommerce.Services.OrderAPI.Dto;
using Stripe.Checkout;

namespace ECommerce.Services.OrderAPI.Service.IService
{
    public interface IStripeService
    {
        Session? CreateStripeSession(StripeRequestDto stripeRequestDto);
    }
}
