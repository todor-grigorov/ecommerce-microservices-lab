using ECommerce.Frontend.Mvc.Dto;

namespace ECommerce.Frontend.Mvc.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto> GetCartByUserIdAsync(string userId);
        Task<ResponseDto> UpsertCartAsync(CartDto cartDto);
        Task<ResponseDto> RemoveFromCartAsync(int cartDetailsId);
        Task<ResponseDto> ApplyCouponAsync(CartDto cartDto);
        //Task<ResponseDto> RemoveCouponAsync(CartDto cartDto);
    }
}
