using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using Moq;

namespace FlyingDutchmanAirlines_Tests.ServiceLayer
{
    [TestClass]
    public class FlightServiceTests
    {
        private Mock<FlightRepository> _mockFlightRepository;
        private Mock<AirportRepository> _mockAirportRepository;

        [TestInitialize]
        public async Task TestInitialize()
        {
            _mockFlightRepository = new Mock<FlightRepository>();
            _mockAirportRepository = new Mock<AirportRepository>();

            Flight flightInDatabase = new Flight
            {
                FlightNumber = 148,
                Origin = 31,
                Destination = 49
            };
            Queue<Flight> mockReturn = new Queue<Flight>(1);
            mockReturn.Enqueue(flightInDatabase);
            _mockFlightRepository.Setup(repository => repository.GetFlights()).ReturnsAsync(mockReturn);
            _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(148)).ReturnsAsync(flightInDatabase);

            _mockAirportRepository.Setup(repository => repository.GetAirportByID(31)).ReturnsAsync(new Airport
            {
                AirportId = 31,
                City = "Mexico City",
                Iata = "MEX"
            });
            _mockAirportRepository.Setup(repository => repository.GetAirportByID(49)).ReturnsAsync(new Airport
            {
                AirportId = 49,
                City = "Ulaanbaataar",
                Iata = "UBN"
            });
        }

        [TestMethod]
        public async Task GetFlights_Success()
        {
            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);

            await foreach (FlightView flightView in service.GetFlights())
            {
                Assert.IsNotNull(flightView);
                Assert.AreEqual(flightView.FlightNumber, "148");
                Assert.AreEqual(flightView.Origin.City, "Mexico City");
                Assert.AreEqual(flightView.Origin.Code, "MEX");
                Assert.AreEqual(flightView.Destination.City, "Ulaanbaataar");
                Assert.AreEqual(flightView.Destination.Code, "UBN");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlights_Failure_RepositoryException()
        {
            _mockAirportRepository.Setup(repository => repository.GetAirportByID(31)).ThrowsAsync(new AirportNotFoundException());

            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);
            await foreach(FlightView _ in service.GetFlights()) {
                ;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetFlights_Failure_RegularException()
        {
            _mockAirportRepository.Setup(repository => repository.GetAirportByID(31)).ThrowsAsync(new NullReferenceException());

            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);
            await foreach (FlightView _ in service.GetFlights())
            {
                ;
            }
        }

        [TestMethod]
        public async Task GetFlightByFlightNumber_Success()
        {
            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);
            FlightView flightView = await service.GetFlightByFlightNumber(148);

            Assert.AreEqual(flightView.FlightNumber, "148");
            Assert.AreEqual(flightView.Origin.City, "Mexico City");
            Assert.AreEqual(flightView.Origin.Code, "MEX");
            Assert.AreEqual(flightView.Destination.City, "Ulaanbaataar");
            Assert.AreEqual(flightView.Destination.Code, "UBN");
        }

        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlightByFlightNumber_Failure_FlightRepository_FlightNotFoundException()
        {
            _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(-1)).ThrowsAsync(new FlightNotFoundException());

            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);
            await service.GetFlightByFlightNumber(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlightByFlightNumber_Failure_AirportRepository_AirportNotFoundException()
        {
            _mockAirportRepository.Setup(repository => repository.GetAirportByID(31)).ThrowsAsync(new AirportNotFoundException());

            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);
            await service.GetFlightByFlightNumber(148);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetFlightByFlightNumber_Failure_AirportRepository_ArgumentException()
        {
            _mockAirportRepository.Setup(repository => repository.GetAirportByID(31)).ThrowsAsync(new ArgumentException());

            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);
            await service.GetFlightByFlightNumber(148);
        }
    }
}
