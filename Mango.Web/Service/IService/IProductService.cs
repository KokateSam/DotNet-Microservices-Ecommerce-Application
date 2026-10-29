

using Mango.Web.Models;

namespace Mango.Web.Service
{
    public interface IProductService
    {
        Task<ResponceDto?> GetAllProductAsync();
        Task<ResponceDto?>GetProductByIdAsync(int ProductId);
        Task<ResponceDto?>CreateProductAsync(ProductDto productDto);
        Task<ResponceDto?>UpdateProductAsync(ProductDto productDto);
        Task<ResponceDto?>DeleteProductAsync(int ProductId);
    }
}
