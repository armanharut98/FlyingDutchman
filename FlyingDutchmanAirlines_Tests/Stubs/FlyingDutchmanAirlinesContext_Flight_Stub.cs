using FlyingDutchmanAirlines.DatabaseLayer;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines_Tests.Stubs
{
    public class FlyingDutchmanAirlinesContext_Flight_Stub : FlyingDutchmanAirlinesContext
    {
        public FlyingDutchmanAirlinesContext_Flight_Stub(DbContextOptions<FlyingDutchmanAirlinesContext> options) : base(options)
        {
            base.Database.EnsureDeleted();
        }
    }
}
