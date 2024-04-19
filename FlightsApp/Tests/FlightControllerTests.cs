using AutoMapper;
using FlightsApp.Controllers;
using FlightsApp.Data;
using FlightsApp.Dtos;
using FlightsApp.Helper;
using FlightsApp.Models;
using FlightsApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FlightsApp.Tests
{
    public class FlightControllerTests
    {
        private DbContextOptions<FlightContext> _options;
        private readonly FlightContext _context;
        private readonly FlightController _controller;

        public FlightControllerTests()
        {
            _options = new DbContextOptionsBuilder<FlightContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;
            _context = new FlightContext(_options);
            _context.Database.EnsureCreated();


            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfiles());
            });
            IMapper mapper = new Mapper(configuration);

            var _flightRepository = new FlightRepository(_context);
            _controller = new FlightController(_flightRepository, mapper);
        }

        private static SqliteConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        }
        [Fact]
        public async Task AddFlight_ReturnsAddedFlight()
        {
            // Arrange
            var newFlight = new FlightDto
            {
                FlightNumber = "FA101",
                DepartureDate = DateTime.UtcNow,
                DepartureLocation = "Warszawa",
                DestinationLocation = "Bydgoszcz",
                AircraftType = FlightModel.Aircraft.Boeing
            };

            // Act
            var result = await _controller.AddFlight(newFlight);

            // Assert
            var actionResult = Assert.IsType<ActionResult<FlightModel>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var model = Assert.IsType<FlightModel>(okObjectResult.Value);

            var savedFlight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightNumber == newFlight.FlightNumber);
            Assert.NotNull(savedFlight);
            Assert.Equal(newFlight.DepartureDate, savedFlight.DepartureDate);
            Assert.Equal(newFlight.DepartureLocation, savedFlight.DepartureLocation);
            Assert.Equal(newFlight.DestinationLocation, savedFlight.DestinationLocation);
            Assert.Equal(newFlight.AircraftType, savedFlight.AircraftType);
        }
        [Fact]
        public async Task GetAllFlights_ReturnsFlightsList()
        {
            // Arrange
            var flightsToAdd = new List<FlightModel>
            {
                new FlightModel
                {
                    FlightNumber = "FA101",
                    DepartureDate = DateTime.UtcNow,
                    DepartureLocation = "Warszawa",
                    DestinationLocation = "Kraków",
                    AircraftType = FlightModel.Aircraft.Boeing
                },
                new FlightModel
                {
                    FlightNumber = "FA102",
                    DepartureDate = DateTime.UtcNow.AddDays(1),
                    DepartureLocation = "Gdańsk",
                    DestinationLocation = "Kraków",
                    AircraftType = FlightModel.Aircraft.Airbus
                },
            };

            _context.Flights.AddRange(flightsToAdd);
            await _context.SaveChangesAsync();

            // Act
            var actionResult = await _controller.GetAllFlights();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var model = Assert.IsAssignableFrom<List<FlightModel>>(okObjectResult.Value);
            Assert.Equal(flightsToAdd.Count, model.Count);
        }
        [Fact]
        public async Task GetFlightById_ShouldReturnFlight()
        {
            // Arrange
            var newFlight = new FlightDto
            {
                FlightNumber = "FA101",
                DepartureDate = DateTime.UtcNow,
                DepartureLocation = "Warszawa",
                DestinationLocation = "Bydgoszcz",
                AircraftType = FlightModel.Aircraft.Boeing
            };

            // Act
            await _controller.AddFlight(newFlight);
            var result = await _controller.GetFlight(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<FlightModel>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var model = Assert.IsType<FlightModel>(okObjectResult.Value);

            Assert.NotNull(model);
            Assert.Equal(newFlight.FlightNumber, model.FlightNumber);
            Assert.Equal(newFlight.DepartureDate, model.DepartureDate);
            Assert.Equal(newFlight.DepartureLocation, model.DepartureLocation);
            Assert.Equal(newFlight.DestinationLocation, model.DestinationLocation);
            Assert.Equal(newFlight.AircraftType, model.AircraftType);
        }
        [Fact]
        public async Task GetFlightById_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.GetFlight(1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task UpdateFlight_ShouldUpdateFlight()
        {
            // Arrange
            var newFlight = new FlightDto
            {
                FlightNumber = "FA101",
                DepartureDate = DateTime.UtcNow,
                DepartureLocation = "Warszawa",
                DestinationLocation = "Bydgoszcz",
                AircraftType = FlightModel.Aircraft.Boeing
            };

            await _controller.AddFlight(newFlight);

            var updatedFlight = new FlightDto
            {
                FlightNumber = "FA102",
                DepartureDate = DateTime.UtcNow.AddDays(1),
                DepartureLocation = "Gdańsk",
                DestinationLocation = "Kraków",
                AircraftType = FlightModel.Aircraft.Airbus
            };

            // Act
            var result = await _controller.UpdateFlight(1, updatedFlight);

            // Assert
            var actionResult = Assert.IsType<ActionResult<FlightModel>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var model = Assert.IsType<FlightModel>(okObjectResult.Value);

            Assert.Equal(updatedFlight.DepartureDate, model.DepartureDate);
            Assert.Equal(updatedFlight.DepartureLocation, model.DepartureLocation);
            Assert.Equal(updatedFlight.DestinationLocation, model.DestinationLocation);
            Assert.Equal(updatedFlight.AircraftType, model.AircraftType);
        }

        [Fact]
        public async Task DeleteFlight_ShouldDeleteFlight()
        {
            // Arrange
            var newFlight = new FlightDto
            {
                FlightNumber = "FA101",
                DepartureDate = DateTime.UtcNow,
                DepartureLocation = "Warszawa",
                DestinationLocation = "Bydgoszcz",
                AircraftType = FlightModel.Aircraft.Boeing
            };

            await _controller.AddFlight(newFlight);

            // Act
            var result = await _controller.DeleteFlight(1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var message = Assert.IsType<string>(actionResult.Value);
            Assert.Equal("Flight deleted.", message);
        }
        [Fact]
        public async Task DeleteFlight_NonExistingFlight_ReturnsBadRequestWithMessage()
        {
            // Act
            var result = await _controller.DeleteFlight(999);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var message = Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal("Flight not found.", message);
        }
    }
}

