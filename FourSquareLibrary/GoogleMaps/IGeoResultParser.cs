using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourSquareLibrary
{
    /// <summary>
    /// Interface for Geo Result Parser
    /// </summary>
    public interface IGeoResultParser
    {
        /// <summary>
        /// Parse result into object
        /// </summary>
        /// <param name="txt">in json, xml , or .... format</param>
        /// <returns>GeoResult object</returns>
        GeoResult Parse(String txt);
    }
}
