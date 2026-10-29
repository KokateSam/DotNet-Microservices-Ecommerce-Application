using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponceDto?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponceDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<ResponceDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto);
    }
}
