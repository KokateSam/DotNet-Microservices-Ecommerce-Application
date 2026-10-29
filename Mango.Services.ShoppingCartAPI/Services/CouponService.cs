using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Services
{
    public class CouponService : ICouponServise
    {
        public readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory httpClientFactory)
        { 
            _httpClientFactory = httpClientFactory;
        }
        public async Task<CouponDto> GetCoupon(string CouponCode)
        {   
            var client = _httpClientFactory.CreateClient("Coupon");
            var responce = await client.GetAsync($"/api/coupon/GetByCode/{CouponCode}");
            var apicontent =await responce.Content.ReadAsStringAsync();

            var resp = JsonConvert.DeserializeObject<ResponceDto>(apicontent);

            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
            }

            return new CouponDto();
        }
    }
}
