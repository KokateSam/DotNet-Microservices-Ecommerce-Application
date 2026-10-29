
using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponceDto?> GetCartByUserIdAsync(string UserId);
        Task<ResponceDto?> UpsertCartAsync(CartDto cartDto);
        Task<ResponceDto?> RemoveFromCartAsync(int CartDetailId);
        Task<ResponceDto?> ApplyCouponAsync(CartDto cartDto);
        Task<ResponceDto?> EmailCart(CartDto cartDto);

    }
}
