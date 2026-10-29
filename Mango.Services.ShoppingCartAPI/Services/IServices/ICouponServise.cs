using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Services.IServices
{
    public interface ICouponServise
    {
        Task<CouponDto> GetCoupon(string CouponCode);
    }
}
