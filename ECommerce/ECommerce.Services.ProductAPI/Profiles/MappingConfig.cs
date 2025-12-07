using AutoMapper;
using ECommerce.Services.ProductAPI.Dto;
using ECommerce.Services.ProductAPI.Models;

namespace ECommerce.Services.CouponAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
