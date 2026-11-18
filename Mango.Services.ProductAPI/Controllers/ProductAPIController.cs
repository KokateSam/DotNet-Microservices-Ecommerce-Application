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
        public ResponceDto Post(ProductDto productDto)
        {
            try
            {
                if (productDto == null)
                {
                    _responce.IsSuccess = false;
                    _responce.Message = "Input Data is Not valid";
                }
                else
                {
                    Product product = _mapper.Map<Product>(productDto);
                    _dbcontext.Products.Add(product);
                    _dbcontext.SaveChanges();

                    if (productDto.Image != null)
                    {
                        string fileName = product.ProductId + Path.GetExtension(productDto.Image.FileName);
                        string filePath = @"wwwroot\ProductImages\" + fileName;
                        var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                        using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                        { 
                            productDto.Image.CopyTo(fileStream);
                        
                        }


                        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                        product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                        product.ImageLocalPath = filePath;
                        
                    }
                    else
                    {
                        product.ImageUrl = "https://placehold.co/600*400";
                    }
                    
                    _dbcontext.Products.Update(product);
                    _dbcontext.SaveChanges();

                    _responce.Result = _mapper.Map<ProductDto>(product);
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
        public ResponceDto PUT( ProductDto productDto)
        {
            try
            {
                if (productDto == null)
                {
                    _responce.IsSuccess = false;
                    _responce.Message = "Invalid data";
                }
                else
                {
                    Product product = _mapper.Map<Product>(productDto);
                    if (productDto.Image != null)
                    {
                        //delete previous file in  ProductAPI solution ProductImage folder also when product is deleted
                        if (!string.IsNullOrEmpty(product.ImageLocalPath))
                        {
                            var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);

                            FileInfo file = new FileInfo(oldFilePathDirectory);

                            if (file.Exists)
                            {
                                file.Delete();
                            }
                        }

                        //Add new file for update file

                        string fileName = product.ProductId + Path.GetExtension(productDto.Image.FileName);
                        string filePath = @"wwwroot\ProductImages\" + fileName;
                        var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                        using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                        {
                            productDto.Image.CopyTo(fileStream);
                        }

                        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                        product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                        product.ImageLocalPath = filePath;

                    }







                    _dbcontext.Products.Update(product);
                    _dbcontext.SaveChanges();

                    _responce.Result=_mapper.Map<ProductDto>(product);

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

                //delete file in  ProductAPI solution ProductImage folder also when product is deleted
                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);

                    FileInfo file = new FileInfo(oldFilePathDirectory);

                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }


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
