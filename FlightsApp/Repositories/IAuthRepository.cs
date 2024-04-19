using FlightsApp.Models;
using Microsoft.AspNetCore.Identity;

namespace FlightsApp.Repositories
{
    public interface IAuthRepository
    {
        public Task<AuthResponse> Register(RegisterModel model);
        public Task<string> GenerateJwtToken(LoginModel model);
    }
}
