using FlyingDutchmanAirlines.ControllerLayer.JsonData;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FlyingDutchmanAirlines.ControllerLayer
{
    [Route("{controller}")]
    public class BookingController : Controller
    {
        private readonly BookingService _service;

        public BookingController(BookingService service)
        {
            _service = service;
        }

        [HttpPost("{flightNumber}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBooking(int flightNumber, [FromBody] BookingData body)
        {
            if (ModelState.IsValid && flightNumber.isPositive())
            {
                string customerName = $"{body.FirstName} {body.LastName}";
                (bool result, Exception? exception) = await _service.CreateBooking(customerName, flightNumber);

                if (result && exception == null)
                {
                    return StatusCode((int)HttpStatusCode.Created);
                }

                return exception is CouldNotAddBookingToDatabaseException
                    ? StatusCode((int)HttpStatusCode.NotFound)
                    : StatusCode((int)HttpStatusCode.InternalServerError, exception!.Message);
            }

            return StatusCode((int)HttpStatusCode.BadRequest, ModelState.Root.Errors.First().ErrorMessage);
        }
    }
}
