using AutoMapper;
using Lider_V_APIServices.Models;
using Lider_V_APIServices.Models.Dto;

namespace Lider_V_APIServices.Helpers
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.ProductImage))
                .ReverseMap();
                config.CreateMap<CartItem, CartItemDto>().ReverseMap();
                config.CreateMap<Cart, CartDto>().ReverseMap();
                config.CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.CategoryImage, opt => opt.MapFrom(src => src.CategoryImage))
                .ReverseMap();
            });

            return mappingConfig;
        }
    }
}
