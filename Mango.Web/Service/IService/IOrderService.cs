using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponceDto> CreateOrder(CartDto cartDto);
        Task<ResponceDto> CreateStripeSession(StripeRequestDto stripeRequestDto);
        Task<ResponceDto> ValidateStripeSession(int orderHeaderId);
    }
}
