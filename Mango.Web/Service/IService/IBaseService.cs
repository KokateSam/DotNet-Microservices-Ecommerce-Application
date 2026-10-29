using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IBaseService
    {
       Task <ResponceDto?>SendAsync(RequestDto requestDto,bool withbearer=true);
    }
}
