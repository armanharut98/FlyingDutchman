using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using Moq;

namespace FlyingDutchmanAirlines_Tests.ServiceLayer
{
    [TestClass]
    public class BookingServiceTests
    {

        private Mock<BookingRepository> _mockBookingRepository;
        private Mock<CustomerRepository> _mockCustomerRepository;
        private Mock<FlightRepository> _mockFlightRepository;

        [TestInitialize]
        public async Task TestInitialize()
        {
            _mockBookingRepository = new Mock<BookingRepository>();
            _mockCustomerRepository = new Mock<CustomerRepository>();
            _mockFlightRepository = new Mock<FlightRepository>();
        }

        [TestMethod]
        public async Task CreateBooking_Success()
        {
            _mockBookingRepository.Setup(repository => repository.CreateBooking(0, 0)).Returns(Task.CompletedTask);
            _mockCustomerRepository.Setup(repository => repository.GetCustomerByName("David Letterman")).Returns(Task.FromResult(new Customer("David Letterman")));
            _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(0)).ReturnsAsync(new Flight());

            BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);

            (bool result, Exception? exception) = await service.CreateBooking("David Letterman", 0);
            Assert.IsTrue(result);
            Assert.IsNull(exception);
        }

        [TestMethod]
        [DataRow("", 0)]
        [DataRow(null, -1)]
        [DataRow("Galileo Galilei", -1)]
        public async Task CreateBooking_Failure_InvalidInputArguments(string customerName, int flightNumber)
        {
            BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);

            (bool result, Exception? exception) = await service.CreateBooking(customerName, flightNumber);

            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public async Task CreateBooking_Failure_RepositoryException_ArgumentException()
        {
            _mockBookingRepository.Setup(repository => repository.CreateBooking(0, 1)).Throws(new ArgumentException());
            _mockCustomerRepository.Setup(repository => repository.GetCustomerByName("Galileo Galilei")).Returns(Task.FromResult(new Customer("Galileo Galilei") { CustomerId = 0 }));
            _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(1)).Returns(Task.FromResult(new Flight()));

            BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);

            (bool result, Exception? exception) = await service.CreateBooking("Galileo Galilei", 1);
            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
            Assert.IsTrue(exception is ArgumentException);
        }

        [TestMethod]
        public async Task CreateBooking_Failure_RepositoryException_CouldNotAddBookingToDatabaseException()
        {
            _mockBookingRepository.Setup(repository => repository.CreateBooking(1, 2)).Throws(new CouldNotAddBookingToDatabaseException());
            _mockCustomerRepository.Setup(repository => repository.GetCustomerByName("Eise Eisinga")).Returns(Task.FromResult(new Customer("Eise Eisinga") { CustomerId = 1 }));
            _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(0)).Returns(Task.FromResult(new Flight()));

            BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);

            (bool result, Exception? exception) = await service.CreateBooking("Eise Eisinga", 2);
            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
            Assert.IsTrue(exception is CouldNotAddBookingToDatabaseException);
        }

        [TestMethod]
        public async Task CreateBooking_Failure_FlightNotInDatabase()
        {
            _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(-1)).Throws(new FlightNotFoundException());
            BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);

            (bool result, Exception? exception) = await service.CreateBooking("Johnny Carson", 1);
            
            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDatabaseException));
        }

        /*
        [TestMethod]
        public async Task CreateBooking_Success_CustomerNotInDatabase()
        {
            _mockCustomerRepository.Setup(repository => repository.GetCustomerByName("Conan O'Brien")).Throws(new CustomerNotFoundException());
            _mockBookingRepository.Setup(repository => repository.CreateBooking(0, 0)).Returns(Task.CompletedTask);
            _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(0)).ReturnsAsync(new Flight());

            BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);

            (bool result, Exception? exception) = await service.CreateBooking("Conan O'Brien", 0);

            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(CustomerNotFoundException));
        }
        */

        [TestMethod]
        public async Task CreateBooking_Success_CustomerNotInDatabase()
        {
            _mockCustomerRepository.SetupSequence(repository => repository.GetCustomerByName("Conan O'Brien"))
                .Throws(new CustomerNotFoundException())
                .ReturnsAsync(new Customer("Conan O'Brien") { CustomerId = 0 });
            _mockCustomerRepository.Setup(repository => repository.CreateCustomer("Conan O'Brien")).ReturnsAsync(true);
            _mockBookingRepository.Setup(repository => repository.CreateBooking(0, 0)).Returns(Task.CompletedTask);
            _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(0)).ReturnsAsync(new Flight());

            BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);

            (bool result, Exception? exception) = await service.CreateBooking("Conan O'Brien", 0);

            Assert.IsTrue(result);
            Assert.IsNull(exception);
        }

        [TestMethod]
        public async Task CreateBooking_Failure_CustomerNotInDatabase_RepositoryFailure()
        {
            _mockBookingRepository.Setup(repository => repository.CreateBooking(0, 0)).Throws(new CouldNotAddBookingToDatabaseException());
            _mockCustomerRepository.Setup(repository => repository.GetCustomerByName("Bill Gates")).ReturnsAsync(new Customer("Bill Gates"));
            _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(0)).Returns(Task.FromResult(new Flight()));

            BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);

            (bool result, Exception? exception) = await service.CreateBooking("Bill Gates", 0);

            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDatabaseException));
        }

        [TestMethod]
        public async Task CreateBooking_Failure_CustomerNotInDatabase_CustomerCouldNotBeAdded()
        {
            _mockCustomerRepository.Setup(repository => repository.GetCustomerByName("Bill Gates")).Throws(new CustomerNotFoundException());
            _mockCustomerRepository.Setup(repository => repository.CreateCustomer("Bill Gates")).ReturnsAsync(false);

            BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);

            (bool result, Exception? exception) = await service.CreateBooking("Bill Gates", 0);

            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(CustomerNotFoundException));
        }
    }
}
