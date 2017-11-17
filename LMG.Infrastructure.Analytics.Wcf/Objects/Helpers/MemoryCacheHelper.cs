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
using System.Runtime.Caching;
using System.Web;

namespace LMG.Infrastructure.Analytics.Wcf.Objects.Helpers
{
    public static class MemoryCacheHelper
    {
        private static object m_cacheLock = "lock";
        private static int m_cacheTimePolicyMinutes = 5;
        public static object Get(string cacheKey)
        {
            object cachedData = MemoryCache.Default.Get(cacheKey, null);

            if (cachedData != null)
            {
                return cachedData;
            }
            return null;
        }
        public static void Set(string cacheKey, object val)
        {
            //object cachedData = MemoryCache.Default.Get(cacheKey, null);

            //if (cachedData != null)
            //{
            //    return cachedData;
            //}

            lock (m_cacheLock)
            {
                //Check to see if anyone wrote to the cache while we where waiting our turn to write the new value.
                object cachedData = MemoryCache.Default.Get(cacheKey, null);

                //if (cachedData != null)
                //{
                //    return cachedData;
                //}

                //The value still did not exist so we now write it in to the cache.
                CacheItemPolicy cip = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(m_cacheTimePolicyMinutes))
                };
                cachedData = val;// GetData();
                MemoryCache.Default.Set(cacheKey, cachedData, cip);
                //return cachedData;
            }

            //return cachedData;
        }

        public static T GetCachedData<T>(string cacheKey, object cacheLock, int cacheTimePolicyMinutes, Func<T> GetData)
            where T : class
        {
            //Returns null if the string does not exist, prevents a race condition where the cache invalidates between the contains check and the retreival.
            T cachedData = MemoryCache.Default.Get(cacheKey, null) as T;

            if (cachedData != null)
            {
                return cachedData;
            }

            lock (cacheLock)
            {
                //Check to see if anyone wrote to the cache while we where waiting our turn to write the new value.
                cachedData = MemoryCache.Default.Get(cacheKey, null) as T;

                if (cachedData != null)
                {
                    return cachedData;
                }

                //The value still did not exist so we now write it in to the cache.
                CacheItemPolicy cip = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(cacheTimePolicyMinutes))
                };
                cachedData = GetData();
                MemoryCache.Default.Set(cacheKey, cachedData, cip);
                return cachedData;
            }
        }
    }
}