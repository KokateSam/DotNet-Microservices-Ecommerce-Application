using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        { 
            _orderService = orderService;
        }

        public IActionResult OrderIndex()
        {
            return View();
        }

        public async Task <IActionResult>OrderDetail(int orderId)
        {
            OrderHeaderDto orderHeaderDto = new();

            string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            var responce = await _orderService.GetOrder(orderId);

            if (responce != null && responce.IsSuccess)
            {
                orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(responce.Result));
            }

            if (!User.IsInRole(SD.RoleAdmin) && userId != orderHeaderDto.UserId)
            {
                return NotFound();
            }

            return View(orderHeaderDto);
        }

        [HttpPost("OrderReadyForPickUp")]
        public async Task<IActionResult> OrderReadyForPickUp(int orderId)
        {
            var responce = await _orderService.UpdateOrderStatus(orderId, SD.Status_ReadyForPickup);
            if (responce != null && responce.IsSuccess)
            {
                TempData["Success"] = "Order status Updated Successfully";

                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }
        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var responce = await _orderService.UpdateOrderStatus(orderId, SD.Status_Completed);
            if (responce != null && responce.IsSuccess)
            {
                TempData["Success"] = "Order status Updated Successfully";

                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var responce = await _orderService.UpdateOrderStatus(orderId, SD.Status_Cancelled);
            if (responce != null && responce.IsSuccess)
            {
                TempData["Success"] = "Order status Updated Successfully";

                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }


        [HttpGet]
        public IActionResult GetAllOrder(string status)
        {
            IEnumerable<OrderHeaderDto> list;
            string userId = "";

            if (!User.IsInRole(SD.RoleAdmin))
            {
                userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;    
            }

            ResponceDto responce = _orderService.GetAllOrders(userId).GetAwaiter().GetResult();

            if (responce != null & responce.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(Convert.ToString(responce.Result));

                switch (status)
                {
                    case "approved":
                        list = list.Where(u => u.Status == SD.Status_Approved);
                        break;
                    case "readyForPickUp":
                        list = list.Where(u => u.Status == SD.Status_ReadyForPickup);
                        break;
                    case "cancelled":
                        list = list.Where(u => u.Status == SD.Status_Cancelled || u.Status == SD.Status_Refunded);
                        break;
                    default:
                        break;

                }

            }
            else { 
            
                list = new List<OrderHeaderDto>();
            }

            return Json(new { data = list });
        }

    }
}
