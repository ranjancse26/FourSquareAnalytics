using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourSquareLibrary
{
    /// <summary>
    /// Parse Json Result 
    ///  Refer to http://code.google.com/apis/maps/documentation/geocoding/index.html
    /// </summary>

    public class JsonGeoResultParser : IGeoResultParser
    {
        /// <summary>
        ///  Parse JSON result to object
        /// </summary>
        /// <param name="json">Input String in JSON format</param>
        /// <returns>class GeoResult containts parsred data</returns>
        public GeoResult Parse(string json)
        {
            GeoResult georesult = new GeoResult();

            JObject o = JObject.Parse(json);
            georesult.Status = GeoResultStatus.GetByKey((String)o["status"]);
            JArray results = (JArray)o["results"];
            georesult.Results = new List<GeoObj>();
            for (int x = 0; x < results.Count; x++)
            {
                GeoObj gobj = new GeoObj();
                JObject ojb = (JObject)results[x];
                gobj.FormattedAddress = (String)ojb["formatted_address"];
                JArray types = (JArray)ojb["types"];
                List<String> stypes = new List<String>();
                for (int i = 0; i < types.Count; i++)
                    stypes.Add((String)types[i]);
                gobj.Types = stypes;
                JArray addComps = (JArray)ojb["address_components"];
                gobj.AddressComponents = new List<GeoObj.AddressComponent>();
                for (int i = 0; i < addComps.Count; i++)
                {
                    GeoObj.AddressComponent acomp = new GeoObj.AddressComponent();
                    acomp.Types = new List<String>();
                    JObject jo = (JObject)addComps[i];
                    acomp.LongName = (String)jo["long_name"];
                    acomp.ShortName = (String)jo["short_name"];
                    JArray _types = (JArray)jo["types"];
                    for (int j = 0; j < _types.Count; j++)
                    {
                        acomp.Types.Add((String)_types[j]);
                    }
                    gobj.AddressComponents.Add(acomp);
                }
                gobj.Geometry = new FourSquareLibrary.GeoObj.GeometryClass();
                JObject geo = (JObject)ojb["geometry"];
                JObject geo_location = (JObject)geo["location"];
                gobj.Geometry.Location = new SpotLatLng(DataUtil.StringToDouble(geo_location["lat"].ToString()), DataUtil.StringToDouble(geo_location["lng"].ToString()));
                gobj.Geometry.LocationType = (String)geo["location_type"];
                JObject viewport = (JObject)geo["viewport"];
                JObject vwsw = (JObject)viewport["southwest"];
                gobj.Geometry.ViewPortSW = new SpotLatLng(DataUtil.StringToDouble(vwsw["lat"].ToString()), DataUtil.StringToDouble(vwsw["lng"].ToString()));
                JObject vwne = (JObject)viewport["northeast"];
                gobj.Geometry.ViewPortNE = new SpotLatLng(DataUtil.StringToDouble(vwne["lat"].ToString()), DataUtil.StringToDouble(vwne["lng"].ToString()));
                georesult.Results.Add(gobj);
            }
            return georesult;
        }
    }
}
