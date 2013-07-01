using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourSquareLibrary
{ 
    /// <summary>
    /// Wrapper classes for geocoding result
    /// Refer to http://code.google.com/apis/maps/documentation/geocoding/index.html
    /// </summary>
    public class GeoObj
    {
        public class AddressComponent
        {
            public String ShortName { set; get; }
            public String LongName { set; get; }
            public List<String> Types { set; get; }
        }

        public class GeometryClass
        {
            public SpotLatLng Location { set; get; }
            public String LocationType { set; get; }
            public SpotLatLng ViewPortSW { set; get; }
            public SpotLatLng ViewPortNE { set; get; }
        }

        public String FormattedAddress { get; set; }
        public List<String> Types { set; get; }
        public GeometryClass Geometry { set; get; }
        public List<AddressComponent> AddressComponents { set; get; }
    }

    /// <summary>
    ///Wrapper classes for geocoding result
    /// Refer to http://code.google.com/apis/maps/documentation/geocoding/index.html
    /// </summary>
    public class GeoResult
    {
        public GeoResult() { }

        public GeoResult(GeoResultStatus status)
        {
            this.Status = status;
        }

        public GeoResultStatus Status;
        public List<GeoObj> Results { get; set; }

        public static GeoResult NotDefinedResult = new GeoResult(GeoResultStatus.UNKNOWN);
        public static GeoResult BlankResult = new GeoResult(GeoResultStatus.ZERO_RESULTS);
    }

    /// <summary>
    /// Class for accessing Google Geoder API
    /// Author: Jeff
    /// Date: 08/0252010
    /// Refer: http://code.google.com/apis/maps/documentation/geocoding/index.html
    /// </summary>
    public class GoogleGeoAPI : WebUtil
    {
        /// <summary>
        /// key for google language translate
        /// </summary>
        public static String GOOGLE_GEOCODING_API_URL = "http://maps.google.com/maps/api/geocode/";

        public static String SendRequest(SpotInfo sinfo, String format)
        {
            String query = GOOGLE_GEOCODING_API_URL + format;
            if (sinfo.LatLng == SpotLatLng.NOT_DEFINE_LAT_LNG)
                query += "?address=" + sinfo.Address;
            else
                query += "?latlng=" + sinfo.LatLng.ToString();
            query += "&sensor=" + ((sinfo.Sensor) ? "true" : "false");
            if (!String.Empty.Equals(sinfo.Language))
                query += "&language=" + sinfo.Language;
            return WebRequest(query);
        }

    }

    /// <summary>
    /// GetGeoResult with default result format json
    /// @see GetGeoResult
    /// </summary>
    public class Geocoder
    {
        /// <summary>
        /// Get geocoding result, call GetGeoResult(SpotInfo info, string format) with format: "json"
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GeoResult GetGeoResult(SpotInfo info)
        {
            return GetGeoResult(info, "json");
        }

        /// <summary>
        /// Get geocoding result
        /// </summary>
        /// <param name="info"></param>
        /// <param name="format">"json", "xml" or...</param>
        /// <returns></returns>
        public GeoResult GetGeoResult(SpotInfo info, string format)
        {
            if (info == null)
                return GeoResult.BlankResult;
            if (string.Empty.Equals(info.Address) && SpotLatLng.NOT_DEFINE_LAT_LNG.Equals(info.LatLng))
                return GeoResult.BlankResult;

            IGeoResultParser parser = null;
            if ("json".Equals(format))
                parser = new JsonGeoResultParser();
            else
                throw new NotSupportedException();

            String resultText = GoogleGeoAPI.SendRequest(info, format);
            /*parse the resultText to GeoResult Object*/
            GeoResult result = parser.Parse(resultText);
            return result;
        }
    }

    /// <summary>
    /// Contains geographic coordinates and address
    /// </summary>
    public class SpotInfo
    {
        public SpotLatLng LatLng;
        public String Address;
        public bool Sensor;
        public String Language;

        public SpotInfo()
        {
            this.LatLng = SpotLatLng.NOT_DEFINE_LAT_LNG;
            this.Address = String.Empty;
            this.Language = String.Empty;
        }

        public SpotInfo(double lattitude, double longitude)
        {
            this.LatLng = new SpotLatLng(lattitude, longitude);
            this.Address = String.Empty;
            this.Language = String.Empty;
        }
    }

    /// <summary>
    /// Contains geographic coordinates
    /// </summary>
    public class SpotLatLng : ICloneable
    {
        public Double Lat, Lng;
        public SpotLatLng(double Lat, double Lng)
        {
            if (Lng == 0)
                Lng = -999;
            if (Lat == 0)
                Lat = -999;
            this.Lat = Lat;
            this.Lng = Lng;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is SpotLatLng))
                return false;
            SpotLatLng sll = (SpotLatLng)obj;
            return (sll.Lat == Lat && sll.Lng == Lng);
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            double temp;
            temp = Lat;
            result = prime * result + (int)(temp);
            temp = Lng;
            result = prime * result + (int)(temp);
            return result;
        }

        public override string ToString()
        {
            return Lat + "," + Lng;
        }

        public static SpotLatLng NOT_DEFINE_LAT_LNG = new SpotLatLng(-999, -999);

        #region ICloneable Members

        public object Clone()
        {
            return new SpotLatLng(this.Lng, this.Lat);
        }

        #endregion
    }
}
