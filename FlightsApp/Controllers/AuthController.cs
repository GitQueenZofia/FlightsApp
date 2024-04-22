using FlightsApp.Dtos;
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

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!IsValidEmail(model.Email))
            {
                return BadRequest(new AuthResponse { Status = "Error", Message = "Invalid email format." });
            }

            var result = await _authRepository.Register(model);
            if (result.Status != "Success")
            {
                return BadRequest(new AuthResponse { Status = "Error", Message = result.Message });
            }
            return Ok(new AuthResponse { Status = "Success", Message = "User created successfully." });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var token = await _authRepository.GenerateJwtToken(model);
            if (!string.IsNullOrEmpty(token))
                return Ok(new { token });

            return Unauthorized();
        }
    }
}
