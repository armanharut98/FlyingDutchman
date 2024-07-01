using FlyingDutchmanAirlines_Tests.Stubs;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer
{
    [TestClass]
    public class FlightRepositoryTests
    {
        private FlyingDutchmanAirlinesContext_Flight_Stub _context;
        private FlightRepository _repository;

        [TestInitialize]
        public async Task TestInitialize()
        {
            DbContextOptions<FlyingDutchmanAirlinesContext> dbOptions = new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
                .UseInMemoryDatabase("FlyingDutchman").Options;
            _context = new FlyingDutchmanAirlinesContext_Flight_Stub(dbOptions);

            Flight flight1 = new Flight
            {
                FlightNumber = 1,
                Origin = 1,
                Destination = 2
            };
            _context.Flights.Add(flight1);
            Flight flight2 = new Flight
            {
                FlightNumber = 10,
                Origin = 3,
                Destination = 4
            };
            _context.Flights.Add(flight2);

            await _context.SaveChangesAsync();

            _repository = new FlightRepository(_context);
            Assert.IsNotNull(_repository);
        }

        [TestMethod]
        public async Task GetFlightByFlightNumber_Success()
        {
            Flight dbFlight = _context.Flights.First(f => f.FlightNumber == 1);
            Assert.IsNotNull(dbFlight);

            Flight flight = await _repository.GetFlightByFlightNumber(1);
            Assert.IsNotNull(flight);

            Assert.AreEqual(flight.FlightNumber, dbFlight.FlightNumber);
            Assert.AreEqual(flight.Origin, dbFlight.Origin);
            Assert.AreEqual(flight.Destination, dbFlight.Destination);
        }

        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlightByFlightNumber_Failure_DatabaseException()
        {
            await _repository.GetFlightByFlightNumber(2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetFlightByFlightNumber_Failure_InvalidFlightNumber()
        {
            await _repository.GetFlightByFlightNumber(-1);
        }

        [TestMethod]
        public async Task GetFLights_Success()
        {
            Queue<Flight> flights = await _repository.GetFlights();

            Assert.IsNotNull(flights);
            Assert.AreEqual(flights.Count, 2);
            Assert.AreEqual(flights.Dequeue().FlightNumber, 1);
            Assert.AreEqual(flights.Dequeue().FlightNumber, 10);

        }
    }
}
