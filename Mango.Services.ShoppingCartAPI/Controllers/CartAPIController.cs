using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Model;
using Mango.Services.ShoppingCartAPI.Model.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        private IMapper _mapper;
        private ResponceDto _responce;
        private readonly IProductService _productService;
        private readonly ICouponServise _couponServise;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public CartAPIController(AppDbContext dbContext,IMapper mapper,
            IProductService productService,ICouponServise couponServise,IMessageBus messageBus,IConfiguration configuration)
        { 
            _dbcontext = dbContext;
            _mapper = mapper;
            this._responce = new ResponceDto();
            _productService = productService;
            _couponServise = couponServise;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponceDto> GetCart(string userId)
        {
            try
            {
               // var carthederToShow = await _dbcontext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
                //CartHeaderDto cartHeader = _mapper.Map<CartHeaderDto>(carthederToShow);
                CartDto cart = new()
                {
                   // CartHeader = cartHeader,
                   CartHeader=_mapper.Map<CartHeaderDto>(_dbcontext.CartHeaders.First(u=>u.UserId==userId))
                };

                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_dbcontext.Cartdetails
                                                                            .Where(u=>u.CartHeaderId==cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> products = await _productService.GetProduct();

                foreach (var item in cart.CartDetails)
                {
                    item.product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.product.Price);
                }

                //apply coupon if any
                if(!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDto coupon = await _couponServise.GetCoupon(cart.CartHeader.CouponCode);

                    if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount=coupon.DiscountAmount;
                    }
                }
                _responce.Result = cart;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.Message=ex.Message.ToString();
            
            }

            return _responce;
        }
        [HttpPost("ApplyCoupon")]
        public async Task<ResponceDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {    

                var cartfromDb = await _dbcontext.CartHeaders.FirstAsync(c => c.UserId == cartDto.CartHeader.UserId);
                cartfromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _dbcontext.SaveChangesAsync();
                _responce.Result = true; 
            }
            catch (Exception ex)
            { 
                _responce.IsSuccess = false;
                _responce.Message = ex.Message.ToString();
            }
            return _responce;
        }

        //Remove coupon can be done from apply coupon endpoint with passing empty string for couponcode in input cartdto for remove coupon
        /*
        [HttpPost("RemoveCoupon")]
        public async Task<ResponceDto> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _dbcontext.CartHeaders.FirstAsync(c => c.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = "";
                _dbcontext.CartHeaders.Update(cartFromDb);
                _dbcontext.SaveChangesAsync();
                _responce.Result = true;
            }
            catch (Exception ex)
            { 
                _responce.IsSuccess=false;
                _responce.Message = ex.Message.ToString();
            }
            return _responce;
        }
        */

        [HttpPost("CartUpsert")]
        public async Task <ResponceDto> CartUpsert(CartDto cartDto) 
        {
            try
            {
                var cartHeaderFrmDb = await _dbcontext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFrmDb == null)
                {
                    // Create new cartheader and Add details for that header initially
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _dbcontext.CartHeaders.Add(cartHeader);
                    await _dbcontext.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;

                    CartDetails cartdetail = _mapper.Map<CartDetails>(cartDto.CartDetails.First());

                    _dbcontext.Cartdetails.Add(cartdetail);
                    await _dbcontext.SaveChangesAsync();
                }
                else
                {
                    // if header already exist then fetch that header having userId and
                    // add details to that header
                    var cartDetailFromDb = await _dbcontext.Cartdetails.AsNoTracking().FirstOrDefaultAsync(u =>
                                    u.CartHeaderId == cartHeaderFrmDb.CartHeaderId &&
                                    u.ProductId == cartDto.CartDetails.First().ProductId);
                    if (cartDetailFromDb == null)
                    {
                        //Create new cartdetails for given cartheader no previous record or item in cart
                        cartDto.CartDetails.First().CartHeaderId=cartHeaderFrmDb.CartHeaderId;
                        CartDetails cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
                        _dbcontext.Cartdetails.Add(cartDetails);
                        await _dbcontext.SaveChangesAsync();
                    }
                    else
                    {
                        //update cart details into existing cartdetails 
                        cartDto.CartDetails.First().Count += cartDetailFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailFromDb.CartDetailsId;

                        var cdetails = _mapper.Map<CartDetails>(cartDto.CartDetails.First());

                        _dbcontext.Cartdetails.Update(cdetails);
                        await _dbcontext.SaveChangesAsync();
                    }

                }
                _responce.Result = cartDto;
            }
            catch (Exception ex)
            {
                _responce.Message = ex.Message.ToString();
                _responce.IsSuccess= false;
            }
            return _responce;
        }

        [HttpPost("CartRemove")]
        public async Task<ResponceDto> CartRemove([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _dbcontext.Cartdetails.First(u => u.CartDetailsId == cartDetailsId);
                int totalcartitemCount = _dbcontext.Cartdetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _dbcontext.Cartdetails.Remove(cartDetails);

                if (totalcartitemCount == 1)
                {
                    var CartHeaderToRemove = await _dbcontext.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _dbcontext.CartHeaders.Remove(CartHeaderToRemove);
                
                }
                await _dbcontext.SaveChangesAsync();
                
                
                _responce.Result = true;
            }
            catch (Exception ex)
            {
                _responce.Message = ex.Message.ToString();
                _responce.IsSuccess = false;
            }
            return _responce;
        }


        //Asynchronous api communication using Azure Service Bus 

        [HttpPost("EmailCartRequest")]
        public async Task<ResponceDto> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                await _messageBus.PublishMessage(cartDto,_configuration.GetValue<string>("TopicAndQueNames:EmailShoppingCartQueue"));
                
                _responce.Result = true;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.Message = ex.Message.ToString();
            }
            return _responce;
        }


    }
}
