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