using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FlyingDutchmanAirlines_Tests.Stubs
{
    public class FlyingDutchmanAirlinesContext_Booking_Stub : FlyingDutchmanAirlinesContext
    {
        public FlyingDutchmanAirlinesContext_Booking_Stub(DbContextOptions<FlyingDutchmanAirlinesContext> options) : base(options)
        {
            base.Database.EnsureDeleted();
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<EntityEntry> pendingChanges = ChangeTracker.Entries().Where(e => e.State == EntityState.Added);
            IEnumerable<Booking> bookings = pendingChanges.Select(e => e.Entity).OfType<Booking>();
            if (bookings.Any(b => b.CustomerId != 1))
            {
                throw new Exception("Database Exception!");
            }

            await base.SaveChangesAsync(cancellationToken);
            return 1;
        }

        /*
        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int?[] arr = base.Bookings.Select(b => b.CustomerId).ToArray();
            Console.WriteLine(string.Join(',', arr));
            await base.SaveChangesAsync(cancellationToken);
            return base.Bookings.First().CustomerId switch
            {
                1 => 1,
                _ => throw new Exception("Database Error!")
            };
        }
        */
    }
}
