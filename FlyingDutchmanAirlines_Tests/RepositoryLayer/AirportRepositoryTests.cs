using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines_Tests.Stubs;
using Microsoft.EntityFrameworkCore;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer
{
    [TestClass]
    public class AirportRepositoryTests
    {
        private FlyingDutchmanAirlinesContext_Airport_Stub _context;
        private AirportRepository _repository;

        [TestInitialize]
        public async Task TestInitialize()
        {
            DbContextOptions<FlyingDutchmanAirlinesContext> dbOptions = new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
                .UseInMemoryDatabase("FlyingDutchman").Options;
            _context = new FlyingDutchmanAirlinesContext_Airport_Stub(dbOptions);

            SortedList<string, Airport> airports = new SortedList<string, Airport>
            {
                {
                    "GOH",
                    new Airport
                    {
                        AirportId = 0,
                        City = "Nuuk",
                        Iata = "GOH"
                    }

                },
                {
                    "PHX",
                    new Airport
                    {
                        AirportId = 1,
                        City = "Phoenix",
                        Iata = "PHX"
                    }
                },
                {
                    "DDH",
                    new Airport
                    {
                        AirportId = 2,
                        City = "Bennington",
                        Iata = "DDH"
                    }
                },
                {
                    "RDU",
                    new Airport
                    {
                        AirportId = 3,
                        City = "Raleigh-Durham",
                        Iata = "RDU"
                    }
                }
            };

            _context.Airports.AddRange(airports.Values);
            await _context.SaveChangesAsync();

            _repository = new AirportRepository(_context);
            Assert.IsNotNull(_repository);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public async Task GetAirportByID_Success(int airportID)
        {
            Airport airport = await _repository.GetAirportByID(airportID);
            Airport dbAirport = _context.Airports.First(a => a.AirportId == airportID);

            Assert.IsNotNull(airport);
            Assert.AreEqual(dbAirport.AirportId, airport.AirportId);
            Assert.AreEqual(dbAirport.City, airport.City);
            Assert.AreEqual(dbAirport.Iata, airport.Iata);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetAirportByID_Failure_InvalidInput()
        {

            StringWriter outputStream = new StringWriter();
            int airportID = -1;
            try
            {
                Console.SetOut(outputStream);
                Airport airport = await _repository.GetAirportByID(airportID);
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(outputStream.ToString().Contains($"Argument exception in GetAirportByID()! airpotID = {airportID}"));
                throw new ArgumentException();
            }
            finally
            {
                outputStream.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AirportNotFoundException))]
        public async Task GetAirportByID_Failure_DatabaseError()
        {
            await _repository.GetAirportByID(69);
        }
    }
}
