/*
    Project VITAL.Dashboard
    Copyright (C) 2017 - Universiteit Hasselt
    This project has been funded with support from the European Commission (Project number: 2015-BE02-KA203-012317). 

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

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