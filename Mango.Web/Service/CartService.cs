
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;

        public CartService(IBaseService baseService)
        { 
            _baseService = baseService;
        }
        public async Task<ResponceDto?> ApplyCouponAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto 
            {
                ApiType=SD.ApiType.POST,
                Url=SD.ShoppingCartAPIBase + "/api/cart/ApplyCoupon",
                Data=cartDto
            });
        }

        public async  Task<ResponceDto?> GetCartByUserIdAsync(string UserId)
        {

            return await _baseService.SendAsync(new RequestDto
            { 
                ApiType=SD.ApiType.GET,
                Url=SD.ShoppingCartAPIBase + "/api/cart/GetCart/"+UserId
            });
        }

        public async  Task<ResponceDto?> RemoveFromCartAsync(int CartDetailId)
        {

            return await _baseService.SendAsync(new RequestDto
            { 
                ApiType = SD.ApiType.POST,
                Url=SD.ShoppingCartAPIBase + "/api/cart/CartRemove",
                Data=CartDetailId

            });
        }

        public async Task<ResponceDto?> UpsertCartAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto 
            {
                ApiType= SD.ApiType.POST,
                Url=SD.ShoppingCartAPIBase + "/api/cart/CartUpsert",
                Data=cartDto
            });
            
        }
        //Async api communication for send Email using Azure service bus

        public async Task<ResponceDto?> EmailCart(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartAPIBase + "/api/cart/EmailCartRequest",
                Data = cartDto
            });

        }

    }
}
