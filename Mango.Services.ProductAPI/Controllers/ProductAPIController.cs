using AutoMapper;
using Mango.Services.CouponAPI.Models.Dto;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        private IMapper   _mapper;
        private readonly ResponceDto _responce;

        public ProductAPIController(AppDbContext dbContext,IMapper mapper)
        {
            _dbcontext = dbContext;
            _mapper = mapper;
            _responce = new ResponceDto();
        }

        [HttpGet]
        public ResponceDto GetAllProduct()
        {
            try
            {

                IEnumerable<Product> productlis = _dbcontext.Products.ToList();

                _responce.Result = _mapper.Map<IEnumerable<ProductDto>>(productlis);

                

            } catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.Message = ex.Message;
            }
            return _responce;
        
        }
        [HttpGet]
        [Route("{id:int}")]
        
        public ResponceDto GetById(int id)
        {
            try
            {
                Product productlist = _dbcontext.Products.First(p => p.ProductId == id);

                _responce.Result=_mapper.Map<ProductDto>(productlist);
                
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.Message = ex.Message;
            
            }
            return _responce;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponceDto Post([FromBody] ProductDto obj)
        {
            try
            {
                if (obj == null)
                {
                    _responce.IsSuccess = false;
                    _responce.Message = "Input Data is Not valid";
                }
                else
                {
                    Product productObj = _mapper.Map<Product>(obj);
                    _dbcontext.Products.Add(productObj);
                    _dbcontext.SaveChanges();
                    _responce.Result = _mapper.Map<ProductDto>(productObj);
                }       
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.Message = ex.Message;
            }
            return _responce;
        }

        [HttpPut]
        [Authorize(Roles ="ADMIN")]
        public ResponceDto PUT([FromBody] ProductDto obj)
        {
            try
            {
                if (obj == null)
                {
                    _responce.IsSuccess = false;
                    _responce.Message = "Invalid data";
                }
                else
                {
                    /*
                    var productid = obj.ProductId;
                    Product existignProduct = _dbcontext.Products.FirstOrDefault(p=>p.ProductId==productid);

                    existignProduct.Name = obj.Name;
                    existignProduct.Price = obj.Price;
                    existignProduct.Description = obj.Description;
                    existignProduct.CategoryName = obj.CategoryName;
                    */
                    Product productObj = _mapper.Map<Product>(obj);
                    _dbcontext.Products.Update(productObj);
                    _dbcontext.SaveChanges();

                    _responce.Result=_mapper.Map<ProductDto>(productObj);

                }

            }
            catch (Exception ex)
            { 
                _responce.IsSuccess = false;
                _responce.Message = ex.Message;
            }
            return _responce;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles ="ADMIN")]
        public ResponceDto Delete(int id)
        {
            try
            {
                Product product = _dbcontext.Products.First(p => p.ProductId == id);
                _dbcontext.Products.Remove(product);
                _dbcontext.SaveChanges();   
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.Message= ex.Message;
            }

            return _responce;
        }
    }  
}
