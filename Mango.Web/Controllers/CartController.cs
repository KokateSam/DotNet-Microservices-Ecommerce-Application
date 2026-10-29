//using Mango.Services.Web.Models;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {  
         _cartService = cartService;
        }
        public async Task<IActionResult> CartIndex()
        {
            return View(await GetCartDtoBasedOnLoginUser());
        }

        private async Task<CartDto> GetCartDtoBasedOnLoginUser()
        {
            var userId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            ResponceDto responce=await _cartService.GetCartByUserIdAsync(userId);
           


            if (responce != null && responce.IsSuccess)
            {
                CartDto cart = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(responce.Result));
                return cart;
            }
           
                return new CartDto();
               
        }

        public async Task<IActionResult> Remove(int cartDetailId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponceDto? responce = await _cartService.RemoveFromCartAsync(cartDetailId);
            if (responce != null & responce.IsSuccess)
            {
                TempData["Success"] = "Cart Updated Successfully.";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = responce.Message;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>ApplyCoupon(CartDto cartDto)
        {
            //var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            if (cartDto.CartHeader.CouponCode == null)
            {
                TempData["error"] = "Please Add valid Coupon";
                return RedirectToAction(nameof(CartIndex));
            }
            ResponceDto? responce = await _cartService.ApplyCouponAsync(cartDto);

            if (responce != null & responce.IsSuccess)
            {
                TempData["Success"] = "Cart Updated Successfully.";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = responce.Message;
            }
            return View();
           
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
           
            cartDto.CartHeader.CouponCode = "";
            ResponceDto? responce = await _cartService.ApplyCouponAsync(cartDto);

            if (responce != null & responce.IsSuccess)
            {
                TempData["Success"] = "Cart Updated Successfully.";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = responce.Message;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await GetCartDtoBasedOnLoginUser();
            var Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            cart.CartHeader.Email = Email;
            ResponceDto? responce = await _cartService.EmailCart(cart);

            if (responce != null & responce.IsSuccess)
            {
                TempData["Success"] = "Email will be processed and sent shortly.";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = responce.Message;
            }
            return View();

        }
    }
}
