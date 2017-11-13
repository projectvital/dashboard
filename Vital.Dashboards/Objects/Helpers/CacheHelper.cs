using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Vital.Dashboards.Objects.Helpers
{
    public static class CacheHelper
    {
        public static string GetVersionedResourcePath(string path)
        {
            string key = "VersionedResource/" + path;
            string value = "" + GetObject(key, CacheLevel.Session);
            if (string.IsNullOrWhiteSpace(value))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(path));
                string timevalue = "" + fi.LastWriteTimeUtc.ToFileTime();
                string sep = "?";
                if (path.Contains("?"))
                    sep = "&";
                value = System.Web.VirtualPathUtility.ToAbsolute(path + sep + "v=" + timevalue);

                Add(key, value, CacheLevel.Session);
            }
            return value;
        }

        public enum CacheLevel
        {
            /*Global = 0,*/
            Session = 1,
            ActiveLanguage = 2,
            Instance = 4,
            All = 7
        }
        public static string GetCachePrefix(CacheLevel level, string instance_id = null)
        {
            string prefix = "";
            //if ((level & CacheLevel.Session) == CacheLevel.Session)
            //    prefix += SessionHelper.GetGuid(SessionFields.ActiveWebServiceSession) + "_";
            return prefix;
        }
        public static object GetObject(string key, CacheLevel level, string instance_id = null)
        {
            key = GetCachePrefix(level, instance_id) + key;
            return HttpContext.Current.Cache.Get(key);
        }
        public static object Add(string key, object obj, CacheLevel level, string instance_id = null)
        {
            key = GetCachePrefix(level, instance_id) + key;
            DateTime expiration = DateTime.Now;
            expiration = expiration.AddMinutes(30);

            HttpContext.Current.Cache.Remove(key);//rmenten 2013-05-24: Added removal of key (just in case)
            return HttpContext.Current.Cache.Add(key, obj, null, expiration, TimeSpan.Zero, CacheItemPriority.Default, null); ;
        }
    }
}