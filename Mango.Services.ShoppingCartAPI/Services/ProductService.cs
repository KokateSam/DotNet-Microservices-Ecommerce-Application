using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Services
{
    public class ProductService : IProductService
    {
        public readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        { 
            _httpClientFactory = httpClientFactory;
        }
        public async  Task<IEnumerable<ProductDto>> GetProduct()
        {
           var client = _httpClientFactory.CreateClient("Product");
           var responce = await client.GetAsync($"/api/product");
           var apicontent=await responce.Content.ReadAsStringAsync();
           var resp=JsonConvert.DeserializeObject<ResponceDto>(apicontent);

            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Result));
            }
            return new List<ProductDto>();
            
        }
    }
}
