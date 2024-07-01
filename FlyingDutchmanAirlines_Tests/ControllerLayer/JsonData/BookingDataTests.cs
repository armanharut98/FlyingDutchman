using FlyingDutchmanAirlines.ControllerLayer.JsonData;

namespace FlyingDutchmanAirlines_Tests.ControllerLayer.JsonData
{
    [TestClass]
    public class BookingDataTests
    {
        [TestMethod]
        public void BookingData_ValidData()
        {
            BookingData bookingData = new BookingData
            {
                FirstName = "Marina",
                LastName = "Michaels"
            };
            Assert.AreEqual(bookingData.FirstName, "Marina");
            Assert.AreEqual(bookingData.LastName, "Michaels");
        }

        [TestMethod]
        [DataRow("Mike", null)]
        [DataRow(null, "Morand")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BookingData_InvalidData_NullPointers(string firstName, string lastName)
        {
            BookingData bookingData = new BookingData
            {
                FirstName = firstName,
                LastName = lastName
            };
        }

        [TestMethod]
        [DataRow("Mike", "")]
        [DataRow("", "Morand")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BookingData_InvalidData_EmptyStrings(string firstName, string lastName)
        {
            BookingData bookingData = new BookingData
            {
                FirstName = firstName,
                LastName = lastName
            };
        }

    }
}
