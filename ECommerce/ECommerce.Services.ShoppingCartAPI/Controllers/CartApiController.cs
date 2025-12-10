using AutoMapper;
using ECommerce.Services.ShoppingCartAPI.Data;
using ECommerce.Services.ShoppingCartAPI.Dto;
using ECommerce.Services.ShoppingCartAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartApiController : ControllerBase
    {
        private ResponseDto _responseDto;
        private IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public CartApiController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            this._responseDto = new ResponseDto();
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                // Map CartDto to CartHeader and CartDetails entities
                var cartHeader = _mapper.Map<CartHeader>(cartDto);
                var cartDetailsList = _mapper.Map<List<CartDetails>>(cartDto.CartDetails);
                // Check if CartHeader exists
                var existingCartHeader = _dbContext.CartHeaders.AsNoTracking()
                    .FirstOrDefault(ch => ch.UserId == cartHeader.UserId);
                if (existingCartHeader == null)
                {
                    // Create new CartHeader
                    _dbContext.CartHeaders.Add(cartHeader);
                    await _dbContext.SaveChangesAsync();
                    // Add CartDetails
                    foreach (var cartDetails in cartDetailsList)
                    {
                        cartDetails.CartHeaderId = cartHeader.CartHeaderId;
                        _dbContext.CartDetails.Add(cartDetails);
                    }
                }
                else
                {
                    // Update existing CartHeader
                    existingCartHeader.CouponCode = cartHeader.CouponCode;
                    // Add or update CartDetails
                    foreach (var cartDetails in cartDetailsList)
                    {
                        var existingCartDetails = _dbContext.CartDetails.AsNoTracking()
                            .FirstOrDefault(cd => cd.CartHeaderId == existingCartHeader.CartHeaderId && cd.ProductId == cartDetails.ProductId);
                        if (existingCartDetails == null)
                        {
                            // New CartDetails
                            cartDetails.CartHeaderId = existingCartHeader.CartHeaderId;
                            _dbContext.CartDetails.Add(cartDetails);
                        }
                        else
                        {
                            // Update existing CartDetails
                            existingCartDetails.Count += cartDetails.Count;
                        }
                    }
                }
                await _dbContext.SaveChangesAsync();
                _responseDto.Result = _mapper.Map<CartDto>(cartHeader);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails existingCartDetails = _dbContext.CartDetails.First(c => c.CartDetailsId == cartDetailsId);

                int totalCountOfCartItems = _dbContext.CartDetails
                    .Where(c => c.CartHeaderId == existingCartDetails.CartHeaderId)
                    .Count();

                _dbContext.CartDetails.Remove(existingCartDetails);

                if (totalCountOfCartItems == 1)
                {
                    // Remove CartHeader if this is the last item
                    CartHeader cartHeaderToRemove = _dbContext.CartHeaders.First(c => c.CartHeaderId == existingCartDetails.CartHeaderId);
                    _dbContext.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _dbContext.SaveChangesAsync();
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }

            return _responseDto;

        }

    }
}
