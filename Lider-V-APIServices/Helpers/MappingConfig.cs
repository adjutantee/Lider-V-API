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
                config.CreateMap<ProductDto, Product>().ReverseMap();
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
                config.CreateMap<Cart, CartDto>().ReverseMap();
                config.CreateMap<Category, CategoryDto>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
