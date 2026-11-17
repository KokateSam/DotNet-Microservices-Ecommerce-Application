using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponceDto> CreateOrder(CartDto cartDto);
        Task<ResponceDto> CreateStripeSession(StripeRequestDto stripeRequestDto);
        Task<ResponceDto> ValidateStripeSession(int orderHeaderId);

        Task<ResponceDto> GetAllOrders(string? userId);
        Task<ResponceDto> GetOrder(int orderId);
        Task<ResponceDto> UpdateOrderStatus(int orderId, string newStatus);
    }
}
