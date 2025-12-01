using AutoMapper;
using ECommerce.Services.CouponAPI.Dto;
using ECommerce.Services.CouponAPI.Models;

namespace ECommerce.Services.CouponAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Coupon, CouponDto>().ReverseMap();
        }
    }
}
