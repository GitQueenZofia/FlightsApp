using AutoMapper;
using FlightsApp.Data;
using FlightsApp.Dtos;
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
        private readonly IMapper _mapper;

        public FlightController(IFlightRepository flightRepository, IMapper mapper)
        {
            _flightRepository = flightRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<FlightModel>> AddFlight([FromBody] FlightDto addedFlight)
        {
            var addedFlightDto = _mapper.Map<FlightModel>(addedFlight);
            var flight = await _flightRepository.AddFlight(addedFlightDto);
            return Ok(flight);
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

        [HttpPut("id")]
        public async Task<ActionResult<FlightModel>> UpdateFlight(int id, [FromBody]FlightDto updatedFlight)
        {
            var updatedFlightDto = _mapper.Map<FlightModel>(updatedFlight);
            var flight = await _flightRepository.UpdateFlight(id, updatedFlightDto);
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
