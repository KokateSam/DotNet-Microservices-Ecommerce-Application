using AutoMapper;
using Mango.Services.ShoppingCartAPI.Model;
using Mango.Services.ShoppingCartAPI.Model.Dto;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration registerMaps()
        {
            var mapperconfig  = new MapperConfiguration(config => {

                //config.CreateMap<Product, ProductDto>();

                // it can do reverse mapping with both too with - Reversemap()
                config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
                config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
            });
            return mapperconfig;
        }
    }
}
