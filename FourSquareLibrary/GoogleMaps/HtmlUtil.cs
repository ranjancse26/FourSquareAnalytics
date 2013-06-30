using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourSquareLibrary
{
    /// <summary>
    /// Summary description for HtmlUtil
    /// </summary>
    /// TODO: NOUSER NOW
    public class HtmlUtil
    {
        public HtmlUtil() { }

        public static Hashtable texToFix = new Hashtable();

        static HtmlUtil()
        {
            texToFix["0026#39;"] = "'";
            texToFix["0026quot;"] = @"""";
            texToFix["003d"] = "=";
        }

        public static String FixGoogleUrlEncode(String text)
        {
            if (text == null)
                return null;
            foreach (Object k in texToFix.Keys)
            {
                String key = DataUtil.ObjetToString(k);
                text = text.Replace(key, DataUtil.ObjetToString(texToFix[key]));
            }
            return text;
        }

    }
}
