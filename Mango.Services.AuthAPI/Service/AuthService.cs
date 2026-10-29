using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDBContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService( AppDBContext dBContext,IJwtTokenGenerator jwtTokenGenerator ,
            UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        { 
            _dbContext = dBContext;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string Email, string roleName)
        {
            var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == Email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if role does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
               await _userManager.AddToRoleAsync(user, roleName);
                return true;

            }
            return false;
        }

        public async Task<LoginResponceDto> Login(LoginRequestDto loginRequestDto)
        {
           var user= _dbContext.ApplicationUsers.FirstOrDefault(u=>u.UserName.ToLower()==loginRequestDto.UserName.ToLower());
           bool isPassValid= await _userManager.CheckPasswordAsync(user,loginRequestDto.Password);

            if(user ==null || isPassValid==false)
            {

                return new LoginResponceDto() { 
                    User=null,
                    Token=""
                };
            }

            //get roles from Usermanager and pass to add as claim while generate token
            //generate token and return with loginresponce dto

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user,roles);
            

            UserDto userDTO = new()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponceDto loginResponceDto = new LoginResponceDto()
            {
                User = userDTO,
                Token = token

            };

            return loginResponceDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName=registrationRequestDto.Email,
                Email=registrationRequestDto.Email,
                NormalizedEmail=registrationRequestDto.Email.ToUpper(),
                Name=registrationRequestDto.Name,
                PhoneNumber=registrationRequestDto.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var UserToReturn = _dbContext.ApplicationUsers.First(u => u.Email == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Id = UserToReturn.Id,
                        Name = UserToReturn.Name,
                        Email = UserToReturn.Email,
                        PhoneNumber = UserToReturn.PhoneNumber
                    };
                    return "";
                }
                else
                {

                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            { 
            
            }

            return "Error Encountered";
        }
    }
}
