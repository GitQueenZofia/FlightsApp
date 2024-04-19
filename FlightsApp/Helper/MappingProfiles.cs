using AutoMapper;
using FlightsApp.Dtos;
using FlightsApp.Models;

namespace FlightsApp.Helper
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<FlightModel, FlightDto>();
            CreateMap<FlightDto, FlightModel>();
        }
    }
}
