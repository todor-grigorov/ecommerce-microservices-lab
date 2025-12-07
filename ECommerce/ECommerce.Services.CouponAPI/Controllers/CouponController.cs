using AutoMapper;
using ECommerce.Services.CouponAPI.Data;
using ECommerce.Services.CouponAPI.Dto;
using ECommerce.Services.CouponAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Services.CouponAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CouponController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private ResponseDto _response;

        public CouponController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet]
        public IActionResult GetCoupons()
        {
            try
            {
                var coupons = _context.Coupons.ToList();
                var couponsDto = _mapper.Map<List<CouponDto>>(coupons);
                _response.Result = couponsDto;

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
        public IActionResult GetById(int id)
        {
            try
            {
                var coupon = _context.Coupons.First(c => c.CouponId == id);
                var couponDto = _mapper.Map<CouponDto>(coupon);
                _response.Result = couponDto;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public IActionResult GetByCode(string code)
        {
            try
            {
                var coupon = _context.Coupons.FirstOrDefault(c => c.CouponCode.ToLower() == code.ToLower());
                var couponDto = _mapper.Map<CouponDto>(coupon);
                _response.Result = couponDto;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }

        [HttpPost]
        public IActionResult CreateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                var coupon = _mapper.Map<Coupon>(couponDto);
                _context.Coupons.Add(coupon);
                _context.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }

        [HttpPut]
        public IActionResult UpdateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                var coupon = _mapper.Map<Coupon>(couponDto);
                _context.Coupons.Update(coupon);
                _context.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response.IsSuccess ? Ok(_response) : BadRequest(_response);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteCoupon(int id)
        {
            try
            {
                var coupon = _context.Coupons.First(c => c.CouponId == id);
                _context.Coupons.Remove(coupon);
                _context.SaveChanges();
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
