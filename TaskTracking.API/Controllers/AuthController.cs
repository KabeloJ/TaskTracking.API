using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TaskTracking.Core.Entities;
using TaskTracking.Infrastructure.Data;
using System.Security.Cryptography;
using TaskTracking.API.Models;
using TaskTracking.Infrastructure.Repositories;
using TaskTracking.Application.Services;
using TaskTracking.Application.Models;


namespace TaskTracking.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginModel request)
        {
            try
            {
                var token = await _userService.LoginAsync(request.Email, request.Password);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid email or password.");
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserModel request)
        {
            try
            {
                await _userService.RegisterUserAsync(request.Username, request.Email, request.Password);
                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse()
                {
                    IsSuccessful = false,
                    Message = ex.Message,   
                });
            }
        }

    }
    public class ErrorResponse
    {
        public bool IsSuccessful { get; set; }  
        public string Message { get; set; }
    }
}
