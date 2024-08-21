using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IJwtTokenGenerator jwt;

        public AuthService (AppDbContext db,UserManager<ApplicationUser> userManger,RoleManager<IdentityRole> roleManager,IJwtTokenGenerator jwt )
        {
            this.db = db;
            this.userManager = userManger;
            this.roleManager = roleManager;
            this.jwt = jwt;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == email.ToLower());
            if(user != null)
            {
                if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult()) 
                {
                    roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user=db.ApplicationUsers.FirstOrDefault(u=>u.UserName.ToLower()==loginRequestDto.Username.ToLower());
            bool isvalid=await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null || isvalid == false) 
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }
            var roles = await userManager.GetRolesAsync(user);
            var token = jwt.GenerateToken(user,roles);
            UserDto userDto = new()
            {
                Email=user.Email,
                ID=user.Id,
                Name=user.Name,
                PhoneNumber=user.PhoneNumber
            };
            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token
            };
            return loginResponseDto;
        }

        public async Task<string> Register(RegisterationRequestDto registerationRequesDto)
        {
            ApplicationUser user = new()
            {
                UserName=registerationRequesDto.Email,
                Email=registerationRequesDto.Email,
                Name=registerationRequesDto.Name,
                PhoneNumber=registerationRequesDto.PhoneNumber,
            };
            try
            {
                var result=await userManager.CreateAsync(user,registerationRequesDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = db.ApplicationUsers.First(u => u.UserName == registerationRequesDto.Email);

                    UserDto userDto = new()
                    {
                        ID = userToReturn.Id,
                        Email = userToReturn.Email,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber,
                    };
                    return "";
                }
                else
                    return result.Errors.FirstOrDefault().Description;
                
            }
            catch (Exception ex) {  }
            return "Error Encountered";
        }
    }
}
