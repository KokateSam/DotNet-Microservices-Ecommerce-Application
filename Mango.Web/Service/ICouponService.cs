using Mango.Web.Models;

namespace Mango.Web.Service
{
    public interface ICouponService
    {
        Task<ResponceDto?>GetCouponAsync(string couponCode);
        Task<ResponceDto?>GetAllCouponAsync();
        Task<ResponceDto?> GetCouponByIdAsync(int couponId);
        Task<ResponceDto?> CreateCouponAsync(CouponDto couponDto);
        Task<ResponceDto?> UpdateCouponAsync(CouponDto couponDto);
        Task<ResponceDto?> DeleteCouponAsync(int couponId);
    }
}
