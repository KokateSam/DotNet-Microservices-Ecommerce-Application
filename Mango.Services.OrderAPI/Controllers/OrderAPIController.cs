using AutoMapper;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Services.IServices;
using Mango.Services.OrderAPI.Utility;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        private IMapper _mapper;
        private IProductService _productService;
        protected ResponceDto _response;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public OrderAPIController(AppDbContext dbContext,IMapper mapper,IProductService productService,
                                                        IMessageBus messageBus,IConfiguration configuration)
        { 
           _dbcontext = dbContext;
            _mapper = mapper;
            _productService = productService;
            this._response = new ResponceDto();
            _messageBus = messageBus;
            _configuration = configuration;

        }

        [Authorize]
        [HttpGet("GetOrders")]
        public ResponceDto? Get(string? userId = "")
        { 
            try
            {
                IEnumerable<OrderHeader> objList;
                if (User.IsInRole(SD.RoleAdmin))
                {
                    objList = _dbcontext.OrderHeaders.Include(u => u.OrderDetails).OrderByDescending(u => u.OrderHeaderId).ToList();
                }
                else
                {
                    objList = _dbcontext.OrderHeaders.Include(u => u.OrderDetails).Where(u => u.UserId == userId).OrderByDescending(u => u.OrderHeaderId);
                }

                _response.Result = _mapper.Map<OrderHeaderDto>(objList);

            }catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetOrders/{id:int}")]
        public ResponceDto? Get(int id)
        {
            try
            {
                 OrderHeader orderHeader = _dbcontext.OrderHeaders.Include(u => u.OrderDetails).First(u => u.OrderHeaderId == id);
                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponceDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated = _dbcontext.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _dbcontext.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;


               _response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponceDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {

                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl= stripeRequestDto.CancelUrl,

                    LineItems = new List<SessionLineItemOptions>(), 
                    Mode = "payment"
                };
                // Create a list to hold discount options
                var discountObj =new List <SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDto.OrderHeader.CouponCode
                    }

                };
                // Add line items for each order detail
                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var SessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), //$20.99 =>2099
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.product.Name,
                            }
                        },
                        Quantity = item.Count,

                    };
                    options.LineItems.Add(SessionLineItem);
                }

                // Apply discount if available
                if (stripeRequestDto.OrderHeader.Discount > 0)
                { 
                    options.Discounts= discountObj;
                }

               // Create the Stripe session
               var service = new SessionService();
               Session session= service.Create(options);

                stripeRequestDto.StripeSessionUrl=session.Url;
                OrderHeader orderHeader = _dbcontext.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                await _dbcontext.SaveChangesAsync();
                _response.Result = stripeRequestDto;

            }
            catch (Exception ex)
            { 
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }




        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponceDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            { 

                OrderHeader orderHeader = _dbcontext.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);
                // Retrieve the Stripe session using the stored StripeSessionId 
                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    //payment is successful, update the order status to approved
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;
                    await _dbcontext.SaveChangesAsync();

                    RewardsDto rewardsDto = new()
                    {
                        OrderId=orderHeader.OrderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId
                    };

                    var topicName = _configuration.GetValue<string>("TopicAndQueNames:OrderCreatedTopic");

                    // Publish the rewardsDto to the message bus for  demo Topic in Azure Service Bus
                    await _messageBus.PublishMessage(rewardsDto, topicName);

                    _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
