using Azure.Core.Pipeline;
using FlyingDutchmanAirlines.ControllerLayer;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace FlyingDutchmanAirlines_Tests.ControllerLayer
{
    [TestClass]
    public class FlightControllerTests
    {
        private Mock<FlightService> _mockFlightService;

        [TestInitialize]
        public async Task TestInitialize()
        {
            _mockFlightService = new Mock<FlightService>();
        }

        [TestMethod]
        public async Task GetFlights_Success()
        {
            List<FlightView> returnFlightViews = new List<FlightView>(2)
            {
                new FlightView("1932", ("Groningen", "GRQ"), ("Phoenix", "PHX")),
                new FlightView("841", ("New York City", "JFK"), ("London", "LHR"))
            };
            _mockFlightService.Setup(service => service.GetFlights()).Returns(FlightViewAsyncGenerator(returnFlightViews));

            FlightController controller = new FlightController(_mockFlightService.Object);

            ObjectResult response = await controller.GetFlights() as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);

            Queue<FlightView> content = response.Value as Queue<FlightView>;
            Assert.IsNotNull(content);

            Assert.AreEqual(content.Count, returnFlightViews.Count);
            Assert.IsTrue(returnFlightViews.All(flight => content.Contains(flight)));
        }

        [TestMethod]
        public async Task GetFlights_Failure_FlightNotFound_404()
        {
            _mockFlightService.Setup(service => service.GetFlights()).Throws(new FlightNotFoundException());

            FlightController controller = new FlightController(_mockFlightService.Object);

            ObjectResult response = await controller.GetFlights() as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.NotFound);
            Assert.AreEqual(response.Value, "No flights were found in the database");
        }

        [TestMethod]
        public async Task GetFlights_Failure_ArgumentException_500()
        {
            _mockFlightService.Setup(service => service.GetFlights()).Throws(new ArgumentException());

            FlightController controller = new FlightController(_mockFlightService.Object);

            ObjectResult response = await controller.GetFlights() as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.InternalServerError);
            Assert.AreEqual(response.Value, "An error occured");
        }

        private async IAsyncEnumerable<FlightView> FlightViewAsyncGenerator(IEnumerable<FlightView> views)
        {
            foreach (FlightView view in views)
            {
                yield return view;
            }
        }

        [TestMethod]
        public async Task GetFlightByFlightNumber_Success()
        {
            FlightView returnedFlightView = new FlightView("0", ("Lagos", "LOS"), ("Marrakesh", "RAK"));
            _mockFlightService.Setup(service => service.GetFlightByFlightNumber(0)).Returns(Task.FromResult(returnedFlightView));

            FlightController controller = new FlightController(_mockFlightService.Object);

            ObjectResult response = await controller.GetFlightByFlightNumber(0) as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);

            FlightView content = response.Value as FlightView;
            Assert.IsNotNull(content);
            Assert.AreEqual(content, returnedFlightView);
        }

        [TestMethod]
        public async Task GetFlightByFlightNumber_Failure_FlightNotFound_404()
        {
            _mockFlightService.Setup(service => service.GetFlightByFlightNumber(0)).Throws(new FlightNotFoundException());

            FlightController controller = new FlightController(_mockFlightService.Object);

            ObjectResult response = await controller.GetFlightByFlightNumber(0) as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.NotFound);
            Assert.AreEqual(response.Value, "Flight not found");
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(1)]
        public async Task GetFlightByFlightNumber_Failure_ArgumentException_400(int flightNumber)
        {
            _mockFlightService.Setup(service => service.GetFlightByFlightNumber(flightNumber)).Throws(new ArgumentException());

            FlightController controller = new FlightController(_mockFlightService.Object);

            ObjectResult response = await controller.GetFlightByFlightNumber(flightNumber) as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
            Assert.AreEqual(response.Value, "Bad request");
        }
    }
}
