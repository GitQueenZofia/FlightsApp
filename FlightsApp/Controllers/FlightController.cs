using FlightsApp.Data;
using FlightsApp.Models;
using FlightsApp.Repositories;
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
        private readonly IFlightRepository _flightRepository;

        public FlightController(IFlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }

        [HttpPost]
        public async Task<ActionResult<FlightModel>> AddFlight(FlightModel flight)
        {
            var addedFlight = await _flightRepository.AddFlight(flight);
            return Ok(addedFlight);
        }

        [HttpGet]
        public async Task<ActionResult<List<FlightModel>>> GetAllFlights()
        {
            var flights = await _flightRepository.GetAllFlights();
            return Ok(flights);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FlightModel>> GetFlight(int id)
        {
            var flight = await _flightRepository.GetFlight(id);
            if (flight == null)
            {
                return BadRequest("Flight not found.");
            }
            return Ok(flight);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FlightModel>> UpdateFlight(int id, FlightModel updatedFlight)
        {
            var flight = await _flightRepository.UpdateFlight(id, updatedFlight);
            if (flight == null)
            {
                return BadRequest("Flight not found.");
            }
            return Ok(flight);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteFlight(int id)
        {
            var result = await _flightRepository.DeleteFlight(id);
            if (result == null)
            {
                return BadRequest("Flight not found.");
            }
            return Ok("Flight deleted.");
        }
    }
}
