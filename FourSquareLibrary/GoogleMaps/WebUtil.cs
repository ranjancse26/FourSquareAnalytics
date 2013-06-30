using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FourSquareLibrary
{
    /// <summary>
    /// Summary description for WebUtil
    /// </summary>
    public class WebUtil
    {
        public WebUtil()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// General WebRequest
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>Result as string</returns>
        public static String WebRequest(String url)
        {
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

                // Get response  
                using (response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    // Console application output  
                    return reader.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                // This exception will be raised if the server didn't return 200 - OK  
                // Try to retrieve more information about the network error  
                if (wex.Response != null)
                {
                    using (HttpWebResponse errorResponse = (HttpWebResponse)wex.Response)
                    {
                        throw new WebRequstBaseException(Citiport.Core.Error.LogLevel.Error, url, (int)errorResponse.StatusCode, "Error in request");
                    }
                }
            }
            finally
            {
                if (response != null) { response.Close(); }
            }
            return "{}";
        }

        public static String getRequest(String key, String def)
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
                return def;
            Object obj = current.Request[key];
            if (obj == null)
                return def;
            else
                return DataUtil.ObjetToString(obj);
        }
    }
}
