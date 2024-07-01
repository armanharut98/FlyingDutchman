using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.Views;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FlyingDutchmanAirlines.ServiceLayer
{
    public class FlightService
    {
        private readonly FlightRepository _flightRepository;
        private readonly AirportRepository _airportRepository;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public FlightService()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This constructor should only be used for testing");
            }
        }

        public FlightService(FlightRepository flightRepository, AirportRepository airportRepository)
        {
            _flightRepository = flightRepository;
            _airportRepository = airportRepository;
        }

        public virtual async IAsyncEnumerable<FlightView> GetFlights()
        {
            Queue<Flight> flights = await _flightRepository.GetFlights();
            foreach (Flight flight in flights)
            {
                Airport originAirport;
                Airport destinationAirport;

                try
                {
                    originAirport = await _airportRepository.GetAirportByID(flight.Origin);
                    destinationAirport = await _airportRepository.GetAirportByID(flight.Destination);
                } 
                catch (AirportNotFoundException)
                {
                    throw new FlightNotFoundException();
                }
                catch (Exception)
                {
                    throw new ArgumentException();
                }

                yield return new FlightView(
                    flight.FlightNumber.ToString(), 
                    (originAirport.City, originAirport.Iata), 
                    (destinationAirport.City, destinationAirport.Iata)
                );
            }
        }

        public virtual async Task<FlightView> GetFlightByFlightNumber(int flightNumber)
        {
            Flight flight = await _flightRepository.GetFlightByFlightNumber(flightNumber);

            Airport originAirport;
            Airport destinationAirport;
            try
            {
                originAirport = await _airportRepository.GetAirportByID(flight.Origin);
                destinationAirport = await _airportRepository.GetAirportByID(flight.Destination);
            }
            catch (AirportNotFoundException)
            {
                throw new FlightNotFoundException();
            }
            catch (Exception)
            {
                throw new ArgumentException();
            }

            return new FlightView(
                flight.FlightNumber.ToString(), 
                (originAirport.City, originAirport.Iata), 
                (destinationAirport.City, destinationAirport.Iata)
            );
        }
    }
}
