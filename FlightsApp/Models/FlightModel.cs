using System;
using System.ComponentModel.DataAnnotations;

namespace FlightsApp.Models
{
    public class FlightModel
    {
        public enum Aircraft
        {
            Embraer,
            Boeing,
            Airbus,
            Other
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Flight number is required")]
        public string FlightNumber { get; set; }

        [Required(ErrorMessage = "Departure date is required")]
        public DateTime DepartureDate { get; set; }

        [Required(ErrorMessage = "Departure location is required")]
        public string DepartureLocation { get; set; }

        [Required(ErrorMessage = "Destination location is required")]
        public string DestinationLocation { get; set; }

        [Required(ErrorMessage = "Aircraft type is required")]
        public Aircraft AircraftType { get; set; }
    }
}

