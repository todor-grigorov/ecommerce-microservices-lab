using ECommerce.Frontend.Mvc.Dto;
using ECommerce.Frontend.Mvc.Service.IService;
using ECommerceFrontend.Mvc.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace ECommerce.Frontend.Mvc.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;

            var response = await _orderService.CreateOrderAsync(cart);
            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result))!;

            if (response != null && response.IsSuccess && response.Result != null)
            {
                // Get Stripe session and redirect to stripe to place order

                //await _cartService.ClearCartAsync(cart.CartHeader.UserId);
                //return RedirectToAction("OrderConfirmation", "Order", new { orderId = Convert.ToInt32(response.Result) });
            }

            return View(cart);
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            CartDto cartDto = new();

            if (response != null && response.IsSuccess && response.Result != null)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var response = await _cartService.ApplyCouponAsync(cartDto);

            if (response != null && response.IsSuccess && response.Result != null)
            {
                TempData["success"] = "Coupon applied successfully";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = "";
            var response = await _cartService.RemoveCouponAsync(cartDto);

            if (response != null && response.IsSuccess && response.Result != null)
            {
                TempData["success"] = "Coupon applied successfully";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart()
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;


            var response = await _cartService.EmailCartAsync(cart);

            if (response != null && response.IsSuccess && response.Result != null)
            {
                TempData["success"] = "Email will be processed and sent shortly.";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var response = await _cartService.GetCartByUserIdAsync(userId);
            CartDto cartDto = new();

            if (response != null && response.IsSuccess && response.Result != null)
            {
                cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result))!;
            }

            return cartDto;
        }
    }
}
