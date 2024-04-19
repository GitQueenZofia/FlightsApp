using FlightsApp.Models;

namespace FlightsApp.Repositories
{
    public interface IFlightRepository
    {
        Task<List<FlightModel>> GetAllFlights();
        Task<FlightModel> GetFlight(int id);
        Task<FlightModel> AddFlight(FlightModel flight);
        Task<FlightModel> UpdateFlight(int id, FlightModel updatedFlight);
        Task<FlightModel> DeleteFlight(int id);
    }
}
