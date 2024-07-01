using System.ComponentModel.DataAnnotations.Schema;

namespace FlyingDutchmanAirlines.DatabaseLayer.Models;

public sealed class Airport
{
    public int AirportId { get; set; }

    public string City { get; set; } = null!;

    public string Iata { get; set; } = null!;

    [NotMapped]
    public ICollection<Flight> FlightDestinationNavigations { get; set; } = new List<Flight>();

    [NotMapped]
    public ICollection<Flight> FlightOriginNavigations { get; set; } = new List<Flight>();
}
