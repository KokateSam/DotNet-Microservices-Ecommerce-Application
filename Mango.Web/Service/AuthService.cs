using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        
        }
        public async Task<ResponceDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType=SD.ApiType.POST,
                Url=SD.AuthAPIBase +"/api/auth/AssignRole",
                Data=registrationRequestDto
            
            });   
        }
        public async Task<ResponceDto?>LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            { 
                ApiType=SD.ApiType.POST,
                Url=SD.AuthAPIBase +"/api/auth/Login",
                Data=loginRequestDto
            },withbearer:false);
        }

        public async Task<ResponceDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto() 
            {
                ApiType=SD.ApiType.POST,
                Url=SD.AuthAPIBase + "/api/auth/register",
                Data=registrationRequestDto
            },withbearer:false);
        }

    }
}
