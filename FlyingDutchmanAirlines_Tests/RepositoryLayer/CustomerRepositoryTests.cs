using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer
{
    [TestClass]
    public class CustomerRepositoryTests
    {
        private FlyingDutchmanAirlinesContext _context;
        private CustomerRepository _repository;

        [TestInitialize]
        public async Task TestInitialize()
        {
            DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions = new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
                .UseInMemoryDatabase("FlyingDutchman").Options;
            _context = new FlyingDutchmanAirlinesContext(dbContextOptions);

            Customer testCustomer = new Customer("Linus Torvalds");
            _context.Customers.Add(testCustomer);
            await _context.SaveChangesAsync();

            _repository = new CustomerRepository(_context);
            Assert.IsNotNull(_repository);
        }

        [TestMethod]
        public async Task CreateCustomer_Success()
        {
            bool result = await _repository.CreateCustomer("David Letterman");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task CreateCustomer_Failure_NameIsNull()
        {
            bool result = await _repository.CreateCustomer(null);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CreateCustomer_Failure_NameIsEmptyString()
        {
            bool result = await _repository.CreateCustomer(string.Empty);
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow('!')]
        [DataRow('@')]
        [DataRow('#')]
        [DataRow('$')]
        [DataRow('%')]
        [DataRow('&')]
        [DataRow('*')]
        public async Task CreateCustomer_Failure_NameContainsInvalidCharacters(char invalidChar)
        {
            bool result = await _repository.CreateCustomer("David Letterman" + invalidChar);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CreateCustomer_Failure_DatabaseAccessError()
        {
            CustomerRepository repository = new CustomerRepository(null);
            Assert.IsNotNull(repository);

            bool result = await repository.CreateCustomer("David Letterman");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GetCustomerByName_Success()
        {
            Customer customer = await _repository.GetCustomerByName("Linus Torvalds");
            Assert.IsNotNull(customer);

            Customer dbCustomer = _context.Customers.First();
            Assert.AreEqual(customer, dbCustomer);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("#")]
        [DataRow("$")]
        [DataRow("%")]
        [DataRow("&")]
        [DataRow("*")]
        [ExpectedException(typeof(CustomerNotFoundException))]
        public async Task GetCustomerByName_Failure_Invalid_Name(string name)
        {
            Customer customer = await _repository.GetCustomerByName(name);
        }
    }
}