using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        private readonly ResponceDto _responce;

        private IMapper _mapper;

        public CouponAPIController(AppDbContext dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext;
            _responce = new ResponceDto();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponceDto Get()
        {

            try
            {
                IEnumerable<Coupon> coupenlist = _dbcontext.Coupons.ToList();

                _responce.Result = _mapper.Map<IEnumerable<CouponDto>>(coupenlist);

            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.Message = ex.Message; ;

            }
            return _responce;

        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponceDto GetById(int id)
        {
            try
            {
                Coupon couponlist = _dbcontext.Coupons.First(c => c.CouponId == id);

                _responce.Result = _mapper.Map<CouponDto>(couponlist);

                


            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.Message = ex.Message; ;
            }

            return _responce;

        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponceDto GetByCode(string code)
        {
            try
            {
                Coupon CouponObj = _dbcontext.Coupons.FirstOrDefault(c => c.CouponCode.ToLower() == code.ToLower());
                if (CouponObj == null)
                {
                    _responce.IsSuccess = false;
                }

                _responce.Result = _mapper.Map<CouponDto>(CouponObj);
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.Message = ex.Message;
            }
            return _responce;
        }

        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public ResponceDto Post([FromBody] CouponDto obj)
        {
            try
            {
                if (obj == null)
                {
                    _responce.IsSuccess = false;
                    _responce.Message = "Input data is null";
                }
                Coupon couponObj = _mapper.Map<Coupon>(obj);

                _dbcontext.Coupons.Add(couponObj);
                _dbcontext.SaveChanges();

                //Create a coupon in Stripe
                try
                {
                    
                    var options = new Stripe.CouponCreateOptions
                    {
                        AmountOff = (long)(obj.DiscountAmount * 100), // Convert to cents
                        Name = obj.CouponCode,
                        Currency = "usd",
                        Id = obj.CouponCode,
                    };
                    var service = new Stripe.CouponService();
                    var coupon = service.Create(options);
                    /*
                    Console.WriteLine($"Coupon Created: {coupon.Id}");

                    
                    var service1 = new Stripe.CouponService();

                    var coupon1 = service.Get(obj.CouponCode);
                    */


                } catch (Stripe.StripeException ex)
                {
                    Console.WriteLine($"Stripe Error: {ex.Message}");
                    Console.WriteLine($"Error Code: {ex.StripeError?.Code}");
                }

                //End of Stripe coupon creation

                _responce.Result = _mapper.Map<CouponDto>(couponObj);

            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.Message = ex.Message;
            }
            return _responce;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponceDto Put([FromBody] CouponDto obj)
        {
            try
            {
                
                Coupon CouponObj = _mapper.Map<Coupon>(obj);
                _dbcontext.Coupons.Update(CouponObj);
                _dbcontext.SaveChanges();

                _responce.Result = _mapper.Map<CouponDto>(CouponObj);

            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.Message = ex.Message;
            
            }
            return _responce;
        
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public ResponceDto Delete(int id)
        {
            try
            {
                Coupon obj = _dbcontext.Coupons.First(u=>u.CouponId==id);

                _dbcontext.Coupons.Remove(obj);
                _dbcontext.SaveChanges();

                var service = new Stripe.CouponService();
                service.Delete(obj.CouponCode);

            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.Message = ex.Message;
            }
            return _responce;
        }
    }
}
