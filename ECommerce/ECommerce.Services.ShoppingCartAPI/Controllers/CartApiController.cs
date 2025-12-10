using AutoMapper;
using ECommerce.Services.ShoppingCartAPI.Data;
using ECommerce.Services.ShoppingCartAPI.Dto;
using ECommerce.Services.ShoppingCartAPI.Models;
using ECommerce.Services.ShoppingCartAPI.Service.IService;
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
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;

        public CartApiController(AppDbContext dbContext, IProductService productService, ICouponService couponService, IMapper mapper)
        {
            _dbContext = dbContext;
            _productService = productService;
            _couponService = couponService;
            _mapper = mapper;
            _responseDto = new ResponseDto();
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userIdcartDetailsId)
        {
            try
            {
                var cartHeader = await _dbContext.CartHeaders
                    .FirstOrDefaultAsync(c => c.UserId == userIdcartDetailsId);
                var cartDetails = _dbContext.CartDetails
                    .Where(c => c.CartHeaderId == cartHeader.CartHeaderId)
                    .ToList();

                var cartDto = new CartDto
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(cartHeader),
                    CartDetails = _mapper.Map<List<CartDetailsDto>>(cartDetails)
                };

                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

                foreach (var detail in cartDto.CartDetails)
                {
                    detail.Product = productDtos.FirstOrDefault(p => p.ProductId == detail.ProductId)!;
                    cartDto.CartHeader.CartTotal += detail.Product.Price * detail.Count;
                }

                _responseDto.Result = cartDto;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var existingCartHeader = await _dbContext.CartHeaders
                    .FirstOrDefaultAsync(c => c.UserId == cartDto.CartHeader.UserId);
                if (existingCartHeader != null)
                {
                    existingCartHeader.CouponCode = cartDto.CartHeader.CouponCode;
                    _dbContext.CartHeaders.Update(existingCartHeader);
                    await _dbContext.SaveChangesAsync();
                    _responseDto.Result = true;
                }
                else
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = "Cart not found.";
                }
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<ResponseDto> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var existingCartHeader = await _dbContext.CartHeaders
                    .FirstOrDefaultAsync(c => c.UserId == cartDto.CartHeader.UserId);
                if (existingCartHeader != null)
                {
                    existingCartHeader.CouponCode = "";
                    _dbContext.CartHeaders.Update(existingCartHeader);
                    await _dbContext.SaveChangesAsync();
                    _responseDto.Result = true;
                }
                else
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = "Cart not found.";
                }
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _dbContext.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //create header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _dbContext.CartHeaders.Add(cartHeader);
                    await _dbContext.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    //check if details has same product
                    var cartDetailsFromDb = await _dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //create cartdetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _dbContext.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _dbContext.SaveChangesAsync();
                    }
                }
                _responseDto.Result = cartDto;
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
