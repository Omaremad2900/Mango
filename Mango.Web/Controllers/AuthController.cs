using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly ITokenProvider token;
       

        public AuthController(IAuthService authService,ITokenProvider token)
        {
            this.authService = authService;
            this.token = token;
            
        }
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            ResponseDto responseDto = await authService.LoginAsync(obj);
            if(responseDto!=null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponse =
                    JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                await SignInUser(loginResponse);
                token.setToken(loginResponse.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
              
                TempData["error"] = responseDto.Message;
                return View(obj);
            }
        }
        public IActionResult Register()
        {
            var rolelist = new List<SelectListItem>()
            { new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin },
              new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer }
              
            };
            ViewBag.Rolelist = rolelist;
          return View(); 
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterationRequestDto obj)
        {
            ResponseDto result=await authService.RegisterAsync(obj);
            ResponseDto assignRole;
            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(obj.Role))
                {
                    obj.Role = SD.RoleCustomer;
                }
                assignRole = await authService.AssignRoleAsync(obj);
                if (assignRole != null && result.IsSuccess)
                {
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }

            }
            else
                TempData["error"] = result.Message;
            var rolelist = new List<SelectListItem>()
            { new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin },
              new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer }

            };
            ViewBag.Rolelist = rolelist;

            return View(obj);
        }

        public async Task<IActionResult> Logout()
        {
             await HttpContext.SignOutAsync();
            token.ClearToken();
            return RedirectToAction("Index","Home");
        }
        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));


            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));



            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
