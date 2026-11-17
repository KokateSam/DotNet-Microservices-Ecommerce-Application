using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Stripe.Climate;

namespace Mango.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;

        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponceDto>CreateOrder(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto {
                ApiType=SD.ApiType.POST,
                Url=SD.OrderAPIBase + "/api/order/CreateOrder",
                Data= cartDto
            });
        }

        public async Task<ResponceDto> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Url = SD.OrderAPIBase + "/api/order/CreateStripeSession",
                Data = stripeRequestDto
            });
            
        }

        public async Task<ResponceDto> GetAllOrders(string? userId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIBase + "/api/order/GetOrders/" + userId

            });
        }

        public async Task<ResponceDto> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIBase + "/api/order/GetOrders/" + orderId
                
            });
        }

        public async Task<ResponceDto> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Url = SD.OrderAPIBase + "/api/order/UpdateOrderStatus/" + orderId,
                Data = newStatus
            });
        }

        public async Task<ResponceDto> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Url = SD.OrderAPIBase + "/api/order/ValidateStripeSession",
                Data = orderHeaderId

            });
        }

    }
}
