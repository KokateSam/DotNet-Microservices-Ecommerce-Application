using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration registerMaps()
        {
            var mapperconfig  = new MapperConfiguration(config => {

                //config.CreateMap<Product, ProductDto>();

                // it can do reverse mapping with both too with - Reversemap()
                config.CreateMap<ProductDto, Product>().ReverseMap();
            });
            return mapperconfig;
        }
    }
}
