using FlightsApp.Dtos;
using FlightsApp.Models;
using Microsoft.AspNetCore.Identity;

namespace FlightsApp.Repositories
{
    public interface IAuthRepository
    {
        public Task<AuthResponse> Register(RegisterDto model);
        public Task<string> GenerateJwtToken(LoginDto model);
    }
}
