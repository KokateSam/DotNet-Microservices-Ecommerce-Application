//using Mango.Services.Web.Models;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService,IOrderService orderService)
        {  
         _cartService = cartService;
         _orderService = orderService;
        }
        public async Task<IActionResult> CartIndex()
        {
            return View(await GetCartDtoBasedOnLoginUser());
        }

        public async Task<IActionResult> CheckOut()
        {
            return View(await GetCartDtoBasedOnLoginUser());
        }

        [HttpPost]
        [ActionName("CheckOut")]
        public async Task<IActionResult> CheckOut(CartDto cartDto)
        {
           CartDto cart= await GetCartDtoBasedOnLoginUser();
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;

            //cartDto.CartDetails = cart.CartDetails;

            var  responce = await _orderService.CreateOrder(cart);

            OrderHeaderDto orderHeaderDto =JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(responce.Result));

            if (responce != null && responce.IsSuccess)
            {
                //get stripe session id and redirect to stripe payment page
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                StripeRequestDto stripeRequestDto = new()
                {
                    ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDto.OrderHeaderId,
                    CancelUrl = domain + "cart/CheckOut",
                    OrderHeader = orderHeaderDto
                };

                var strieResponce = await _orderService.CreateStripeSession(stripeRequestDto);

                StripeRequestDto stripeResponceResult = JsonConvert.DeserializeObject<StripeRequestDto>
                                                                        (Convert.ToString(strieResponce.Result));

                Response.Headers.Add("Location",stripeResponceResult.StripeSessionUrl);

                return new StatusCodeResult(303);

            }


            return View();
        }



        [Authorize]
        public async Task<IActionResult> Confirmation(int orderId)
        {

            ResponceDto? responce = await _orderService.ValidateStripeSession(orderId);
            if (responce != null & responce.IsSuccess)
            {
                OrderHeaderDto orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(responce.Result));
                if (orderHeader.Status == SD.Status_Approved)
                {
                    return View(orderId);
                                    
                }
            }
            //redirect to some other/error page based on  paymentIntent status
            return View();
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
