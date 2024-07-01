using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FlyingDutchmanAirlines.RepositoryLayer
{
    public class BookingRepository
    {
        private readonly FlyingDutchmanAirlinesContext _context;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public BookingRepository()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This constructor should only be used for testing");
            }
        }

        public BookingRepository(FlyingDutchmanAirlinesContext context)
        {
            _context = context;
        }

        public virtual async Task CreateBooking(int customerID, int flightNumber)
        {
            if (!customerID.isPositive() || !flightNumber.isPositive())
            {
                Console.WriteLine($"Argument Exception in CreateBooking()! customerID = {customerID}, flightNumber = {flightNumber}");
                throw new ArgumentException("Invalid argumnets provided");
            }
            Booking booking = new Booking()
            {
                CustomerId = customerID,
                FlightNumber = flightNumber
            };

            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception during databse query: {exception.Message}");
                throw new CouldNotAddBookingToDatabaseException();
            }
        }
    }
}
