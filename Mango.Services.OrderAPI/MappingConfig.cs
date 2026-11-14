using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;

namespace Mango.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration registerMaps()
        {
            var mapperconfig  = new MapperConfiguration(config => {

                //config.CreateMap<Product, ProductDto>();

                // it can do reverse mapping with both too with - Reversemap()
                /*
                config.CreateMap<OrderHeaderDto, CartHeaderDto>()
                .ForMember(dest => dest.CartTotal, s => s.MapFrom(src => src.OrderTotal)).ReverseMap();
                */
                config.CreateMap<CartHeaderDto, OrderHeaderDto>()
                .ForMember(dest => dest.OrderTotal, s => s.MapFrom(src => src.CartTotal)).ReverseMap();

                config.CreateMap< CartDetailsDto,OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, s => s.MapFrom(src => src.product.Name))
                .ForMember(dest => dest.Price, s => s.MapFrom(src => src.product.Price));

                config.CreateMap<OrderDetailsDto,CartDetailsDto>();

                config.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
                config.CreateMap<OrderDetails, OrderDetailsDto>().ReverseMap();

            });
            return mapperconfig;
        }
    }
}
