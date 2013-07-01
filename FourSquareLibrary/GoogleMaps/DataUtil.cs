using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace FourSquareLibrary
{
   /// <summary>
/// Utility class for data
/// Author: Jeff
/// Date: 04/08/2008
/// </summary>
    public class DataUtil
    {
        public DataUtil() { }

        public const int UNDEFINED_ID = -1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUndefinedId(int id)
        {
            return (UNDEFINED_ID == id);
        }

        public const String WEBSISTE_URL = "http://www.citiport.net";

        public static String getWebSiteUrl()
        {
            return WEBSISTE_URL;
        }

        /// <summary>
        /// Convert object array to int array
        /// </summary>
        /// <param name="objs">array of objects</param>
        /// <returns>array of ints</returns>
        public static int[] ObjsToInts(Object[] objs)
        {
            int[] a = new int[objs.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                a[i] = Convert.ToInt32(objs[i]);

            }
            return a;
        }

        /// <summary>
        /// Copy an array fo String to Destination by Clone()
        /// </summary>
        /// <param name="source">Source array</param>
        /// <returns>Dest Array</returns>
        public static String[] StringsCopy(String[] source)
        {
            String[] dest = new String[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                dest[i] = (String)source[i].Clone();
            }
            return dest;
        }

        /// <summary>
        /// Convert the ints string to int array. EX: ints string is in the format => "1,3,4,5"
        /// </summary>
        /// <param name="ints">ints string is in the format => "1,3,4,5"</param>
        /// <returns>array of ints</returns>
        public static int[] StringIntsToInts(String ints)
        {
            if (DataUtil.IsEmptyString(ints))
                return new int[] { };
            String[] ss = ints.Split(',');
            int[] a = new int[ss.Length];
            for (int i = 0; i < ss.Length; i++)
            {
                a[i] = Convert.ToInt32(ss[i]);
            }
            return a;
        }

        /// <summary>
        /// Convert the longs string to long array. EX: ints string is in the format => "1,3,4,5"
        /// </summary>
        /// <param name="longs">longs string is in the format => "1,3,4,5"</param>
        /// <returns>array of longs</returns>
        public static long[] StringLongsToLongs(string longs)
        {
            if (DataUtil.IsEmptyString(longs))
                return new long[] { };
            String[] ss = longs.Split(',');
            long[] l = new long[ss.Length];
            for (int i = 0; i < ss.Length; i++)
            {
                l[i] = long.Parse(ss[i]);
            }
            return l;
        }

        /// <summary>
        /// Convert the ints string to int array. EX: ints string is in the format => "1,3,4,5"
        /// </summary>
        /// <param name="ints">ints string is in the format => "1,3,4,5</param>
        /// <param name="sep">seperator</param>
        /// <returns>array of ints</returns>
        public static int[] StringIntstoInts(String ints, char sep)
        {
            if (DataUtil.IsEmptyString(ints))
                return new int[] { };
            String[] ss = ints.Split(sep);
            int[] a = new int[ss.Length];
            for (int i = 0; i < ss.Length; i++)
            {
                a[i] = Convert.ToInt32(ss[i]);
            }
            return a;
        }

        public static int StringToInt(String _int)
        {
            return Convert.ToInt32(_int);
        }

        public static int StringToInt(String s, int d)
        {
            if (string.IsNullOrEmpty(s))
                return d;
            return StringToInt(s);
        }

        public static String ConcreteInts(int[] a, char sep)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < a.Length; i++)
            {
                sb.Append(a[i]);
                if (i < a.Length - 1)
                    sb.Append(sep);
            }
            return sb.ToString();
        }

        public static String ConcreteList(List<int> list, char sep)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0, c = list.Count; i < c; i++)
            {
                sb.Append(list[i]);
                if (i < c - 1)
                    sb.Append(sep);
            }
            return sb.ToString();
        }

        public static int OjectToInt(Object obj)
        {
            return Convert.ToInt32(obj);
        }

        public static int OjectToInt(Object obj, int def)
        {
            if (obj == null)
                return def;
            return OjectToInt(obj);
        }

        public static double ObjectToDouble(Object obj)
        {
            if (obj == null)
                throw new ArgumentNullException();
            else
                return Convert.ToDouble(obj);
        }

        public static String ObjetToString(Object obj)
        {
            if (obj == null)
                return null;
            else
                return obj.ToString();
        }

        public static String ObjectToString(Object obj, String def)
        {
            if (obj == null)
                return def;
            else
                return obj.ToString();
        }

        public static String IntToString(int i)
        {
            return i.ToString();
        }

        public static String IntToString(int? i)
        {
            return (i == null) ? null : i.ToString();
        }

        public static String IntsToString(int[] a, char s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < a.Length; i++)
            {
                sb.Append(a[i]);
                if (i < a.Length - 1)
                    sb.Append(s);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Return true, whether the string is empty(null, "", or...)
        /// </summary>
        /// <param name="s">input string</param>
        /// <returns></returns>
        public static bool IsEmptyString(String s)
        {
            if (s == null)
                return true;
            else if (String.Empty == s)
                return true;
            //else if (s.Length == 0)
            //    return true;
            return false;
        }

        public static bool IsBlankString(String s)
        {
            if (s == null)
                return true;
            return IsEmptyString(s.Trim());
        }

        public static String TrimString(String s)
        {
            if (s == null)
                return String.Empty;
            return s.Trim();
        }

        public static String CleanLowStirng(String s)
        {
            if (s == null)
                return null;
            else
                return s.ToLower().Trim();
        }

        public static bool StringTotalEqual(String a, String b)
        {
            if (a == null || b == null)
                return false;
            return CleanLowStirng(a).Equals(CleanLowStirng(b));
        }

        public static bool StringTotalContains(String a, String b)
        {
            if (a == null || b == null)
                return false;
            return CleanLowStirng(a).Contains(CleanLowStirng(b));
        }

        //Double
        public static decimal DoubleToDecimal(double d)
        {
            return (decimal)d;
        }

        public static Double StringToDouble(String s)
        {
            return Convert.ToDouble(s);
        }

        public static String DecimalToString(decimal d)
        {
            return d.ToString();
        }

        public static Double DecimalToDouble(decimal? d, Double def)
        {
            if (d == null)
                return def;
            return (Double)d;
        }

        /// <summary>
        /// Return a/ b * 100.0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Double Percentage(int a, int b)
        {
            Double r = Convert.ToDouble(a) / Convert.ToDouble(b) * 100.0;
            if (Double.IsInfinity(r))
                return 0.0;
            return (Double.IsNaN(r)) ? 0.0 : r;
        }

        public static String TrimEmailAfterAt(String email)
        {
            return email.Substring(0, email.LastIndexOf('@'));
        }

        public static String TimeDifferentFromNow(DateTime t)
        {
            TimeSpan ts = DateTime.Now.Subtract(t);
            string r = (ts.Days != 0) ? ts.Days + " Days " : "";
            r += (ts.Hours != 0) ? ts.Hours + " Hours " : "";
            r += (ts.Minutes != 0) ? ts.Minutes + " Minutes " : "";
            return r;
        }

        public static String UppercaseFirstFast(String s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] letters = s.ToCharArray();
            letters[0] = char.ToUpper(letters[0]);
            return new string(letters);
        }

        public static double NaNOrDef(double v, int d)
        {
            return (Double.IsNaN(v)) ? d : v;
        }

        public static int IntOrDefault(int? v, int d)
        {
            return (v == null) ? d : v.Value;
        }

        public static int IntOr0(int? v)
        {
            return IntOrDefault(v, 0);
        }

        public static Double Average(int t, int c)
        {
            return t / Convert.ToDouble(c);
        }

        public static Double StarRound(double p)
        {
            return ((p - Math.Truncate(p)) > 0) ? Math.Truncate(p) + 0.5 : p;
        }

        public static String HtmlifedString(Object o)
        {
            if (o == null)
                return null;
            return HtmlifedString(o.ToString());
        }

        public static String HtmlifedString(String s)
        {
            if (IsEmptyString(s))
                return string.Empty;
            s = HtmlLineBreak(s);
            s = HtmlMakeLink(s);
            s = HtmlUtil.FixGoogleUrlEncode(s);
            return s;
        }

        public static string HtmlifedString(String s, bool trim)
        {
            if (IsEmptyString(s))
                return string.Empty;
            s = HtmlLineBreak(s);
            s = HtmlMakeLink(s, trim);
            s = HtmlUtil.FixGoogleUrlEncode(s);
            return s;
        }

        public static String HtmlMakeLink(String txt)
        {
            return HtmlMakeLink(txt, true);
        }

        public static String HtmlMakeLink(String txt, bool trim)
        {
            String append = "";
            if ('/'.Equals(txt[txt.Length - 1]))
            {
                txt = txt.TrimEnd('/');
                append = "/";
            }

            Regex regx = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);

            MatchCollection mactches = regx.Matches(txt);

            string link_pre = string.Empty;

            if (mactches.Count == 0)
            {
                regx = new Regex("^[a-zA-Z0-9\\-\\.]+\\.(com|org|net|mil|edu|COM|ORG|NET|MIL|EDU)$", RegexOptions.IgnoreCase);
                mactches = regx.Matches(txt);
                link_pre = "http://";
            }
            List<String> _pa = new List<string>();
            foreach (Match match in mactches)
            {
                String _v = match.Value;
                if (!StringArrayContains(_pa, _v))
                {
                    if (trim)
                        _v = TrimWithAbbr(match.Value, 30, "...");
                    //txt = txt.Replace(match.Value, "<a target='_other' href='" + match.Value + "'>" + match.Value + "</a>");
                    txt = txt.Replace(match.Value, "<a class='user_post_link'  target='_other' href='" + link_pre + match.Value + "'>" + _v + "</a>");
                    _pa.Add(match.Value);
                }
            }

            return txt + append;
        }

        public static bool StringArrayContains(List<String> a, String x)
        {
            foreach (var j in a)
            {
                if (j.Contains(x) || a.Contains(j))
                    return true;
            }
            return false;
        }

        public static String HtmlLineBreak(String s)
        {
            return (s == null) ? null : s.Replace("\n", "<br/>");
        }

        public static String ConditionAppendString(String p, String b, bool cond, String sep)
        {
            if (cond)
            {
                return string.Format("{0}{1}{2}", p, sep, b);
            }
            else
            {
                return p;
            }
        }

        public const int SPOT_ABBR_LENGTH = 20;

        public static String TrimWithAbbr(String t, int len, String abbr)
        {
            if (t == null)
                return null;
            if (t.Length <= len)
                return t;
            len -= abbr.Length;
            return t.Substring(0, len) + abbr;
        }

        public static String AbbrWithDot(String t)
        {
            return TrimWithAbbr(t, SPOT_ABBR_LENGTH, "...");
        }

        public static int getRandomInt(int min, int max, int seed)
        {
            Random r = new Random(seed);
            return r.Next(min, max);
        }

        public static int getRandomInt(int min, int max)
        {
            int s = System.DateTime.Now.Millisecond;
            return getRandomInt(min, max, s);
        }

        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public static int CalculateAge(DateTime Input)
        {
            DateTime now = DateTime.Today;
            int years = now.Year - Input.Year;
            // birth day in the current year
            if (now.Month < Input.Month || (now.Month == Input.Month && now.Day < Input.Day))
                --years;
            return years;
        }

        public static bool IsStringMatched() { throw new NotImplementedException(); }

        /// <summary>
        /// From http://www.geekzilla.co.uk/viewC93F8521-8014-4CB9-8989-F2E2403A8D76.htm
        /// 
        /// string myVirtualPath = GetVirtualPath(HttpContext.Current.Request.Raw));
        /// 
        /// </summary>
        /// <param name="url">HttpContext.Current.Request.Raw</param>
        /// <returns>VirtualPath</returns>
        public static string GetVirtualPath(string url)
        {
            if (HttpContext.Current.Request.ApplicationPath == "/")
            {
                return "~" + url;
            }

            return Regex.Replace(url, "^" +
                           HttpContext.Current.Request.ApplicationPath + "(.+)$", "~$1");
        }

        public static String GetVirtualPath()
        {
            HttpContext cur = HttpContext.Current;
            if (cur == null)
                return null;
            else
            {
                return GetVirtualPath(cur.Request.Url.AbsolutePath);
            }
        }

        public static String FormatDatetime(DateTime d)
        {
            return FormatDatetime(d, "d");
        }

        public static String FormatDatetime(DateTime d, String fmt)
        {
            return d.ToString(fmt, System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// adapted from 
        /// http://cssfriendly.codeplex.com/sourcecontrol/changeset/view/24242?projectName=cssfriendly#12000
        /// </summary>
        /// <param name="container"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        /// 
        public static object GetPrivateField(object container, string fieldName)
        {
            Type type = container.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (fieldInfo == null ? null : fieldInfo.GetValue(container));
        }

        public static String NumericalOnly(String s, bool decimals)
        {
            bool hasdec = false;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= s.Length - 1; i++)
            {
                if (Char.IsDigit(s, i))
                {
                    sb.Append(s[i]);
                }
                else if (decimals && s[i] == '.' && !hasdec)
                {
                    sb.Append(s[i]);
                    hasdec = true;
                }
            }
            return sb.ToString();
        }
    }
}
