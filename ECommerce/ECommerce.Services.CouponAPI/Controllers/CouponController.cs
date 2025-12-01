using ECommerce.Services.CouponAPI.Data;
using ECommerce.Services.CouponAPI.Dto;
using ECommerce.Services.CouponAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Services.CouponAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly AppDbContext _context;
        private ResponseDto _response;

        public CouponController(AppDbContext context)
        {
            _context = context;
            _response = new ResponseDto();
        }

        [HttpGet]
        public IActionResult GetCoupons()
        {
            try
            {
                var coupons = _context.Coupons.ToList();
                _response.Result = coupons;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetCouponById(int id)
        {
            try
            {
                var coupon = _context.Coupons.First(c => c.CouponId == id);
                _response.Result = coupon;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }
    }
}
