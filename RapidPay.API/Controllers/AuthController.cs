using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RapidPay.DAL.Models;
using RapidPay.Service.Services.Token;
using RapidPay.Shared.Utils;

namespace RapidPay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly FileLogger _Logger;

        public AuthController(UserManager<User> userManager, 
                              SignInManager<User> signInManager, 
                              ITokenService tokenService,
                              FileLogger fileLogger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _Logger = fileLogger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTO.LoginRequest request)
        {
            // Find the user by username
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                await _Logger.LogAsync($"Login failed for user {request.Username}: User or Password incorrect.");
                return Unauthorized();
            }

            // Check the user's password
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                await _Logger.LogAsync($"Login failed for user {request.Username}: User or Password incorrect.");
                return Unauthorized();
            }

            // Generate the token
            var token = _tokenService.GenerateToken(request.Username);
            await _Logger.LogAsync($"Login succeeded for user {request.Username}.");

            return Ok(new { Token = $"Bearer {token}" });
        }
    }

}
