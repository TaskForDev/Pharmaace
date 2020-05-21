using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Design.PluralizationServices;
using System.Web;
using System.Data.SqlClient;
using PharmaACE.ChartAudit.Models;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.Utility
{
    public static class Util
    {
        public const string WORD_BOUNDARY = "\\b";

        //used for string.contains(substring)
        public const string WORD_BOUNDARY_START = @"(?<!\w)";
        public const string WORD_BOUNDARY_END = @"(?!\w)";

        //used for getting all words out of a string
        public const string WORD_MAKER_START = @"\w\+\/\*\%\:\-";
        public const string WORD_MAKER_END = @"\w\+\/\*\%\:\-\.";

        //used for joining words in a phrase; i.e. spaces are replaced with this symbol
        public const string PHRASE_MAKER = "|^^|";

        //used for database date format compatibility
        public const string DATE_MAKER = "-";

        public static bool IsMatchingEntity(string candidate, string entityName, List<string> entityEquivalents)
        {
            bool isMatching = false;
            if (!String.IsNullOrWhiteSpace(entityName))
            {
                isMatching = String.Compare(candidate, entityName, true) == 0;
            }

            if (!isMatching)
            {
                isMatching = entityEquivalents.Contains(candidate, StringComparer.OrdinalIgnoreCase);

                if (!isMatching)
                {
                    CultureInfo ci = new CultureInfo("en-us");
                    PluralizationService ps =
                      PluralizationService.CreateService(ci);

                    if (ps.IsPlural(candidate))
                    {
                        string pluralEntityName = ps.Pluralize(entityName);
                        isMatching = String.Compare(candidate, pluralEntityName, true) == 0;
                        if (!isMatching)
                        {
                            foreach (var str in entityEquivalents)
                            {
                                string pluralStr = ps.Pluralize(str);
                                if (String.Compare(candidate, pluralStr, true) == 0)
                                    return true;
                            }
                        }
                    }
                }
            }

            return isMatching;
        }

        public static bool IsPPT(string contentType)
        {
            return string.Compare(contentType , "application/vnd.openxmlformats-officedocument.presentationml.presentation",true) == 0 || 
                string.Compare(contentType, "application/vnd.ms-powerpoint",true) == 0;
        }

    

        public static string SafeTrim(this object obj)
        {
            if (obj == null)
                return String.Empty;
            return obj.ToString().Trim();
        }

        /// <summary>
        /// </summary>
        public static bool IsNull(object x)
        {
            if ((x == null)
                || (Convert.IsDBNull(x))
                || x.ToString().Trim().Length == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// </summary>
        public static bool IsInt(object o)
        {
            if (IsNull(o))
                return false;

            Int64 dummy = 0;
            return Int64.TryParse(o.ToString(), out dummy);
        }

        /// <summary>
        /// If not valid returns -1.
        /// </summary>
        public static int SafeToNum(this object o)
        {
            if (IsNull(o))
                return -1;
            else
            {
                if (IsInt(o))
                    return int.Parse(o.ToString());
                else
                    return -1;
            }
        }

        /// <summary>
        /// If not valid returns -1.
        /// </summary>
        public static double SafeToDouble(this object o)
        {
            double res;
            if (IsNull(o))
                return -1;
            else
            {
                if (Double.TryParse(o.SafeTrim(), out res))
                    return res;
                else
                    return -1;
            }
        }

        public static DateTime SafeToDateTime(this object o)
        {
            if (IsNull(o))
                return default(DateTime);
            else
            {
                DateTime dummy;

                if (IsInt(o))
                    return new DateTime(Convert.ToInt64(o)); // use ticks
                else
                {
                    if (DateTime.TryParse(o.ToString(), out dummy))
                        return dummy;
                    else
                        return default(DateTime);
                }
            }
        }

        public static bool AnyOrNotNull<T>(this IEnumerable<T> source)
        {
            if (source != null && source.Any())
                return true;
            else
                return false;
        }

        /// <summary>
        /// hierarchical realm for relational table
        /// assumes 1st column is non-domain primary key, hence ignored
        /// assumes left to right -> parent to child / sibling to sibling
        /// </summary>
        /// <param name="thisColumn"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool IsChildOrSiblingOf(this DataColumn thisColumn, DataColumn column)
        {
            return thisColumn.Ordinal > 0 && thisColumn.Ordinal > column.Ordinal;
        }

        /// <summary>
        /// Adds or updates value of a dictionary based on a given key
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="map"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate<T1,T2>(this Dictionary<T1, T2> map, T1 key, T2 value)
        {
            if (map == null)
                map = new Dictionary<T1, T2>();
            if (!map.ContainsKey(key))
                map.Add(key, value);
            else
                map[key] = value;
        }


        /// /// <summary>
        /// For list values mapped against the key, the list will be appended with the new value
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="map"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate<T1, T2>(this Dictionary<T1, List<T2>> map, T1 key, T2 value)
        {
            if (map == null)
                map = new Dictionary<T1, List<T2>>();
            List<T2> list = null;
            if (!map.ContainsKey(key))
                map.Add(key, new List<T2>());
            list = map[key];
            list.Add(value);
        }

        /// <summary>
        /// Adds or updates value of a hashset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate<T>(this HashSet<T> set, T value)
        {
            if (set == null)
                set = new HashSet<T>();
            if (!set.Contains(value))
                set.Add(value);
        }

        /// <summary>
        /// Adds or updates value of a hashset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <param name="values"></param>
        public static void AddOrUpdate<T>(this HashSet<T> set, List<T> values)
        {
            if (set == null)
                set = new HashSet<T>();
            foreach (var value in values)
            {
                if (!set.Contains(value))
                    set.Add(value);
            }
        }

        public static string ToFormattedDateTimeStr(this object dtObject, string format)
        {
            string result = null;
            try
            {
                DateTime dt;
                if (!DateTime.TryParse(dtObject.ToString(), out dt))
                    result = null;
                else
                    result = dt.ToString(format);
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }

        public static bool CaseInsensitiveContains(this string text, string value,
        StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }

        public static int GetTimeout
        {
            get
            {
                int Timeout;

                if (Int32.TryParse(ConfigurationManager.AppSettings["Timeout"], out Timeout))
                   return Timeout;
                return 86400;

            }
        }

        public static string mailForm
        {
            get
            {
                return ConfigurationManager.AppSettings["MailSender"];
            }
        }

        public static string MailSenderPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["MailSenderPassword"];
            }
        }

        public static string SMTPMailingHost
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPMailingHost"];
            }
        }
        public static string SMTPMailingPort
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPMailingPort"];
            }
        }


      

        public static byte[] ReadFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            int length = Convert.ToInt32(fs.Length);
            byte[] data = new byte[length];
            fs.Read(data, 0, length);
            fs.Close();
            return data;
        }

        public static string AppleDevCert
        {
            get
            {
                return ConfigurationManager.AppSettings["SandBox"];
            }
        }
        public static string AppleProdCert
        {
            get
            {
                return ConfigurationManager.AppSettings["Production"];
            }
        }

        public static string GetCertPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["CertPassword"];
            }
        }

        public static int MaxFileSize
        {
            get
            {
                int fileSize;

                if (Int32.TryParse(ConfigurationManager.AppSettings["MaxFileSize"], out fileSize))
                    return fileSize;
                return 10000000;

            }
        }

       

      
    }


    public class ActionStatus
    {
        // public int UserID { get; set; }
        public int Number { get; set; }
        public string Message { get; set; }
    }
}
