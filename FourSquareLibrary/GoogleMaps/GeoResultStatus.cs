using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourSquareLibrary
{
    /// <summary>
    /// Enum for Geo result status
    /// Refer :http://code.google.com/apis/maps/documentation/geocoding/index.html#StatusCodes
    /// </summary>
    public class GeoResultStatus
    {
        internal GeoResultStatus(String s)
        {
            this.Key = s;
            if (!status.Contains(this))
                status.Add(this);
        }

        public static GeoResultStatus GetByKey(String key)
        {
            key = key.Trim();
            for (int i = 0; i < status.Count; i++)
            {
                if (status[i].Key == key)
                    return status[i];
            }
            return UNKNOWN;
        }

        private static List<GeoResultStatus> status = new List<GeoResultStatus>();

        public String Key { set; get; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(GeoResultStatus))
                return false;
            return ((GeoResultStatus)obj).Key == this.Key;
        }

        public override int GetHashCode()
        {
            if (Key == null)
                return 0;
            return Key.GetHashCode();
        }

        public static GeoResultStatus OK = new GeoResultStatus("OK");
        public static GeoResultStatus ZERO_RESULTS = new GeoResultStatus("ZERO_RESULTS");
        public static GeoResultStatus OVER_QUERY_LIMIT = new GeoResultStatus("OVER_QUERY_LIMIT");
        public static GeoResultStatus REQUEST_DENIED = new GeoResultStatus("REQUEST_DENIED");
        public static GeoResultStatus INVALID_REQUEST = new GeoResultStatus("INVALID_REQUEST");
        public static GeoResultStatus UNKNOWN = new GeoResultStatus("UNKNOWN");
    }
}
