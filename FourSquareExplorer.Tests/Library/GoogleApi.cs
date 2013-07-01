using FourSquareLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FourSquareExplorer.Tests.Library
{
    [TestClass]
    public class GoogleApi
    {
        [TestMethod]
        public void TestGoogleGeoCodingResponse()
        {
            SpotInfo sinfo = new SpotInfo();
            sinfo.Address = "1310 South White Oak Drive , Apt 714 , Waukegan";
            Geocoder geo = new Geocoder();
            GeoResult result = geo.GetGeoResult(sinfo);
            Assert.AreEqual(result.Status, GeoResultStatus.OK);
        }

        [TestMethod]
        public void TestCurrentLocation()
        {
            SpotInfo sinfo = new SpotInfo(42.370525571796215, -87.85941852461143);
            //sinfo.LatLng = new SpotLatLng(42.370525571796215, -87.85941852461143);
            sinfo.Sensor = true;
            Geocoder geo = new Geocoder();
            GeoResult geoResult = geo.GetGeoResult(sinfo);
            Assert.IsTrue(geoResult.Results.Count > 0);
        }
    }
}
