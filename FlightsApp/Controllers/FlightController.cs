using FlightsApp.Data;
using FlightsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightsApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly FlightContext _flightContext;
        public FlightController(FlightContext flightContext)
        {
            _flightContext = flightContext;
        }

        [HttpPost]
        public async Task<ActionResult<Flight>> AddFlight(Flight flight)
        {
            _flightContext.Flights.Add(flight);
            await _flightContext.SaveChangesAsync();
            return Ok(flight);
        }

        [HttpGet]
        public async Task<ActionResult<List<Flight>>> GetAllFlights()
        {
            return Ok(await _flightContext.Flights.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(int id)
        {
            var flight = await _flightContext.Flights.FindAsync(id);
            if(flight == null)
            {
                return BadRequest("Flight not found.");
            }
            return Ok(flight);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Flight>> UpdateFlight(int id, Flight updatedFlight)
        {
            var flight = await _flightContext.Flights.FindAsync(id);
            if (flight == null)
            {
                return BadRequest("Flight not found.");
            }

            flight.FlightNumber = updatedFlight.FlightNumber;
            flight.DepartureDate = updatedFlight.DepartureDate;
            flight.DepartureLocation = updatedFlight.DepartureLocation;
            flight.DestinationLocation = updatedFlight.DestinationLocation;
            flight.AircraftType = updatedFlight.AircraftType;

            await _flightContext.SaveChangesAsync();

            return Ok(flight);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteFlight(int id)
        {
            var flight = await _flightContext.Flights.FindAsync(id);
            if (flight == null)
            {
                return BadRequest("Flight not found.");
            }
            _flightContext.Flights.Remove(flight);

            await _flightContext.SaveChangesAsync();

            return Ok("Flight deleted.");
        }
    }
}
