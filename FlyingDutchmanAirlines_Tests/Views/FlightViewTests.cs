﻿using FlyingDutchmanAirlines.Views;

namespace FlyingDutchmanAirlines_Tests.Views
{
    [TestClass]
    public class FlightViewTests
    {
        [TestMethod]
        public void Constructor_FlightView_Success()
        {
            string flightNumber = "0";
            string originCity = "Amsterdam";
            string originCityCode = "AMS";
            string destinationCity = "Moscow";
            string destinationCityCode = "SVO";

            FlightView view = new FlightView(flightNumber, (originCity, originCityCode), (destinationCity, destinationCityCode));

            Assert.AreEqual(view.FlightNumber, flightNumber);
            Assert.AreEqual(view.Origin.City, originCity);
            Assert.AreEqual(view.Origin.Code, originCityCode);
            Assert.AreEqual(view.Destination.City, destinationCity);
            Assert.AreEqual(view.Destination.Code, destinationCityCode);
        }

        [TestMethod]
        public void Constructor_FlightView_Success_FlightNumber_Null()
        {
            string originCity = "Athens";
            string originCityCode = "ATH";
            string destinationCity = "Dubai";
            string destinationCityCode = "DXB";

            FlightView view = new FlightView(null, (originCity, originCityCode), (destinationCity, destinationCityCode));

            Assert.AreEqual(view.FlightNumber, "No flight number found");
            Assert.AreEqual(view.Origin.City, originCity);
            Assert.AreEqual(view.Destination.City, destinationCity);
        }

        [TestMethod]
        public void Constructor_FlightView_Success_City_EmptyString()
        {
            string originCity = string.Empty;
            string originCityCode = "SYD";

            AirportInfo airportInfo = new AirportInfo((originCity, originCityCode));

            Assert.AreEqual(airportInfo.City, "No city found");
            Assert.AreEqual(airportInfo.Code, originCityCode);
        }

        [TestMethod]
        public void Constructor_FlightView_Success_Code_EmptyString()
        {
            string originCity = "Los Angeles";
            string originCityCode = string.Empty;

            AirportInfo airportInfo = new AirportInfo((originCity, originCityCode));

            Assert.AreEqual(airportInfo.City, originCity);
            Assert.AreEqual(airportInfo.Code, "No code found");
        }
    }
}
