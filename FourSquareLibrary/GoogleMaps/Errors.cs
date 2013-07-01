using FourSquareLibrary.Citiport.Core.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourSquareLibrary
{
    /// <summary>
    /// Class contains all the exceptinos and erros
    /// Author: Jeff
    /// Date: 04-12-2008
    /// </summary>
    public class ERRORS
    {

        public ERRORS()
        {

        }
    }

    namespace Citiport.Core.Error
    {
        public enum LogLevel
        {
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        }
    }
    public class WebRequstBaseException : Exception
    {
        public String RequestUrl { set; get; }

        public LogLevel LogLevel { set; get; }

        public String Msg { set; get; }

        public long StatsCode { set; get; }

        public WebRequstBaseException() { }

        public WebRequstBaseException(LogLevel level, String url, long code, String msg)
        {
            this.RequestUrl = url;
            this.LogLevel = level;
            this.Msg = msg;
            this.StatsCode = code;
        }
    }

}
