using Mango.Web.Models;
using Mango.Web.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _icouponService;

        public CouponController( ICouponService icouponService)
        {
            _icouponService = icouponService;
        }
         
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto> list = new();

            ResponceDto? responce = await _icouponService.GetAllCouponAsync();

            if (responce != null && responce.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(responce.Result));
            }else
            {
                TempData["error"] = responce?.Message;
            }

            return View(list);
        }

        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                ResponceDto? responce = await _icouponService.CreateCouponAsync(model);
                if (responce != null && responce.IsSuccess)
                {
                    TempData["success"] = "Coupon created successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }else
                {
                    TempData["error"] = responce?.Message;
                }

            }

            return View(model);
        }

        public async Task<IActionResult> DeleteCoupon(int CouponId)
        {
            ResponceDto? responce = await _icouponService.GetCouponByIdAsync(CouponId);

            if (responce != null && responce.IsSuccess)
            {
                
                CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(responce.Result));
                return View(model);
            }else
            {
                TempData["error"] = responce?.Message;
            }

            return NotFound();
            
        
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCoupon(CouponDto couponDto)
        {
            ResponceDto? responce = await _icouponService.DeleteCouponAsync(couponDto.CouponId);

            if (responce != null && responce.IsSuccess)
            {
                TempData["success"] = "Coupon deleted successfully";
                return RedirectToAction(nameof(CouponIndex));

            }else
            {
                TempData["error"] = responce?.Message;
            }
            return View(couponDto);
        }
    }
}
