
using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel;



namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        
        public HomeController(IProductService productService,ICartService cartService)
        {
            _productService = productService;   
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> productlist = new();

            ResponceDto responce = await _productService.GetAllProductAsync();
            if (responce != null & responce.IsSuccess)
            {
                productlist = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(responce.Result));
            }
            else
            {
                TempData["error"] = responce.Message;
            }

            return View(productlist);
        }


        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDto? model = new();

            ResponceDto responce = await _productService.GetProductByIdAsync(productId);
            if (responce != null & responce.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responce.Result));
            }
            else
            {
                TempData["error"] = responce.Message;
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {
            CartDto cart = new CartDto()
            {
                CartHeader = new CartHeaderDto()
                {
                    UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetails = new CartDetailsDto()
            {
                ProductId = productDto.ProductId,
                Count = productDto.Count
            };

            List < CartDetailsDto > cartDetailsDtos= new() { cartDetails };
            cart.CartDetails= cartDetailsDtos;

            ResponceDto? responce=await _cartService.UpsertCartAsync(cart);

            if (responce != null && responce.IsSuccess)
            {
                TempData["Success"] = "Item has been added to shopping cart";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = responce?.Message; 
            }
            return View(productDto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
