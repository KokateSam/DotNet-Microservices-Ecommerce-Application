
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;

        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponceDto?> GetAllProductAsync()
        {
            return await _baseService.SendAsync(new RequestDto { 
                ApiType=SD.ApiType.GET,
                Url=SD.ProductAPIBase + "/api/product"
            });
        }

        public async Task<ResponceDto?> GetProductByIdAsync(int ProductId)
        {
            return await _baseService.SendAsync(new RequestDto { 
                ApiType = SD.ApiType.GET,
                Url=SD.ProductAPIBase + "/api/product/" + ProductId
            });
        }

        public async Task<ResponceDto?> CreateProductAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto{
                ApiType = SD.ApiType.POST,
                Url=SD.ProductAPIBase + "/api/product",
                Data=productDto
            });
        }
        public async Task<ResponceDto?> UpdateProductAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.PUT,
                Url = SD.ProductAPIBase + "/api/product",
                Data = productDto
            });
        }
        public async Task<ResponceDto?> DeleteProductAsync(int ProductId)
        {
            return await _baseService.SendAsync(new RequestDto { 
                ApiType=SD.ApiType.DELETE,
                Url=SD.ProductAPIBase +"/api/product/" + ProductId 
            });  
        }       
    }
}
