using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponceDto _responce;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _responce = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMssage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMssage))
            {
                _responce.IsSuccess = false;
                _responce.Message = errorMssage;
            }
            return Ok(_responce);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginresponce = await _authService.Login(model);
            if (loginresponce.User == null)
            {
                _responce.IsSuccess = false;
                _responce.Message = "Username or Password is incorrect";
                return BadRequest(_responce);
            }

            _responce.Result = loginresponce;
            return Ok(_responce);
        }
        
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var AssignRolesuccssful = await _authService.AssignRole(model.Email, model.Role.ToUpper());

            if (!AssignRolesuccssful)
            {
                _responce.IsSuccess = false;
                _responce.Message = "error encountered";
                return BadRequest(_responce);
            }
            return Ok(_responce);
        }

    }
}
