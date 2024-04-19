using FlightsApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlightsApp.Data
{
    public class FlightContext : IdentityDbContext<UserModel>
    {
        public DbSet<FlightModel> Flights => Set<FlightModel>();

        public FlightContext(DbContextOptions<FlightContext> options) : base(options)
        {

        }
    }
}
