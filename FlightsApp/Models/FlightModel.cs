using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace FlightsApp.Models
{
    public class FlightModel
    {
        public enum Aircraft
        {
            [EnumMember(Value = "Embraer")]
            Embraer,
            [EnumMember(Value = "Boeing")]
            Boeing,
            [EnumMember(Value = "Airbus")]
            Airbus,
            [EnumMember(Value = "Other")]
            Other
        }

        public int Id { get; set; }

        public string FlightNumber { get; set; }

        public DateTime DepartureDate { get; set; }

        public string DepartureLocation { get; set; }

        public string DestinationLocation { get; set; }

        [EnumDataType(typeof(Aircraft))]
        public Aircraft AircraftType { get; set; }
    }
}

