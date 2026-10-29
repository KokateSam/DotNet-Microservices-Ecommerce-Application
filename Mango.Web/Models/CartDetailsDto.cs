

using Mango.Web.Models;

namespace Mango.Web.Models
{
    public class CartDetailsDto
    {
       
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        public CartHeaderDto? CartHeader { get; set; }
        public int ProductId { get; set; }
        public ProductDto? product { get; set; }
        public int Count { get; set; }

    }
}
