using AutoMapper;
using ECommerce.Services.ShoppingCartAPI.Dto;
using ECommerce.Services.ShoppingCartAPI.Models;

namespace ECommerce.Services.ShoppingCartAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
            CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
        }
    }
}
