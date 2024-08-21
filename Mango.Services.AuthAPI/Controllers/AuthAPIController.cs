using Azure;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        protected ResponseDto response {  get; set; }
        private readonly IAuthService _authService;
        public AuthAPIController (IAuthService authService)
        {
            _authService = authService;
            response = new();
        }
        

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterationRequestDto model)
        {
            var errorMessage=await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage)) 
            {
                response.IsSuccess = false;
                response.Message = errorMessage;
                return BadRequest(response);
            
            }
            return Ok(response);
        }
        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody]LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse == null)
            {
                response.IsSuccess = false;
                response.Message = "Username or password is incorrect";
                return BadRequest(response);
            }
            response.Result = loginResponse;
            return Ok(response);
        }
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegisterationRequestDto model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                response.IsSuccess = false;
                response.Message = "Error encountered";
                return BadRequest(response);
            }
            return Ok(response);

        }
    }
}
