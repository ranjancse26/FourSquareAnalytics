using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;

namespace FourSquareExplorer.Tests.Library
{
    [TestClass]
    public class VenueTest
    {
        [TestMethod]
        public void TestVenue()
        {
            string authToken = ConfigurationManager.AppSettings["authToken"].ToString();
            var recommentedVenues = NetSquare.VenueExplore("40.7,-74", "", "", "", "", "", "Coffee", "", "", authToken);
            foreach (var place in recommentedVenues.places)
            {
                var recommendedValues = ((List<NetSquare.FourSquareRecommendedVenues.recommends>)place.Value);
                foreach (var recommendedValue in recommendedValues)
                {
                   Assert.AreNotEqual(recommendedValue.venue.name, string.Empty);
                }
            }
        }
    }
}
