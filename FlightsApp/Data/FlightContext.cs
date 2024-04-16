using FlightsApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlightsApp.Data
{
    public class FlightContext : IdentityDbContext<User>
    {
        public DbSet<Flight> Flights => Set<Flight>();

        public FlightContext(DbContextOptions<FlightContext> options) : base(options)
        {

        }
    }
}
