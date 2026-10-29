using Mango.Web.Models;
using Mango.Web.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task< IActionResult> ProductIndex()
        {
             List <ProductDto>productlist = new();

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

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponceDto? responce = await _productService.CreateProductAsync(model);
                if (responce != null && responce.IsSuccess)
                {
                    TempData["Success"] = "Product Created Successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                

            }

            return View(model);
        }

        public async Task<IActionResult> ProductEdit(int ProductId)
        {
            ResponceDto? responce = await _productService.GetProductByIdAsync(ProductId);
            if (responce != null && responce.IsSuccess)
            {
                ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responce.Result));
                return View(model);
            }
            else {

                TempData["error"] = responce?.Message;
            }

                return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponceDto? responce = await _productService.UpdateProductAsync(model);
                if (responce != null && responce.IsSuccess)
                {
                    TempData["Success"] = "Product Updated successfully.";
                    return RedirectToAction(nameof(ProductIndex));
                
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ProductDelete(int ProductId)
        {
            ResponceDto? responce = await _productService.GetProductByIdAsync(ProductId);
            if (responce != null && responce.IsSuccess)
            {
                ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responce.Result));
                return View(model);
            }
            else {
                TempData["error"] = responce?.Message;
            }
              return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponceDto? responce = await _productService.DeleteProductAsync(model.ProductId);
                if (responce != null & responce.IsSuccess)
                {
                    TempData["success"] = "Product deleted successfully";
                    return RedirectToAction(nameof(ProductIndex));

                }
                else {
                    TempData["error"] = responce?.Message;  
                }
            }
           return View(model);
        }


    }
}
