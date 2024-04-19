using FlightsApp.Data;
using FlightsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightsApp.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly FlightContext _flightContext;
        public FlightRepository(FlightContext flightContext)
        {
            _flightContext = flightContext;
        }

        public async Task<FlightModel> AddFlight(FlightModel flight)
        {
            _flightContext.Flights.Add(flight);
            await _flightContext.SaveChangesAsync();
            return flight;
        }

        public async Task<FlightModel> DeleteFlight(int id)
        {
            var flight = await _flightContext.Flights.FindAsync(id);
            if (flight != null)
            {
                _flightContext.Flights.Remove(flight);
                await _flightContext.SaveChangesAsync();
            }
            return flight;
        }

        public async Task<List<FlightModel>> GetAllFlights()
        {
            return await _flightContext.Flights.ToListAsync();
        }

        public async Task<FlightModel> GetFlight(int id)
        {
            return await _flightContext.Flights.FindAsync(id);
        }

        public async Task<FlightModel> UpdateFlight(int id, FlightModel updatedFlight)
        {
            var flight = await _flightContext.Flights.FindAsync(id);
            if (flight != null)
            {
                flight.FlightNumber = updatedFlight.FlightNumber;
                flight.DepartureDate = updatedFlight.DepartureDate;
                flight.DepartureLocation = updatedFlight.DepartureLocation;
                flight.DestinationLocation = updatedFlight.DestinationLocation;
                flight.AircraftType = updatedFlight.AircraftType;

                await _flightContext.SaveChangesAsync();
            }
            return flight;
        }
    }
}
