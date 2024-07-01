using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FlyingDutchmanAirlines.RepositoryLayer
{
    public class AirportRepository
    {
        private readonly FlyingDutchmanAirlinesContext _context;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AirportRepository()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This constructor should only be used for testing");
            }
        }

        public AirportRepository(FlyingDutchmanAirlinesContext context)
        {
            this._context = context;
        }

        public virtual async Task<Airport> GetAirportByID(int airportID)
        {
            if (!airportID.isPositive())
            {
                Console.WriteLine($"Argument exception in GetAirportByID()! airpotID = {airportID}");
                throw new ArgumentException("Invalid arguments provided.");
            }

            return await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == airportID)
                    ?? throw new AirportNotFoundException();
        }
    }
}
