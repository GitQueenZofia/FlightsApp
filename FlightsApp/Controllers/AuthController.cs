using FlightsApp.Models;
using FlightsApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace FlightsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authRepository.Register(model);
            if (result.Status!="Success")
            {
                return BadRequest(new AuthResponse { Status = "Error", Message = result.Message });
            }
            return Ok(new AuthResponse { Status = "Success", Message = "User created successfully" });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var token = await _authRepository.GenerateJwtToken(model);
            if (!string.IsNullOrEmpty(token))
                return Ok(new { token });

            return Unauthorized();
        }
    }
}
